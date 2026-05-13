use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize, sqlx::Type, Clone, Copy)]
#[serde(try_from = "u8")]
#[sqlx(type_name = "job_site", rename_all = "snake_case")]
pub enum JobSite {
    JustJoinIt,
    TheProtocolIt,
    SolidJobs,
    PracujPl,
}

impl TryFrom<u8> for JobSite {
    type Error = String;

    fn try_from(value: u8) -> Result<Self, Self::Error> {
        match value {
            0 => Ok(JobSite::JustJoinIt),
            1 => Ok(JobSite::TheProtocolIt),
            2 => Ok(JobSite::SolidJobs),
            3 => Ok(JobSite::PracujPl),
            _ => Err(format!("Unknown JobSite value: {value}")),
        }
    }
}

#[derive(Debug, Deserialize, sqlx::Type)]
#[sqlx(type_name = "contract_type", rename_all = "snake_case")]
pub enum ContractType {
    Undefined,
    Uop,
    #[sqlx(rename = "b2b")]
    B2b,
    Uz,
    Any,
    Internship,
    Contract,
    Replacement,
}

#[derive(Debug, Clone, Copy, Deserialize, sqlx::Type)]
#[sqlx(type_name = "workplace_type", rename_all = "snake_case")]
pub enum WorkplaceType {
    Remote,
    Hybrid,
    OnSite,
}

#[derive(Debug, Clone, Copy, Deserialize, sqlx::Type)]
#[sqlx(type_name = "experience_level", rename_all = "snake_case")]
pub enum ExperienceLevel {
    Undefined,
    Junior,
    Mid,
    Senior,
    Any,
    None,
}

#[derive(Debug, Clone, Copy, Deserialize, sqlx::Type)]
#[sqlx(type_name = "offer_status", rename_all = "snake_case")]
pub enum OfferStatus {
    NewlyAdded,
    Active,
    Inactive,
}

impl Default for OfferStatus {
    fn default() -> Self {
        OfferStatus::NewlyAdded
    }
}
