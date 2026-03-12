use chrono::{DateTime, Utc};
use serde::Deserialize;
use sqlx::FromRow;

use crate::models::messages::CommonJobAdDto;
use crate::models::skill::Skill;

use super::enums::{ExperienceLevel, JobSite, OfferStatus, WorkplaceType};

#[derive(Debug, FromRow, Deserialize)]
pub struct JobAd {
    pub job_ad_id: i32,
    pub external_id: String,
    pub title: String,
    pub offer_status: OfferStatus,
    pub workplace_type: Vec<WorkplaceType>,
    pub experience_level: Vec<ExperienceLevel>,
    pub company_id: Option<i32>,
    pub job_site: JobSite,
    pub slug: String,
    pub expired_at: Option<DateTime<Utc>>,
    pub published_at: Option<DateTime<Utc>>,
    pub skills: sqlx::types::Json<Vec<Skill>>,
}

impl JobAd {}

#[derive(Debug, FromRow, Deserialize)]
pub struct NewJobAd {
    pub external_id: String,
    pub title: String,
    pub offer_status: OfferStatus,
    pub workplace_type: Vec<WorkplaceType>,
    pub experience_level: Vec<ExperienceLevel>,
    pub company_id: Option<i32>,
    pub job_site: JobSite,
    pub slug: String,
    pub published_at: Option<DateTime<Utc>>,
}

impl NewJobAd {
    pub fn from_message(cjad: &CommonJobAdDto) -> Self {
        let workplace_type = cjad
            .workplace_types
            .as_ref()
            .map(|wts| {
                wts.iter()
                    .filter_map(|s| parse_workplace_type(s))
                    .collect()
            })
            .unwrap_or_default();

        let experience_level = cjad
            .experience_levels
            .as_ref()
            .map(|els| {
                els.iter()
                    .filter_map(|s| parse_experience_level(s))
                    .collect()
            })
            .unwrap_or_default();

        Self {
            external_id: cjad.id.clone(),
            title: cjad.title.clone(),
            offer_status: OfferStatus::NewlyAdded,
            workplace_type,
            experience_level,
            company_id: None,
            job_site: cjad.source_site,
            slug: cjad.slug.clone(),
            published_at: cjad.published_at,
        }
    }
}

fn parse_workplace_type(s: &str) -> Option<WorkplaceType> {
    match s.to_lowercase().as_str() {
        "remote" => Some(WorkplaceType::Remote),
        "hybrid" => Some(WorkplaceType::Hybrid),
        "on_site" | "onsite" | "office" => Some(WorkplaceType::OnSite),
        _ => None,
    }
}

fn parse_experience_level(s: &str) -> Option<ExperienceLevel> {
    match s.to_lowercase().as_str() {
        "junior" => Some(ExperienceLevel::Junior),
        "mid" => Some(ExperienceLevel::Mid),
        "senior" => Some(ExperienceLevel::Senior),
        "any" => Some(ExperienceLevel::Any),
        _ => None,
    }
}
