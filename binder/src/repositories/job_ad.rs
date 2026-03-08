use crate::models::enums::{ExperienceLevel, JobSite, OfferStatus, WorkplaceType};
use crate::models::job_ad::JobAd;

pub struct JobAdRepository {}

impl JobAdRepository {
    pub async fn get_all<'e, E>(executor: E) -> color_eyre::Result<Vec<JobAd>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(
            JobAd,
            r#"SELECT
                job_ad_id,
                external_id,
                title,
                offer_status as "offer_status: OfferStatus",
                workplace_type as "workplace_type: WorkplaceType",
                experience_level as "experience_level: ExperienceLevel",
                company_id,
                job_site as "job_site: JobSite",
                slug,
                expired_at,
                published_at
            FROM job_ad"#
        )
        .fetch_all(executor)
        .await?;

        Ok(result)
    }

    // TODO: add: read job ads that are expired/inactive instead of reading them all
}
