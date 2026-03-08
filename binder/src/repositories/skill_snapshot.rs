use crate::models::skill::SkillSnapshot;

pub struct SkillSnapshotRepository {}

impl SkillSnapshotRepository {
    pub async fn get_all<'e, E>(executor: E) -> color_eyre::Result<Vec<SkillSnapshot>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(SkillSnapshot, "SELECT * FROM skill_snapshot")
            .fetch_all(executor)
            .await?;

        Ok(result)
    }
}