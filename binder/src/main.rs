mod bind;
mod config;
mod models;
mod repositories;

use sqlx::{Postgres, postgres::PgPoolOptions};
use tracing::info;
use tracing_subscriber::{EnvFilter, fmt, layer::SubscriberExt, util::SubscriberInitExt};

use crate::config::config;

#[tokio::main]
async fn main() -> color_eyre::Result<()> {
    config();
    let _guard = setup_tracing();
    info!("Starting application");

    let pool = setup_postgres().await?;
    let consumer = setup_rmq().await?;

    bind::bind(&pool, consumer).await?;

    Ok(())
}

fn setup_tracing() -> tracing_appender::non_blocking::WorkerGuard {
    color_eyre::install().expect("Failed to install color_eyre");

    let file_appender = tracing_appender::rolling::daily("logs", "binder.log");
    let (non_blocking, _guard) = tracing_appender::non_blocking(file_appender);

    tracing_subscriber::registry()
        .with(EnvFilter::from_default_env())
        // for stdout logs
        // .with(fmt::layer().with_writer(std::io::stdout))
        .with(fmt::layer().with_writer(non_blocking).with_ansi(false))
        .init();

    _guard
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
