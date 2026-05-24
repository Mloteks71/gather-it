#![warn(clippy::all, clippy::pedantic)]

use axum::{
    Json, Router,
    http::StatusCode,
    routing::{get, post},
};
use sqlx::Postgres;
use tracing::{info, warn};

use crate::repositories::worker;

mod config;
mod enums;
mod models;
mod repositories;
mod scheduler;

const HOST: &str = "0.0.0.0";
const PORT: u16 = 8101;

#[derive(Clone)]
struct AppState {
    pool: sqlx::Pool<Postgres>,
}

#[tokio::main]
async fn main() {
    tracing_subscriber::fmt::init();
    config::config();
    tracing::info!("Starting scheduler...");

    let Ok(pool) = sqlx::postgres::PgPoolOptions::new()
        .max_connections(5)
        .connect(&config::config().database_url)
        .await
    else {
        panic!("cannot connect to database");
    };

    let shared_state = AppState { pool };

    info!("Connected to PostgreSQL database");

    let app = Router::new()
        .route("/healthcheck", get(healthcheck))
        .route("/register", post(register))
        .with_state(shared_state);

    let listener = tokio::net::TcpListener::bind(format!("{HOST}:{PORT}"))
        .await
        .unwrap();

    let server = tokio::spawn(async {
        tracing::info!("Server started on {}:{}", HOST, PORT);
        let _ = axum::serve(listener, app).await;
    });

    let bg_job = tokio::spawn(async {
        tracing::info!("Background service started");
    });

    let _ = tokio::try_join!(server, bg_job);
}

async fn healthcheck() -> &'static str {
    "OK"
}

async fn register(
    axum::extract::State(state): axum::extract::State<AppState>,
    Json(payload): Json<crate::models::register_worker::RegisterWorker>,
) -> StatusCode {
    let mut transaction = match state.pool.begin().await {
        Ok(txn) => txn,
        Err(e) => {
            warn!(
                "Failed to begin transaction for worker id: {}, error: {}",
                payload.id, e
            );
            return StatusCode::INTERNAL_SERVER_ERROR;
        }
    };

    match worker::is_worker_registered(&mut *transaction, &payload.id).await {
        Ok(true) => return StatusCode::CONFLICT,
        Err(_) => return StatusCode::INTERNAL_SERVER_ERROR,
        Ok(false) => {}
    }

    let Ok(()) = worker::register_worker(&mut *transaction, &payload).await else {
        warn!("Failed to register with id: {}", payload.id);
        return StatusCode::INTERNAL_SERVER_ERROR;
    };

    if transaction.commit().await.is_err() {
        warn!("Failed to commit transaction for worker id: {}", payload.id);
        return StatusCode::INTERNAL_SERVER_ERROR;
    }

    info!(
        "Registered worker with id: {}, url: {}, interval: {}",
        payload.id, payload.endpoint, payload.interval
    );
    StatusCode::OK
}
