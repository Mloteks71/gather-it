use sqlx::FromRow;

#[derive(Debug, FromRow)]
pub struct JobAdSkill {
    pub job_ad_id: i32,
    pub skill_id: i32,
}