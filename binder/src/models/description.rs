use sqlx::FromRow;

#[derive(Debug, FromRow)]
pub struct Description {
    pub description_id: i32,
    pub job_ad_id: i32,
    pub description_text: Option<String>,
    pub requirements: Option<String>,
    pub benefits: Option<String>,
    pub workstyle: Option<String>,
    pub about_project: Option<String>,
}
