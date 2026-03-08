use sqlx::FromRow;

#[derive(Debug, FromRow)]
pub struct City {
    pub city_id: i32,
    pub name: String,
}
