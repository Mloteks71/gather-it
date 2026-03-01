mod config;

use lapin::types::ShortString;
use sqlx::{Postgres, postgres::PgPoolOptions};

use crate::config::config;

#[tokio::main]
async fn main() {
    let pool = setup_postgres().await;
    let consumer = setup_rmq().await;
}

async fn setup_postgres() -> sqlx::Pool<Postgres> {
    PgPoolOptions::new()
        .max_connections(5)
        .connect(&config().postgres_url)
        .await
        .expect("Failed to connect to the database")
}

async fn setup_rmq() -> lapin::Consumer {
    let conn_options = lapin::ConnectionProperties::default();

    let conn = lapin::Connection::connect(&config().rabbitmq_url, conn_options)
        .await
        .expect("Failed to connect to RabbitMQ");

    let channel = conn
        .create_channel()
        .await
        .expect("Failed to create channel");

    let consumer_options = lapin::options::BasicConsumeOptions::default();

    channel
        .basic_consume(
            ShortString::from(config().rabbitmq_consumer_tag.clone()),
            ShortString::from(config().rabbitmq_consumer_tag.clone()),
            consumer_options,
            Default::default(),
        )
        .await
        .expect("Failed to create consumer")
}
