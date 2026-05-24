use sqlx::types::chrono::{DateTime, Utc};

use crate::enums::work_status::WorkStatus;

#[allow(dead_code)]
pub struct WorkLog {
    id: i32,
    work_id: i32,
    status: WorkStatus,
    message: String,
    start_time: DateTime<Utc>,
    duration: i32,
}
