use dotenvy::{from_filename, from_filename_override};
use sqlx::postgres::PgPoolOptions;

#[tokio::main]
async fn main() {
    setup_env();

    let postgres_url = std::env::var("POSTGRES_URL").expect("POSTGRES_URL must be set");

    let pool = PgPoolOptions::new()
        .max_connections(5)
        .connect(&postgres_url)
        .await
        .expect("Failed to connect to the database");
}

fn setup_env() {
    let _ = from_filename(".env");
    let _ = from_filename_override(".env.development");
}
