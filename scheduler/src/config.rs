use std::sync::OnceLock;

static CONFIG: OnceLock<Config> = OnceLock::new();

pub struct Config {
    pub database_url: String,
}

impl Config {
    pub fn from_env() -> Self {
        let _ = dotenvy::from_filename(".env");
        let _ = dotenvy::from_filename_override(".env.development");
        Self {
            database_url: std::env::var("DATABASE_URL").expect("DATABASE_URL must be set"),
        }
    }
}

pub fn config() -> &'static Config {
    CONFIG.get_or_init(Config::from_env)
}
