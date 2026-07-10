import asyncio
import logging
import os
from contextlib import asynccontextmanager

import httpx
from dotenv import load_dotenv
from fastapi import FastAPI
from fastapi.responses import PlainTextResponse

import scraper

load_dotenv()

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s %(levelname)s %(name)s: %(message)s",
)
logger = logging.getLogger(__name__)

SCRAPER_ID = "justjoinit-scraper-python"
SCRAPE_INTERVAL_SECONDS = 600
REGISTRATION_ATTEMPTS = 5
REGISTRATION_RETRY_DELAY_SECONDS = 15


async def register_scraper() -> bool:
    scheduler_url = os.environ["SCHEDULER_URL"]
    payload = {
        "id": SCRAPER_ID,
        "endpoint": os.environ["SCRAPER_ENDPOINT"],
        "interval": SCRAPE_INTERVAL_SECONDS,
    }
    async with httpx.AsyncClient() as client:
        response = await client.post(scheduler_url, json=payload)
    if response.is_success or response.status_code == 409:
        logger.info("Registered with scheduler (status %d)", response.status_code)
        return True
    logger.warning("Scheduler registration returned status %d", response.status_code)
    return False


async def handle_initial_registration() -> None:
    for attempt in range(1, REGISTRATION_ATTEMPTS + 1):
        try:
            if await register_scraper():
                return
        except httpx.HTTPError as error:
            logger.warning(
                "Scheduler registration attempt %d/%d failed: %s",
                attempt,
                REGISTRATION_ATTEMPTS,
                error,
            )
        await asyncio.sleep(REGISTRATION_RETRY_DELAY_SECONDS)
    logger.error(
        "Could not register with scheduler after %d attempts", REGISTRATION_ATTEMPTS
    )


@asynccontextmanager
async def lifespan(app: FastAPI):
    registration = asyncio.create_task(handle_initial_registration())
    yield
    registration.cancel()


app = FastAPI(lifespan=lifespan)


@app.get("/healthcheck")
async def healthcheck() -> PlainTextResponse:
    return PlainTextResponse("OK")


@app.post("/register")
async def register():
    try:
        if await register_scraper():
            return {"status": 200, "message": "Registered with scheduler."}
        return {"status": 500, "message": "Scheduler rejected registration."}
    except httpx.HTTPError:
        logger.exception("Scheduler registration failed")
        return {"status": 500, "message": "Could not reach scheduler."}


@app.get("/scrap")
async def scrap():
    try:
        offers_count = await scraper.start_scraping()
        return {
            "status": 200,
            "message": f"Scraping completed successfully. Got {offers_count} offers.",
        }
    except Exception:
        logger.exception("Scraping failed")
        return {"status": 500, "message": "Scraping failed."}
