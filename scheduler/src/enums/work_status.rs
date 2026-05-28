#[allow(dead_code)]
#[derive(sqlx::Type)]
#[sqlx(type_name = "work_status", rename_all = "lowercase")]
pub enum WorkStatus {
    Completed,
    Failed,
}
