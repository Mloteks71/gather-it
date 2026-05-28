#![allow(dead_code)]

use sqlx::types::chrono::{DateTime, Utc};

pub struct Work {
    pub id: i32,
    pub worker_id: i32,
    pub scheduled_time: DateTime<Utc>,
}

impl Work {
    pub fn new(worker_id: i32, scheduled_time: DateTime<Utc>) -> Self {
        Self {
            id: 0,
            worker_id,
            scheduled_time,
        }
    }
}

pub struct PendingWork {
    pub id: i32,
    pub worker_id: i32,
    pub scheduled_time: DateTime<Utc>,
    pub external_id: String,
    pub endpoint: String,
    pub interval: i32,
}
