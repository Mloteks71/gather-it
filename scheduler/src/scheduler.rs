#![allow(dead_code)]
#![allow(clippy::unused_async)]
use sqlx::Postgres;

const SCHEDULE_INTERVAL: std::time::Duration = tokio::time::Duration::from_mins(5);

pub async fn start_scheduler(_pool: &sqlx::Pool<Postgres>) {}
