use sqlx::Postgres;
use tracing::{error, info};

use crate::{
    enums::work_status::WorkStatus,
    models::{work::PendingWork, work_log::WorkLog},
    repositories::{
        work::{self, update_work_schedule},
        work_log::insert_work_log,
    },
};

const SCHEDULE_INTERVAL: std::time::Duration = tokio::time::Duration::from_secs(30);

#[allow(clippy::cast_sign_loss)]
pub async fn start_scheduler(pool: sqlx::Pool<Postgres>) {
    let client = reqwest::Client::builder()
        .connect_timeout(tokio::time::Duration::from_mins(1))
        .build()
        .unwrap();

    let mut interval = tokio::time::interval(SCHEDULE_INTERVAL);

    loop {
        interval.tick().await;

        let pending = match work::get_pending_work_with_worker(&pool).await {
            Ok(jobs) => jobs,
            Err(e) => {
                error!("Failed to fetch pending work: {}", e);
                continue;
            }
        };

        info!("Dispatching {} pending work item(s)", pending.len());

        for pw in pending {
            let client = client.clone();
            let executor = pool.clone();

            tokio::spawn(async move {
                let log = do_work(&client, &pw).await;
                let new_time = sqlx::types::chrono::Utc::now()
                    + std::time::Duration::from_secs(pw.interval as u64);
                if update_work_schedule(&executor, pw.id, new_time)
                    .await
                    .is_err()
                {
                    // TODO: Implement retry logic
                    error!("Failed to update schedule for work id: {}", pw.id);
                }

                if insert_work_log(&executor, &log).await.is_err() {
                    error!("Failed to insert work log for work id: {}", pw.id);
                }
            });
        }
    }
}

#[allow(clippy::cast_possible_truncation)]
async fn do_work(client: &reqwest::Client, pw: &PendingWork) -> WorkLog {
    let start = std::time::Instant::now();
    match client.get(&pw.endpoint).send().await {
        Ok(response) => {
            let elapsed = start.elapsed().as_secs() as i32;
            if response.status().is_success() {
                info!("Triggered worker {} at {}", pw.external_id, pw.endpoint);
                WorkLog::new(
                    pw.id,
                    WorkStatus::Completed,
                    None,
                    sqlx::types::chrono::Utc::now(),
                    elapsed,
                )
            } else {
                error!(
                    "Worker {} at {} responded with status code: {}",
                    pw.external_id,
                    pw.endpoint,
                    response.status()
                );
                WorkLog::new(
                    pw.id,
                    WorkStatus::Failed,
                    Some(format!("HTTP error: {}", response.status())),
                    sqlx::types::chrono::Utc::now(),
                    elapsed,
                )
            }
        }
        Err(e) => {
            let elapsed = start.elapsed().as_secs() as i32;
            error!(
                "Failed to trigger worker {} at {}: {}",
                pw.external_id, pw.endpoint, e
            );
            WorkLog::new(
                pw.id,
                WorkStatus::Failed,
                Some("Request error: {e}".to_string()),
                sqlx::types::chrono::Utc::now(),
                elapsed,
            )
        }
    }
}
