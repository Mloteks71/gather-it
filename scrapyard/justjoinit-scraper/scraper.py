import asyncio
import logging
import os
import time

import httpx

from mapper import map_offers
from rabbitmq import publish_offers

logger = logging.getLogger(__name__)

HTTP_HEADERS = {"Version": "2"}
REQUEST_TIMEOUT_SECONDS = 30


async def start_scraping() -> int:
    started = time.monotonic()
    api_url = os.environ["API_URL"]

    async with httpx.AsyncClient(
        headers=HTTP_HEADERS, timeout=REQUEST_TIMEOUT_SECONDS
    ) as client:
        first_page = await fetch_page(client, api_url, 1)
        offers = list(first_page["data"])
        total_pages = first_page["meta"]["totalPages"]

        if total_pages > 1:
            pages = await asyncio.gather(
                *(
                    fetch_page(client, api_url, page_number)
                    for page_number in range(2, total_pages + 1)
                )
            )
            for page in pages:
                offers.extend(page.get("data") or [])

    if not offers:
        logger.warning("JustJoinIt returned no job postings.")
        return 0

    await publish_offers(map_offers(offers))

    elapsed_ms = int((time.monotonic() - started) * 1000)
    logger.info(
        "Fetched %d job ads from JustJoinIt in %d ms", len(offers), elapsed_ms
    )
    return len(offers)


async def fetch_page(client: httpx.AsyncClient, api_url: str, page_number: int) -> dict:
    response = await client.get(f"{api_url}{page_number}")
    response.raise_for_status()
    return response.json()
