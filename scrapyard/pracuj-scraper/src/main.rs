#![warn(clippy::all, clippy::pedantic)]
#![allow(clippy::struct_field_names)]

use axum::Router;
use color_eyre::eyre::Result;
use tracing::info;

use crate::models::scheduler_request::RegisterScraper;

mod models;
mod rabbitmq;
mod scraper;

const HOST: &str = "0.0.0.0";
const PORT: u16 = 8800;
const DELAY_BETWEEN_REGISTRATIONS_SECS: u64 = 15;
const REGISTRATION_RETRY_LIMIT: usize = 5;

#[tokio::main]
async fn main() -> Result<()> {
    color_eyre::install()?;
    let _ = dotenvy::dotenv();
    tracing_subscriber::fmt::init();
    info!("Starting pracuj.pl scraper...");

    let app = Router::new()
        .route("/register", axum::routing::post(register_scraper))
        .route("/healthcheck", axum::routing::get(healthcheck))
        .route("/scrap", axum::routing::get(crate::scraper::start_scraping));

    let listener = tokio::net::TcpListener::bind(format!("{HOST}:{PORT}"))
        .await
        .expect("Failed to bind to port 8800");

    info!("Pracuj.pl scraper started on {}:{}", HOST, PORT);

    handle_initial_registration().await;

    let _ = axum::serve(listener, app).await;

    Ok(())
}

async fn handle_initial_registration() {
    for attempt in 1..=REGISTRATION_RETRY_LIMIT {
        let (status, message) = register_scraper().await;
        if status.is_success() {
            info!("Successfully registered scraper: {message}");
            return;
        }
        info!(
            "Attempt {attempt}/{REGISTRATION_RETRY_LIMIT} - Failed to register scraper: {message}"
        );
        if attempt < REGISTRATION_RETRY_LIMIT {
            tokio::time::sleep(std::time::Duration::from_secs(
                DELAY_BETWEEN_REGISTRATIONS_SECS,
            ))
            .await;
        }
    }
    info!(
        "Exceeded maximum registration attempts. Scraper will continue running without registration."
    );
}

async fn healthcheck() -> &'static str {
    "OK"
}

async fn register_scraper() -> (axum::http::StatusCode, String) {
    let scheduler_url =
        std::env::var("SCHEDULER_URL").expect("SCHEDULER_URL environment variable is not set");

    let response = reqwest::Client::new()
        .post(scheduler_url)
        .header("Content-Type", "application/json")
        .body(
            serde_json::to_string(&RegisterScraper::default())
                .expect("Failed to serialize RegisterScraper"),
        )
        .send()
        .await;

    match response {
        Ok(resp) => {
            if resp.status().is_success() {
                (
                    axum::http::StatusCode::OK,
                    "Scraper registered successfully".to_string(),
                )
            } else {
                (
                    axum::http::StatusCode::INTERNAL_SERVER_ERROR,
                    format!("Failed to register scraper: HTTP {}", resp.status()),
                )
            }
        }
        Err(e) => (
            axum::http::StatusCode::INTERNAL_SERVER_ERROR,
            format!("Failed to register scraper: {e}"),
        ),
    }
}
