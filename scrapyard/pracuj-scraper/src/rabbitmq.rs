use color_eyre::eyre::Result;
use lapin::{BasicProperties, Connection, ConnectionProperties, options::BasicPublishOptions};
use tracing::info;

use crate::models::message::{CommonJobAdDto, map_offer};
use crate::models::response::JobOffer;

const MAPPING_EXCHANGE: &str = "mapping_exchange";
const MAPPING_ROUTING_KEY: &str = "mapping";

pub async fn setup_rmq(rabbitmq_url: &str) -> Result<()> {
    let conn = Connection::connect(rabbitmq_url, ConnectionProperties::default()).await?;
    let channel = conn.create_channel().await?;

    match channel
        .exchange_declare(
            MAPPING_EXCHANGE.into(),
            lapin::ExchangeKind::Direct,
            lapin::options::ExchangeDeclareOptions {
                passive: true,
                ..Default::default()
            },
            lapin::types::FieldTable::default(),
        )
        .await
    {
        Ok(_) => info!("Connected to '{}' exchange", MAPPING_EXCHANGE),
        Err(_) => panic!("'{}' exchange does not exist", MAPPING_EXCHANGE),
    }

    Ok(())
}

pub async fn publish_offers(offers: &[JobOffer], rabbitmq_url: &str) -> Result<()> {
    let messages: Vec<CommonJobAdDto> = offers.iter().map(map_offer).collect();

    let payload = serde_json::to_vec(&messages)?;

    let conn = Connection::connect(rabbitmq_url, ConnectionProperties::default()).await?;
    let channel = conn.create_channel().await?;

    channel
        .basic_publish(
            MAPPING_EXCHANGE.into(),
            MAPPING_ROUTING_KEY.into(),
            BasicPublishOptions::default(),
            &payload,
            BasicProperties::default().with_content_type("application/json".into()),
        )
        .await?
        .await?;

    info!("Published {} job ads to RabbitMQ", messages.len());

    Ok(())
}
