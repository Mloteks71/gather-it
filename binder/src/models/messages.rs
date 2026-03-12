use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};

use super::enums::JobSite;

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "PascalCase")]
pub struct CommonJobAdDto {
    pub id: String,
    pub slug: String,
    pub title: String,
    pub company_name: String,
    pub source_site: JobSite,
    pub skills: Option<Vec<String>>,
    pub workplace_types: Option<Vec<String>>,
    pub experience_levels: Option<Vec<String>>,
    pub locations: Option<Vec<String>>,
    pub salaries: Option<Vec<SalaryRangeDto>>,
    pub published_at: Option<DateTime<Utc>>,
    pub logo_url: Option<String>,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "PascalCase")]
pub struct SalaryRangeDto {
    pub from: Option<f64>,
    pub to: Option<f64>,
    pub currency: String,
    pub contract_type: Option<String>,
}
