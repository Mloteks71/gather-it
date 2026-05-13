use serde::Serialize;

use super::response::JobOffer;

const PRACUJ_PL_SITE: u8 = 3;

#[derive(Debug, Serialize)]
#[serde(rename_all = "PascalCase")]
pub struct CommonJobAdDto {
    pub id: String,
    pub slug: String,
    pub title: String,
    pub company_name: String,
    pub source_site: u8,
    pub skills: Option<Vec<String>>,
    pub workplace_types: Option<Vec<String>>,
    pub experience_levels: Option<Vec<String>>,
    pub locations: Option<Vec<String>>,
    pub salaries: Option<Vec<SalaryRangeDto>>,
    pub published_at: Option<String>,
    pub logo_url: Option<String>,
}

#[derive(Debug, Serialize)]
#[serde(rename_all = "PascalCase")]
pub struct SalaryRangeDto {
    pub from: Option<f64>,
    pub to: Option<f64>,
    pub currency: String,
    pub contract_type: Option<String>,
}

fn extract_slug(uri: &str) -> String {
    uri.trim_end_matches('/')
        .rsplit('/')
        .next()
        .unwrap_or(uri)
        .to_string()
}

fn non_empty(v: Vec<String>) -> Option<Vec<String>> {
    if v.is_empty() { None } else { Some(v) }
}

pub fn map_offer(offer: &JobOffer) -> CommonJobAdDto {
    let slug = offer
        .offers
        .first()
        .map(|o| extract_slug(&o.offer_absolute_uri))
        .unwrap_or_else(|| offer.group_id.clone());

    let locations: Vec<String> = offer
        .offers
        .iter()
        .map(|o| o.display_workplace.clone())
        .filter(|s| !s.is_empty())
        .collect();

    CommonJobAdDto {
        id: offer.group_id.clone(),
        slug,
        title: offer.job_title.clone(),
        company_name: offer.company_name.clone(),
        source_site: PRACUJ_PL_SITE,
        skills: non_empty(offer.technologies.clone()),
        workplace_types: non_empty(offer.work_modes.clone()),
        experience_levels: non_empty(offer.position_levels.clone()),
        locations: non_empty(locations),
        salaries: None,
        published_at: offer.last_publicated.clone(),
        logo_url: offer.company_logo_uri.clone(),
    }
}
