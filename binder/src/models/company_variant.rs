use sqlx::FromRow;

#[derive(Debug, FromRow)]
pub struct CompanyVariant {
    pub company_variant_id: i32,
    pub name: String,
    pub company_id: i32,
}
