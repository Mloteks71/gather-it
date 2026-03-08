use crate::models::company::CompanySnapshot;

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
}