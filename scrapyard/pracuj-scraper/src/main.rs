#![warn(clippy::all, clippy::pedantic)]
#![allow(clippy::struct_field_names)]

use axum::Router;
use color_eyre::eyre::Result;

mod models;
mod rabbitmq;
mod scraper;

#[tokio::main]
async fn main() -> Result<()> {
    color_eyre::install()?;
    let _ = dotenvy::dotenv();
    tracing_subscriber::fmt::init();
    // TODO: add registration with scheduler
    let app = Router::new()
        .route("/healthcheck", axum::routing::get(healthcheck))
        .route("/scrap", axum::routing::get(crate::scraper::start_scraping));

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3000").await.unwrap();
    let _ = axum::serve(listener, app).await;
    Ok(())
}

async fn healthcheck() -> &'static str {
    "OK"
}
