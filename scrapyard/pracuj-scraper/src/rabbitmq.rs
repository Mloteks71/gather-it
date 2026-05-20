use color_eyre::eyre::Result;
use lapin::{
    BasicProperties, Channel, Connection, ConnectionProperties, options::BasicPublishOptions,
};
use tracing::info;

use crate::models::pracujpl_response::JobOffer;
use crate::models::rmq_message::{CommonJobAdDto, map_offer};

pub async fn setup_rmq() -> Result<Channel> {
    let rabbitmq_url = std::env::var("RABBITMQ_URL").expect("RABBITMQ_URL must be set");
    let exchange = std::env::var("RABBITMQ_EXCHANGE").expect("RABBITMQ_EXCHANGE must be set");

    let conn = Connection::connect(&rabbitmq_url, ConnectionProperties::default()).await?;
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
        Ok(()) => info!("Connected to '{}' exchange", exchange),
        Err(e) => panic!("'{exchange}' exchange does not exist: {e}"),
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
