#![allow(dead_code)]

use crate::models::{register_worker::RegisterWorker, worker::Worker};

pub async fn get_all_workers<'e, E>(executor: E) -> Result<Vec<Worker>, sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    let result = sqlx::query_as!(
        Worker,
        r"SELECT
            id,
            external_id,
            endpoint,
            interval
        FROM worker",
    )
    .fetch_all(executor)
    .await?;

    Ok(result)
}

pub async fn get_worker_by_id<'e, E>(executor: E, worker_id: i32) -> Result<Worker, sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    let result = sqlx::query_as!(
        Worker,
        r"SELECT
            id,
            external_id,
            endpoint,
            interval
        FROM worker
        WHERE id = $1",
        worker_id
    )
    .fetch_one(executor)
    .await?;

    Ok(result)
}

pub async fn is_worker_registered<'e, E>(
    executor: E,
    external_id: &str,
) -> Result<bool, sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    sqlx::query!(
        "SELECT EXISTS(SELECT 1 FROM worker WHERE external_id = $1)",
        external_id
    )
    .fetch_one(executor)
    .await
    .map(|record| record.exists.unwrap_or(false))
}

pub async fn register_worker<'e, E>(
    executor: E,
    register_worker: &RegisterWorker,
) -> Result<i32, sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    let record = sqlx::query!(
        r"INSERT INTO worker (external_id, endpoint, interval)
            VALUES ($1, $2, $3)
            RETURNING id",
        register_worker.id,
        register_worker.endpoint,
        register_worker.interval,
    )
    .fetch_one(executor)
    .await?;

    Ok(record.id)
}
