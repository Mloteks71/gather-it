#![allow(dead_code)]

use crate::{enums::work_status::WorkStatus, models::work_log::WorkLog};

pub async fn insert_work_log<'e, E>(executor: E, work_log: &WorkLog) -> Result<(), sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    sqlx::query!(
        r"INSERT INTO work_log (work_id, status, message, duration)
            VALUES ($1, $2, $3, $4)",
        work_log.work_id,
        &work_log.status as &WorkStatus,
        work_log.message,
        work_log.duration,
    )
    .execute(executor)
    .await?;

    Ok(())
}
