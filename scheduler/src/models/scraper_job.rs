#[allow(dead_code)]
pub struct ScraperJob {
    pub id: i32,
    pub scraper_id: i32,
    pub start_time: sqlx::types::chrono::DateTime<sqlx::types::chrono::Utc>,
}
