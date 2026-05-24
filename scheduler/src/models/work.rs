#[allow(dead_code)]
pub struct ScraperJob {
    pub id: i32,
    pub worker_id: i32,
    pub scheduled_time: sqlx::types::chrono::DateTime<sqlx::types::chrono::Utc>,
}
