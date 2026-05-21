#![warn(clippy::all, clippy::pedantic)]
use axum::{
    Json, Router,
    http::StatusCode,
    routing::{get, post},
};

mod models;

const HOST: &str = "0.0.0.0";
const PORT: u16 = 8101;

#[tokio::main]
async fn main() {
    // initialize tracing
    tracing_subscriber::fmt::init();
    tracing::info!("Starting scheduler...");

    let app = Router::new()
        .route("/healthcheck", get(healthcheck))
        .route("/register-scraper", post(register_scraper));

    // run our app with hyper, listening globally on port 3000
    let listener = tokio::net::TcpListener::bind(format!("{HOST}:{PORT}"))
        .await
        .unwrap();

    tracing::info!("Scheduler started on {}:{}", HOST, PORT);

    let _ = axum::serve(listener, app).await;
}

async fn healthcheck() -> &'static str {
    "OK"
}

async fn register_scraper(
    Json(payload): Json<crate::models::register_scraper::RegisterScraper>,
) -> StatusCode {
    tracing::info!("Registering scraper: {:?}", payload);
    StatusCode::OK
}
