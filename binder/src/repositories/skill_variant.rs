use crate::models::skill_variant::SkillVariant;

pub struct SkillVariantRepository {}

impl SkillVariantRepository {
    pub async fn get_all<'e, E>(executor: E) -> color_eyre::Result<Vec<SkillVariant>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(SkillVariant, "SELECT * FROM skill_variant")
            .fetch_all(executor)
            .await?;

        Ok(result)
    }
}
