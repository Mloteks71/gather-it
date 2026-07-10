import json
import logging
import os

import aio_pika

logger = logging.getLogger(__name__)

CHUNK_SIZE = 500


async def publish_offers(offers: list[dict]) -> None:
    url = os.environ["RABBITMQ_URL"]
    exchange_name = os.environ["RABBITMQ_EXCHANGE"]
    routing_key = os.environ["RABBITMQ_ROUTING_KEY"]

    connection = await aio_pika.connect_robust(url)
    try:
        channel = await connection.channel()
        exchange = await channel.get_exchange(exchange_name)
        for start in range(0, len(offers), CHUNK_SIZE):
            chunk = offers[start : start + CHUNK_SIZE]
            await exchange.publish(
                aio_pika.Message(
                    body=json.dumps(chunk).encode(),
                    content_type="application/json",
                ),
                routing_key=routing_key,
            )
        logger.info("Published %d job ads to RabbitMQ", len(offers))
    finally:
        await connection.close()
