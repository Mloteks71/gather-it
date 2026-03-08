use crate::models::skill::Skill;

pub struct SkillRepository {}

impl SkillRepository {
    pub async fn get_all<'e, E>(executor: E) -> color_eyre::Result<Vec<Skill>, sqlx::Error>
    where
        E: sqlx::Executor<'e, Database = sqlx::Postgres>,
    {
        let result = sqlx::query_as!(Skill, "SELECT * FROM skill")
            .fetch_all(executor)
            .await?;

        Ok(result)
    }
}

