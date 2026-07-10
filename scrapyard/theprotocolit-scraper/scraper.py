import asyncio
import logging
import os
import time

import httpx

from mapper import map_offers
from rabbitmq import publish_offers

logger = logging.getLogger(__name__)

REQUEST_TIMEOUT_SECONDS = 30
PAGE_DELAY_SECONDS = 0.3


def http_headers() -> dict:
    return {
        "Cache-Control": "no-cache",
        "x-xsrf-token": os.environ["TP_XSRF_TOKEN"],
        "Cookie": os.environ["TP_COOKIE"],
    }


async def start_scraping() -> int:
    started = time.monotonic()
    api_url = os.environ["API_URL"]

    async with httpx.AsyncClient(
        headers=http_headers(), timeout=REQUEST_TIMEOUT_SECONDS
    ) as client:
        first_page = await fetch_page(client, api_url, 1)
        offers = list(first_page["offers"])
        total_pages = first_page["page"]["count"]

        for page_number in range(2, total_pages + 1):
            await asyncio.sleep(PAGE_DELAY_SECONDS)
            page = await fetch_page(client, api_url, page_number)
            offers.extend(page.get("offers") or [])

    if not offers:
        logger.warning("TheProtocolIt returned no job postings.")
        return 0

    await publish_offers(map_offers(offers))

    elapsed_ms = int((time.monotonic() - started) * 1000)
    logger.info(
        "Fetched %d job ads from TheProtocolIt in %d ms", len(offers), elapsed_ms
    )
    return len(offers)


async def fetch_page(client: httpx.AsyncClient, api_url: str, page_number: int) -> dict:
    response = await client.post(
        f"{api_url}{page_number}",
        content=b"",
        headers={"Content-Type": "application/json; charset=utf-8"},
    )
    response.raise_for_status()
    return response.json()
