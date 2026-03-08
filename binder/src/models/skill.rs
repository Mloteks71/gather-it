use sqlx::FromRow;

#[derive(Debug, FromRow)]
pub struct Skill {
    pub skill_id: i32,
    pub name: String,
    pub variants: Vec<String>,
}

#[derive(Debug, FromRow)]
pub struct SkillSnapshot {
    pub skill_snapshot_id: i32,
    pub name: String,
    pub job_ad_ids: Vec<i32>,
}
