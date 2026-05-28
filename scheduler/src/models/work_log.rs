use sqlx::types::chrono::{DateTime, Utc};

use crate::enums::work_status::WorkStatus;

#[allow(dead_code)]
pub struct WorkLog {
    pub id: i32,
    pub work_id: i32,
    pub status: WorkStatus,
    pub message: Option<String>,
    pub start_time: DateTime<Utc>,
    pub duration: i32,
}

impl WorkLog {
    pub fn new(
        work_id: i32,
        status: WorkStatus,
        message: Option<String>,
        start_time: DateTime<Utc>,
        duration: i32,
    ) -> Self {
        Self {
            id: 0,
            work_id,
            status,
            message,
            start_time,
            duration,
        }
    }
}
