use crate::models::enums::{ExperienceLevel, JobSite, OfferStatus, WorkplaceType};
use crate::models::job_ad::{JobAd, NewJobAd};
use crate::models::skill::Skill;
use sqlx::types::Json;

pub struct JobAdRepository {}

impl JobAdRepository {
    pub async fn get_all<'e, E>(executor: E) -> color_eyre::Result<Vec<JobAd>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(
            JobAd,
            r#"SELECT
                ja.job_ad_id,
                ja.external_id,
                ja.title,
                ja.offer_status as "offer_status: OfferStatus",
                ja.workplace_type as "workplace_type: Vec<WorkplaceType>",
                ja.experience_level as "experience_level: Vec<ExperienceLevel>",
                ja.company_id,
                ja.job_site as "job_site: JobSite",
                ja.slug,
                ja.expired_at,
                ja.published_at,
                COALESCE(
                    (SELECT json_agg(json_build_object('skill_id', s.skill_id, 'name', s.name))
                     FROM job_ad_skill jas
                     JOIN skill s ON jas.skill_id = s.skill_id
                     WHERE jas.job_ad_id = ja.job_ad_id),
                    '[]'::json
                ) as "skills!: Json<Vec<Skill>>"
            FROM job_ad ja"#
        )
        .fetch_all(executor)
        .await?;

        Ok(result)
    }

    pub async fn get_active<'e, E>(executor: E) -> color_eyre::Result<Vec<JobAd>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(
            JobAd,
            r#"SELECT
                ja.job_ad_id,
                ja.external_id,
                ja.title,
                ja.offer_status as "offer_status: OfferStatus",
                ja.workplace_type as "workplace_type: Vec<WorkplaceType>",
                ja.experience_level as "experience_level: Vec<ExperienceLevel>",
                ja.company_id,
                ja.job_site as "job_site: JobSite",
                ja.slug,
                ja.expired_at,
                ja.published_at,
                COALESCE(
                    (SELECT json_agg(json_build_object('skill_id', s.skill_id, 'name', s.name))
                     FROM job_ad_skill jas
                     JOIN skill s ON jas.skill_id = s.skill_id
                     WHERE jas.job_ad_id = ja.job_ad_id),
                    '[]'::json
                ) as "skills!: Json<Vec<Skill>>"
            FROM job_ad ja
            WHERE ja.offer_status != 'inactive'"#
        )
        .fetch_all(executor)
        .await?;

        Ok(result)
    }

    pub async fn insert<'e, E>(executor: E, job_ad: &NewJobAd) -> Result<i32, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let row = sqlx::query_scalar!(
            r#"INSERT INTO job_ad (external_id, title, offer_status, workplace_type, experience_level, company_id, job_site, slug, published_at)
            VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9)
            RETURNING job_ad_id"#,
            job_ad.external_id,
            job_ad.title,
            job_ad.offer_status as OfferStatus,
            &job_ad.workplace_type as &[WorkplaceType],
            &job_ad.experience_level as &[ExperienceLevel],
            job_ad.company_id,
            job_ad.job_site as JobSite,
            job_ad.slug,
            job_ad.published_at,
        )
        .fetch_one(executor)
        .await?;

        Ok(row)
    }

    pub async fn update<'e, E>(
        executor: E,
        job_ad_id: i32,
        job_ad: &NewJobAd,
    ) -> Result<(), sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        sqlx::query!(
            r#"UPDATE job_ad
            SET title = $1,
                workplace_type = $2,
                experience_level = $3,
                slug = $4,
                published_at = $5,
                company_id = $6,
                offer_status = $7
            WHERE job_ad_id = $8"#,
            job_ad.title,
            &job_ad.workplace_type as &[WorkplaceType],
            &job_ad.experience_level as &[ExperienceLevel],
            job_ad.slug,
            job_ad.published_at,
            job_ad.company_id,
            job_ad.offer_status as OfferStatus,
            job_ad_id,
        )
        .execute(executor)
        .await?;

        Ok(())
    }

    pub async fn set_inactive_batch<'e, E>(
        executor: E,
        job_ad_ids: &[i32],
    ) -> Result<(), sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        sqlx::query!(
            r#"UPDATE job_ad SET offer_status = 'inactive' WHERE job_ad_id = ANY($1)"#,
            job_ad_ids,
        )
        .execute(executor)
        .await?;

        Ok(())
    }
}
