use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct JobOffer {
    pub technologies: Vec<String>,
    pub about_project_short_description: String,
    pub group_id: String,
    pub job_title: String,
    pub company_name: String,
    pub company_profile_absolute_uri: String,
    pub company_id: u64,
    pub company_logo_uri: String,
    pub last_publicated: String,
    pub expiration_date: String,
    pub salary_display_text: String,
    pub job_description: String,
    pub is_remote_work_allowed: bool,
    pub offers: Vec<Offer>,
    pub position_levels: Vec<String>,
    pub types_of_contract: Vec<String>,
    pub work_schedules: Vec<String>,
    pub work_modes: Vec<String>,
    pub primary_attributes: Vec<PrimaryAttribute>,
    pub common_offer_id: Option<String>,
    pub mobile_banner_uri: Option<String>,
    pub desktop_banner_uri: Option<String>,
    pub ai_summary: String,
    pub applied_products: Vec<serde_json::Value>,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Offer {
    pub partition_id: u64,
    pub offer_absolute_uri: String,
    pub display_workplace: String,
    pub is_whole_poland: bool,
    pub applied_products: Vec<serde_json::Value>,
    pub coordinates: Option<Coordinates>,
    pub distance_in_kilometers: Option<f64>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct Coordinates {
    pub latitude: f64,
    pub longitude: f64,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct PrimaryAttribute {
    pub code: String,
    pub label: AttributeLabel,
    pub model: AttributeModel,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct AttributeLabel {
    pub text: String,
    pub pracuj_pl_text: String,
    pub primary_target_site_text: String,
}

#[derive(Debug, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct AttributeModel {
    pub model_type: String,
    pub flag: Option<bool>,
}
