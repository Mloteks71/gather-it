#![warn(clippy::all, clippy::pedantic)]
#![allow(clippy::struct_field_names)]

use axum::Router;
use color_eyre::eyre::Result;
use tracing::info;

use crate::models::scheduler_request::RegisterScraper;

mod models;
mod rabbitmq;
mod scraper;

#[tokio::main]
async fn main() -> Result<()> {
    color_eyre::install()?;
    let _ = dotenvy::dotenv();
    tracing_subscriber::fmt::init();
    info!("Starting pracuj.pl scraper...");
    register_scraper().await?;
    info!("Scraper registered with scheduler");

    let app = Router::new()
        .route("/healthcheck", axum::routing::get(healthcheck))
        .route("/scrap", axum::routing::get(crate::scraper::start_scraping));

    let listener = tokio::net::TcpListener::bind("localhost:3000")
        .await
        .unwrap();
    info!("Pracuj.pl scraper started on port 3000");
    let _ = axum::serve(listener, app).await;

    Ok(())
}

async fn healthcheck() -> &'static str {
    "OK"
}

async fn register_scraper() -> Result<()> {
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
        .await?;
    println!("Scheduler response: {response:?}");
    Ok(())
}
