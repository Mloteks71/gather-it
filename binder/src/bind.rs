use std::collections::{HashMap, HashSet};
use std::time::Duration;

use sqlx::{Pool, Postgres};
use tokio_stream::StreamExt;
use tracing::{info, warn};

use crate::models::company::{CompanySnapshot, NewCompanySnapshot};
use crate::models::enums::OfferStatus;
use crate::models::job_ad::NewJobAd;
use crate::models::messages::CommonJobAdDto;
use crate::models::skill::{NewSkillSnapshot, SkillSnapshot};
use crate::repositories::{
    company::CompanyRepository, company_snapshot::CompanySnapshotRepository,
    company_variant::CompanyVariantRepository, job_ad::JobAdRepository,
    job_ad_skill::JobAdSkillRepository, skill::SkillRepository,
    skill_snapshot::SkillSnapshotRepository, skill_variant::SkillVariantRepository,
};

const IDLE_TIMEOUT: Duration = Duration::from_secs(5);

pub async fn bind(pool: &Pool<Postgres>, mut consumer: lapin::Consumer) -> color_eyre::Result<()> {
    let mut buffer: Vec<CommonJobAdDto> = Vec::new();

    loop {
        tokio::select! {
            delivery = consumer.next() => {
                let Some(delivery) = delivery else {
                    info!("Consumer stream ended");
                    break;
                };

                let delivery = delivery?;
                let payload = std::str::from_utf8(&delivery.data)?;

                let job_ads: Vec<CommonJobAdDto> = match serde_json::from_str(payload) {
                    Ok(ads) => ads,
                    Err(e) => {
                        warn!("Failed to deserialize payload: {e}");
                        delivery
                            .ack(lapin::options::BasicAckOptions::default())
                            .await?;
                        continue;
                    }
                };

                info!("Received {} job ads", job_ads.len());
                buffer.extend(job_ads);

                delivery
                    .ack(lapin::options::BasicAckOptions::default())
                    .await?;
            }
            _ = tokio::time::sleep(IDLE_TIMEOUT) => {
                println!("times up");
                if buffer.is_empty() {
                    info!("No messages received for {IDLE_TIMEOUT:?}, nothing to flush");
                    continue;
                }

                info!("Idle timeout reached, flushing {} job ads to database", buffer.len());

                let data = std::mem::take(&mut buffer);
                if let Err(e) = upsert_data(data, pool).await {
                    warn!("Failed to upsert data: {e}");
                }
            }
        }
    }

    Ok(())
}

async fn upsert_data(data: Vec<CommonJobAdDto>, pool: &Pool<Postgres>) -> color_eyre::Result<()> {
    let (
        all_job_ads,
        all_skills,
        all_skill_variants,
        all_skill_snapshots,
        all_companies,
        all_company_variants,
        all_company_snapshots,
    ) = tokio::try_join!(
        JobAdRepository::get_active(pool),
        SkillRepository::get_all(pool),
        SkillVariantRepository::get_all(pool),
        SkillSnapshotRepository::get_all(pool),
        CompanyRepository::get_all(pool),
        CompanyVariantRepository::get_all(pool),
        CompanySnapshotRepository::get_all(pool),
    )?;

    println!("entering binding");
    println!("data count: {}", data.len());

    let mut tx = pool.begin().await?;

    let existing_by_external_id: HashMap<&str, i32> = all_job_ads
        .iter()
        .map(|ja| (ja.external_id.as_str(), ja.job_ad_id))
        .collect();

    // Skill name/variant -> skill_id (lowercase keys)
    let mut skill_variant_to_id: HashMap<String, i32> = HashMap::new();
    for skill in &all_skills {
        skill_variant_to_id.insert(skill.name.to_lowercase(), skill.skill_id);
    }
    for sv in &all_skill_variants {
        skill_variant_to_id.insert(sv.name.to_lowercase(), sv.skill_id);
    }

    // Company name/variant -> company_id (lowercase keys)
    let mut company_variant_to_id: HashMap<String, i32> = HashMap::new();
    for company in &all_companies {
        company_variant_to_id.insert(company.name.to_lowercase(), company.company_id);
    }
    for cv in &all_company_variants {
        company_variant_to_id.insert(cv.name.to_lowercase(), cv.company_id);
    }

    // Snapshot lookups by lowercase name
    let skill_snapshot_by_name: HashMap<String, &SkillSnapshot> = all_skill_snapshots
        .iter()
        .map(|ss| (ss.name.to_lowercase(), ss))
        .collect();

    let company_snapshot_by_name: HashMap<String, &CompanySnapshot> = all_company_snapshots
        .iter()
        .map(|cs| (cs.name.to_lowercase(), cs))
        .collect();

    let incoming_external_ids: HashSet<&str> = data.iter().map(|d| d.id.as_str()).collect();

    let expired_ids: Vec<i32> = all_job_ads
        .iter()
        .filter(|ja| !incoming_external_ids.contains(ja.external_id.as_str()))
        .map(|ja| ja.job_ad_id)
        .collect();

    if !expired_ids.is_empty() {
        info!("Setting {} job ads to inactive", expired_ids.len());
        JobAdRepository::set_inactive_batch(&mut *tx, &expired_ids).await?;
    }

    let mut new_company_snapshot_ids: HashMap<String, i32> = HashMap::new();
    let mut new_skill_snapshot_ids: HashMap<String, i32> = HashMap::new();

    for cjad in &data {
        let company_name_lower = cjad.company_name.to_lowercase();
        let company_id = company_variant_to_id.get(&company_name_lower).copied();

        let mut new_job_ad = NewJobAd::from_message(cjad);
        new_job_ad.company_id = company_id;

        let job_ad_id = if let Some(&existing_id) = existing_by_external_id.get(cjad.id.as_str()) {
            new_job_ad.offer_status = OfferStatus::Active;
            JobAdRepository::update(&mut *tx, existing_id, &new_job_ad).await?;
            existing_id
        } else {
            JobAdRepository::insert(&mut *tx, &new_job_ad).await?
        };

        if company_id.is_none() {
            if let Some(&snapshot) = company_snapshot_by_name.get(&company_name_lower) {
                CompanySnapshotRepository::append_job_ad_id(
                    &mut *tx,
                    snapshot.company_snapshot_id,
                    job_ad_id,
                )
                .await?;
            } else if let Some(&snapshot_id) = new_company_snapshot_ids.get(&company_name_lower) {
                CompanySnapshotRepository::append_job_ad_id(&mut *tx, snapshot_id, job_ad_id)
                    .await?;
            } else {
                let snapshot = NewCompanySnapshot {
                    name: cjad.company_name.clone(),
                    job_ad_ids: vec![job_ad_id],
                };
                let snapshot_id = CompanySnapshotRepository::insert(&mut *tx, &snapshot).await?;
                new_company_snapshot_ids.insert(company_name_lower.clone(), snapshot_id);
            }
        }

        if let Some(skills) = &cjad.skills {
            for skill_name in skills {
                let skill_name_lower = skill_name.to_lowercase();

                if let Some(&skill_id) = skill_variant_to_id.get(&skill_name_lower) {
                    JobAdSkillRepository::insert(&mut *tx, job_ad_id, skill_id).await?;
                } else if let Some(&snapshot) = skill_snapshot_by_name.get(&skill_name_lower) {
                    SkillSnapshotRepository::append_job_ad_id(
                        &mut *tx,
                        snapshot.skill_snapshot_id,
                        job_ad_id,
                    )
                    .await?;
                } else if let Some(&snapshot_id) = new_skill_snapshot_ids.get(&skill_name_lower) {
                    SkillSnapshotRepository::append_job_ad_id(&mut *tx, snapshot_id, job_ad_id)
                        .await?;
                } else {
                    let snapshot = NewSkillSnapshot {
                        name: skill_name.clone(),
                        job_ad_ids: vec![job_ad_id],
                    };
                    let snapshot_id = SkillSnapshotRepository::insert(&mut *tx, &snapshot).await?;
                    new_skill_snapshot_ids.insert(skill_name_lower, snapshot_id);
                }
            }
        }
    }

    tx.commit().await?;

    info!(
        "Upsert complete: {} total, {} new, {} updated, {} expired",
        data.len(),
        data.len() - existing_by_external_id.len().min(data.len()),
        data.iter()
            .filter(|d| existing_by_external_id.contains_key(d.id.as_str()))
            .count(),
        expired_ids.len(),
    );

    Ok(())
}
