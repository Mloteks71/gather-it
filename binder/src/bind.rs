use std::collections::HashMap;
use std::time::Duration;

use sqlx::{Pool, Postgres};
use tokio_stream::StreamExt;
use tracing::{info, warn};

use crate::models::company::NewCompanySnapshot;
use crate::models::job_ad::{JobAd, NewJobAd};
use crate::models::messages::CommonJobAdDto;
use crate::models::skill::NewSkillSnapshot;
use crate::repositories::{
    company::CompanyRepository, company_snapshot::CompanySnapshotRepository,
    company_variant::CompanyVariantRepository, job_ad::JobAdRepository,
    job_ad_skill::JobAdSkillRepository, skill::SkillRepository,
    skill_snapshot::SkillSnapshotRepository, skill_variant::SkillVariantRepository,
};

const IDLE_TIMEOUT: Duration = Duration::from_mins(1);
const FAILED_PAYLOAD_DIR: &str = "failed_payloads";

pub async fn bind(pool: &Pool<Postgres>, mut consumer: lapin::Consumer) -> color_eyre::Result<()> {
    std::fs::create_dir(FAILED_PAYLOAD_DIR)
        .unwrap_or_else(|e| warn!("Failed to create failed payloads directory: {e}"));

    let mut buffer: Vec<CommonJobAdDto> = Vec::new();

    loop {
        tokio::select! {
            delivery = consumer.next() => {
                let Some(delivery) = delivery else {
                    info!("Consumer stream ended, flushing {} buffered job ads", buffer.len());
                    if !buffer.is_empty() {
                        let data = std::mem::take(&mut buffer);
                        if let Err(e) = upsert_data(data, pool).await {
                            warn!("Failed to flush buffer on disconnect: {e}");
                        }
                    }
                    break;
                };
                let delivery = delivery?;
                let payload = std::str::from_utf8(&delivery.data)?;

                let job_ads: Vec<CommonJobAdDto> = match serde_json::from_str(payload) {
                    Ok(ads) => ads,
                    Err(e) => {
                        std::fs::write(format!("{}/{}.txt", FAILED_PAYLOAD_DIR, chrono::Utc::now().timestamp()), payload)?;
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
            () = tokio::time::sleep(IDLE_TIMEOUT) => {
                if buffer.is_empty() {
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

#[allow(clippy::too_many_lines)]
async fn upsert_data(data: Vec<CommonJobAdDto>, pool: &Pool<Postgres>) -> color_eyre::Result<()> {
    let start = std::time::Instant::now();
    let data_len = data.len();
    let mut tx = pool.begin().await?;
    let external_ids: Vec<String> = data.iter().map(|d| d.id.clone()).collect();

    let (
        existing_job_ads,
        all_skills,
        all_skill_variants,
        all_skill_snapshots,
        all_companies,
        all_company_variants,
        all_company_snapshots,
    ) = tokio::try_join!(
        JobAdRepository::get_by_external_ids(pool, &external_ids),
        SkillRepository::get_all(pool),
        SkillVariantRepository::get_all(pool),
        SkillSnapshotRepository::get_all(pool),
        CompanyRepository::get_all(pool),
        CompanyVariantRepository::get_all(pool),
        CompanySnapshotRepository::get_all(pool),
    )?;

    let existing_job_ads: HashMap<String, JobAd> = existing_job_ads
        .into_iter()
        .map(|ja| (ja.external_id.clone(), ja))
        .collect();

    // prepare lookups for companies
    let mut company_by_variant = HashMap::new();

    for company in &all_companies {
        company_by_variant.insert(&company.name, company.company_id);
    }
    for cv in &all_company_variants {
        company_by_variant.insert(&cv.name, cv.company_id);
    }

    // prepare lookups for company snapshots
    let mut company_snapshot_by_name = all_company_snapshots
        .iter()
        .map(|cs| (&cs.name, cs.company_snapshot_id))
        .collect::<HashMap<&String, i32>>();

    // prepare lookups for skill
    let mut skill_by_variant: HashMap<&String, i32> = HashMap::new();
    for skill in &all_skills {
        skill_by_variant.insert(&skill.name, skill.skill_id);
    }
    for sv in &all_skill_variants {
        skill_by_variant.insert(&sv.name, sv.skill_id);
    }

    // prepare lookups for skill snapshots
    let mut skill_snapshot_by_name: HashMap<&String, i32> = all_skill_snapshots
        .iter()
        .map(|ss| (&ss.name, ss.skill_snapshot_id))
        .collect();

    let new_data: Vec<CommonJobAdDto> = data
        .into_iter()
        .filter(|ja| !existing_job_ads.contains_key(&ja.id))
        .collect();

    // main uspert loop
    for job_ad_to_insert in &new_data {
        let mut new_job_ad = NewJobAd::from_message(job_ad_to_insert);
        // handle company if exists
        new_job_ad.company_id = company_by_variant
            .get(&job_ad_to_insert.company_name)
            .copied();

        let new_job_ad_id = JobAdRepository::insert(pool, &new_job_ad).await?;

        // If no company exists, check if a snapshot exists
        if new_job_ad.company_id.is_none() {
            if let Some(company_snapshot_id) =
                company_snapshot_by_name.get(&job_ad_to_insert.company_name)
            {
                CompanySnapshotRepository::append_job_ad_id(
                    &mut *tx,
                    *company_snapshot_id,
                    new_job_ad_id,
                )
                .await?;
            } else {
                // If no snapshot exists, insert a new company snapshot
                let new_company_snapshot = NewCompanySnapshot {
                    name: job_ad_to_insert.company_name.clone(),
                    job_ad_ids: vec![new_job_ad_id],
                };
                let new_company_snapshot_id =
                    CompanySnapshotRepository::insert(&mut *tx, &new_company_snapshot).await?;
                company_snapshot_by_name
                    .insert(&job_ad_to_insert.company_name, new_company_snapshot_id);
            }
        }

        // handle skills if exists
        if let Some(ja_skills) = &job_ad_to_insert.skills {
            for ja_skill in ja_skills {
                // if the skills exists append new pair (job_ad, skill_id)
                if let Some(&skill_id) = skill_by_variant.get(ja_skill) {
                    JobAdSkillRepository::insert(&mut *tx, new_job_ad_id, skill_id).await?;
                }
                // if the skill snapshot exists, append new job_ad_id to the snapshot
                else if let Some(skill_snapshot_id) = skill_snapshot_by_name.get(&ja_skill) {
                    SkillSnapshotRepository::append_skill_snapshot_job_ad_id(
                        &mut *tx,
                        *skill_snapshot_id,
                        new_job_ad_id,
                    )
                    .await?;
                }
                // if no skill snapshot exists, appen a new skill snapshot
                else {
                    let new_skill_snapshot = NewSkillSnapshot {
                        name: ja_skill.clone(),
                        job_ad_ids: vec![new_job_ad_id],
                    };
                    let new_skill_snapshot_id =
                        SkillSnapshotRepository::insert(&mut *tx, &new_skill_snapshot).await?;
                    skill_snapshot_by_name.insert(ja_skill, new_skill_snapshot_id);
                }
            }
        }
    }

    tx.commit().await?;

    info!(
        "Upsert complete: {} total, {} new in {}ms.",
        data_len,
        new_data.len(),
        start.elapsed().as_millis()
    );
    Ok(())
}
