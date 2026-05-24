use serde::Deserialize;
use sqlx::FromRow;

#[allow(dead_code)]
#[derive(Debug, FromRow, Deserialize)]
pub struct RegisteredScraper {
    pub id: i32,
    pub endpoint: String,
    pub timeout: i32,
}
