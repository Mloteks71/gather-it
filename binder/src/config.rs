use std::sync::{LazyLock, OnceLock};

use dotenvy::{from_filename, from_filename_override};

static CONFIG: OnceLock<Config> = OnceLock::new();

pub struct Config {
    pub rabbitmq_url: String,
    pub rabbitmq_queue_name: String,
    pub rabbitmq_consumer_tag: String,
    pub postgres_url: String,
}

impl Config {
    pub fn from_env() -> Self {
        let _ = from_filename(".env");
        let _ = from_filename_override(".env.development");

        Self {
            rabbitmq_url: std::env::var("RABBITMQ_URL").expect("RABBITMQ_URL must be set"),
            rabbitmq_queue_name: std::env::var("RABBITMQ_QUEUE_NAME")
                .expect("RABBITMQ_QUEUE_NAME must be set"),
            rabbitmq_consumer_tag: std::env::var("RABBITMQ_CONSUMER_TAG")
                .expect("RABBITMQ_CONSUMER_TAG must be set"),
            postgres_url: std::env::var("POSTGRES_URL").expect("POSTGRES_URL must be set"),
        }
    }
}

pub fn config() -> &'static Config {
    CONFIG.get_or_init(Config::from_env)
}
