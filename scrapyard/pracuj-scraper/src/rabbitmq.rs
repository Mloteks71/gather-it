use color_eyre::eyre::Result;
use lapin::{
    BasicProperties, Channel, Connection, ConnectionProperties, options::BasicPublishOptions,
};
use tracing::info;

use crate::models::message::{CommonJobAdDto, map_offer};
use crate::models::response::JobOffer;

pub async fn setup_rmq(rabbitmq_url: &str) -> Result<Channel> {
    let exchange = std::env::var("RABBITMQ_EXCHANGE").expect("RABBITMQ_EXCHANGE must be set");

    let conn = Connection::connect(rabbitmq_url, ConnectionProperties::default()).await?;
    let channel = conn.create_channel().await?;

    match channel
        .exchange_declare(
            exchange.as_str().into(),
            lapin::ExchangeKind::Direct,
            lapin::options::ExchangeDeclareOptions {
                passive: true,
                ..Default::default()
            },
            lapin::types::FieldTable::default(),
        )
        .await
    {
        Ok(_) => info!("Connected to '{}' exchange", exchange),
        Err(_) => panic!("'{}' exchange does not exist", exchange),
    }

    Ok(channel)
}

pub async fn publish_offers(channel: &Channel, offers: &[JobOffer]) -> Result<()> {
    let exchange = std::env::var("RABBITMQ_EXCHANGE").expect("RABBITMQ_EXCHANGE must be set");
    let routing_key =
        std::env::var("RABBITMQ_ROUTING_KEY").expect("RABBITMQ_ROUTING_KEY must be set");

    let messages: Vec<CommonJobAdDto> = offers.iter().map(map_offer).collect();
    let payload = serde_json::to_vec(&messages)?;

    channel
        .basic_publish(
            exchange.as_str().into(),
            routing_key.as_str().into(),
            BasicPublishOptions::default(),
            &payload,
            BasicProperties::default().with_content_type("application/json".into()),
        )
        .await?
        .await?;

    info!("Published {} job ads to RabbitMQ", messages.len());

    Ok(())
}
