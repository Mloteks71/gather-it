use sqlx::{Pool, Postgres};
use tokio_stream::StreamExt;

use crate::repositories::{
    company::CompanyRepository, company_snapshot::CompanySnapshotRepository,
    job_ad::JobAdRepository, skill::SkillRepository, skill_snapshot::SkillSnapshotRepository,
};

pub async fn bind(pool: &Pool<Postgres>, mut consumer: lapin::Consumer) -> color_eyre::Result<()> {
    let (all_job_ads, all_skills, all_skill_snapshots, all_companies, all_companies_snapshots) = tokio::try_join!(
        JobAdRepository::get_all(pool),
        SkillRepository::get_all(pool),
        SkillSnapshotRepository::get_all(pool),
        CompanyRepository::get_all(pool),
        CompanySnapshotRepository::get_all(pool)
    )?;

    while let Some(delivery) = consumer.next().await {
        let delivery = delivery?;
        let payload = std::str::from_utf8(&delivery.data)?;
        println!("{}", payload);
        // tracing::info!("Received message: {}", payload);
        delivery
            .ack(lapin::options::BasicAckOptions::default())
            .await?;
    }

    Ok(())
}
