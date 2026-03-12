use sqlx::FromRow;

#[derive(Debug, FromRow)]
pub struct SkillVariant {
    pub skill_variant_id: i32,
    pub name: String,
    pub skill_id: i32,
}
