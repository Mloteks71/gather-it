use crate::models::company::Company;

pub struct CompanyRepository {}

impl CompanyRepository {
    pub async fn get_all<'e, E>(executor: E) -> color_eyre::Result<Vec<Company>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(Company, "SELECT * FROM company")
            .fetch_all(executor)
            .await?;

        Ok(result)
    }
}
