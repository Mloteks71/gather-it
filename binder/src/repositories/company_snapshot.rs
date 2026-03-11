use crate::models::company::{CompanySnapshot, NewCompanySnapshot};

pub struct CompanySnapshotRepository {}

impl CompanySnapshotRepository {
    pub async fn get_all<'e, E>(
        executor: E,
    ) -> color_eyre::Result<Vec<CompanySnapshot>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(CompanySnapshot, "SELECT * FROM company_snapshot")
            .fetch_all(executor)
            .await?;

        Ok(result)
    }

    pub async fn insert<'e, E>(
        executor: E,
        snapshot: &NewCompanySnapshot,
    ) -> Result<i32, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let id = sqlx::query_scalar!(
            r#"INSERT INTO company_snapshot (name, job_ad_ids)
            VALUES ($1, $2)
            RETURNING company_snapshot_id"#,
            snapshot.name,
            &snapshot.job_ad_ids,
        )
        .fetch_one(executor)
        .await?;

        Ok(id)
    }

    pub async fn append_job_ad_id<'e, E>(
        executor: E,
        company_snapshot_id: i32,
        job_ad_id: i32,
    ) -> Result<(), sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        sqlx::query!(
            r#"UPDATE company_snapshot
            SET job_ad_ids = array_append(job_ad_ids, $1)
            WHERE company_snapshot_id = $2"#,
            job_ad_id,
            company_snapshot_id,
        )
        .execute(executor)
        .await?;

        Ok(())
    }
}

