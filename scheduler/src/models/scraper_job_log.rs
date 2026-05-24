use sqlx::types::chrono::{DateTime, Utc};

use crate::enums::job_status::JobStatus;

#[allow(dead_code)]
pub struct ScraperJobLog {
    id: i32,
    scraper_job_id: i32,
    number_of_job_ads: i32,
    status: JobStatus,
    message: String,
    log_time: DateTime<Utc>,
}
