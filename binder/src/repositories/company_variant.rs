use crate::models::company_variant::CompanyVariant;

pub struct CompanyVariantRepository {}

impl CompanyVariantRepository {
    pub async fn get_all<'e, E>(
        executor: E,
    ) -> color_eyre::Result<Vec<CompanyVariant>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(CompanyVariant, "SELECT * FROM company_variant")
            .fetch_all(executor)
            .await?;

        Ok(result)
    }
}
