#![warn(clippy::all, clippy::pedantic)]
use axum::{
    Json, Router,
    http::StatusCode,
    routing::{get, post},
};
use serde::{Deserialize, Serialize};

#[tokio::main]

async fn main() {
    // initialize tracing
    tracing_subscriber::fmt::init();

    let app = Router::new().route("/healthcheck", get(healthcheck));

    // run our app with hyper, listening globally on port 3000
    let listener = tokio::net::TcpListener::bind("0.0.0.0:3000").await.unwrap();
    let _ = axum::serve(listener, app).await;
}

async fn healthcheck() -> &'static str {
    "OK"
}
