use chrono::{DateTime, Utc};
use serde::Deserialize;
use sqlx::FromRow;

use super::enums::{ExperienceLevel, JobSite, OfferStatus, WorkplaceType};

#[derive(Debug, FromRow, Deserialize)]
pub struct JobAd {
    pub job_ad_id: i32,
    pub external_id: String,
    pub title: String,
    pub offer_status: OfferStatus,
    pub workplace_type: WorkplaceType,
    pub experience_level: ExperienceLevel,
    pub company_id: i32,
    pub job_site: JobSite,
    pub slug: String,
    pub expired_at: Option<DateTime<Utc>>,
    pub published_at: Option<DateTime<Utc>>,
}
