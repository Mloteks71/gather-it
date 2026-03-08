use serde::Deserialize;

#[derive(Debug, Deserialize, sqlx::Type)]
#[sqlx(type_name = "job_site", rename_all = "snake_case")]
pub enum JobSite {
    JustJoinIt,
    TheProtocolIt,
    SolidJobs,
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

#[derive(Debug, Deserialize, sqlx::Type)]
#[sqlx(type_name = "workplace_type", rename_all = "snake_case")]
pub enum WorkplaceType {
    Remote,
    Hybrid,
    OnSite,
}

#[derive(Debug, Deserialize, sqlx::Type)]
#[sqlx(type_name = "experience_level", rename_all = "snake_case")]
pub enum ExperienceLevel {
    Undefined,
    Junior,
    Mid,
    Senior,
    Any,
}

#[derive(Debug, Deserialize, sqlx::Type)]
#[sqlx(type_name = "offer_status", rename_all = "snake_case")]
pub enum OfferStatus {
    NewlyAdded,
    Active,
    Inactive,
}
