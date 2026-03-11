use crate::models::job_ad_skill::JobAdSkill;

pub struct JobAdSkillRepository {}

impl JobAdSkillRepository {
    pub async fn get_all<'e, E>(executor: E) -> color_eyre::Result<Vec<JobAdSkill>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(JobAdSkill, "SELECT job_ad_id, skill_id FROM job_ad_skill")
            .fetch_all(executor)
            .await?;

        Ok(result)
    }

    pub async fn get_by_job_ad_id<'e, E>(
        executor: E,
        job_ad_id: i32,
    ) -> color_eyre::Result<Vec<JobAdSkill>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(
            JobAdSkill,
            "SELECT job_ad_id, skill_id FROM job_ad_skill WHERE job_ad_id = $1",
            job_ad_id
        )
        .fetch_all(executor)
        .await?;

        Ok(result)
    }

    pub async fn get_by_skill_id<'e, E>(
        executor: E,
        skill_id: i32,
    ) -> color_eyre::Result<Vec<JobAdSkill>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(
            JobAdSkill,
            "SELECT job_ad_id, skill_id FROM job_ad_skill WHERE skill_id = $1",
            skill_id
        )
        .fetch_all(executor)
        .await?;

        Ok(result)
    }

    pub async fn insert<'e, E>(
        executor: E,
        job_ad_id: i32,
        skill_id: i32,
    ) -> Result<(), sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        sqlx::query!(
            r#"INSERT INTO job_ad_skill (job_ad_id, skill_id)
            VALUES ($1, $2)
            ON CONFLICT (job_ad_id, skill_id) DO NOTHING"#,
            job_ad_id,
            skill_id,
        )
        .execute(executor)
        .await?;

        Ok(())
    }
}