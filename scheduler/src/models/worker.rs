use serde::Deserialize;
use sqlx::FromRow;

#[allow(dead_code)]
#[derive(Debug, FromRow, Deserialize)]
pub struct Worker {
    pub id: i32,
    pub external_id: String,
    pub endpoint: String,
    pub interval: i32,
}
