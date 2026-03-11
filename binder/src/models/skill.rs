use serde::Deserialize;
use sqlx::FromRow;

#[derive(Debug, FromRow, Deserialize)]
pub struct Skill {
    pub skill_id: i32,
    pub name: String,
}

#[derive(Debug, FromRow)]
pub struct SkillSnapshot {
    pub skill_snapshot_id: i32,
    pub name: String,
    pub job_ad_ids: Vec<i32>,
}

pub struct NewSkill {
    pub name: String,
    pub variants: Vec<String>,
}

pub struct NewSkillSnapshot {
    pub name: String,
    pub job_ad_ids: Vec<i32>,
}
