use sqlx::FromRow;

#[derive(Debug, FromRow)]
pub struct Company {
    pub company_id: i32,
    pub name: String,
}

#[derive(Debug, FromRow)]
pub struct CompanySnapshot {
    pub company_snapshot_id: i32,
    pub name: String,
    pub job_ad_ids: Vec<i32>,
}

pub struct NewCompany {
    pub name: String,
    pub variants: Vec<String>,
}

pub struct NewCompanySnapshot {
    pub name: String,
    pub job_ad_ids: Vec<i32>,
}
