mod bind;
mod config;
mod models;
mod repositories;

use sqlx::{Postgres, postgres::PgPoolOptions};

use crate::config::config;

#[tokio::main]
async fn main() -> color_eyre::Result<()> {
    let pool = setup_postgres().await?;
    let consumer = setup_rmq().await?;

    bind::bind(&pool, consumer).await?;

    Ok(())
}

async fn setup_postgres() -> color_eyre::Result<sqlx::Pool<Postgres>> {
    Ok(PgPoolOptions::new()
        .max_connections(5)
        .connect(&config().database_url)
        .await?)
}

async fn setup_rmq() -> color_eyre::Result<lapin::Consumer> {
    let conn_options = lapin::ConnectionProperties::default();

    let conn = lapin::Connection::connect(&config().rabbitmq_url, conn_options).await?;

    let channel = conn.create_channel().await?;

    let consumer_options = lapin::options::BasicConsumeOptions::default();

    Ok(channel
        .basic_consume(
            config().rabbitmq_queue_name.clone().into(),
            config().rabbitmq_consumer_tag.clone().into(),
            consumer_options,
            Default::default(),
        )
        .await?)
}
