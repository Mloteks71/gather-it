use crate::models::work::{PendingWork, Work};

#[allow(dead_code)]
pub async fn get_pending_work<'e, E>(executor: E) -> Result<Vec<Work>, sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    let result = sqlx::query_as!(
        Work,
        r"SELECT
            id,
            worker_id,
            scheduled_time
        FROM work
        WHERE scheduled_time <= NOW()",
    )
    .fetch_all(executor)
    .await?;

    Ok(result)
}

pub async fn get_pending_work_with_worker<'e, E>(
    executor: E,
) -> Result<Vec<PendingWork>, sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    let result = sqlx::query_as!(
        PendingWork,
        r"SELECT
            w.id,
            w.worker_id,
            w.scheduled_time,
            wr.external_id,
            wr.endpoint,
            wr.interval
        FROM work w
        JOIN worker wr ON wr.id = w.worker_id
        WHERE w.scheduled_time <= NOW()",
    )
    .fetch_all(executor)
    .await?;

    Ok(result)
}

pub async fn insert_work<'e, E>(executor: E, work: &Work) -> Result<i32, sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    let record = sqlx::query!(
        r"INSERT INTO work (worker_id, scheduled_time)
            VALUES ($1, $2)
            RETURNING id",
        work.worker_id,
        work.scheduled_time,
    )
    .fetch_one(executor)
    .await?;

    Ok(record.id)
}

pub async fn update_work_schedule<'e, E>(
    executor: E,
    work_id: i32,
    new_time: sqlx::types::chrono::DateTime<sqlx::types::chrono::Utc>,
) -> Result<(), sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    sqlx::query!(
        "UPDATE work SET scheduled_time = $1 WHERE id = $2",
        new_time,
        work_id
    )
    .execute(executor)
    .await?;

    Ok(())
}
