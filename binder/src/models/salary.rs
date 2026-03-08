use sqlx::FromRow;

use super::enums::ContractType;

#[derive(Debug, FromRow)]
pub struct Salary {
    pub salary_id: i32,
    pub contract_type: ContractType,
    pub salary_min: Option<f32>,
    pub salary_max: Option<f32>,
    pub job_ad_id: i32,
}
