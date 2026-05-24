use std::sync::Arc;

use crate::{models::pracujpl_response::JobOffer, rabbitmq};
use color_eyre::eyre::{self, Result, bail};
use scraper::{Html, Selector};
use serde_json::Value;
use tokio::{sync::Semaphore, task::JoinSet};
use tracing::{info, warn};
use wreq::Client;
use wreq_util::Emulation;

const BASE_URL: &str = "https://it.pracuj.pl/praca";
const PAGES_PER_BUFFER: usize = 10;
const MAX_PAGES_SELECTOR: &str = "button.listing_n19df7xb:nth-child(6)";
const SCRIPT_DATA_SELECTOR: &str = "script#__NEXT_DATA__";

pub async fn start_scraping() -> (axum::http::StatusCode, String) {
    match do_scraping().await {
        Ok(msg) => (axum::http::StatusCode::OK, msg),
        Err(e) => (axum::http::StatusCode::INTERNAL_SERVER_ERROR, e.to_string()),
    }
}

async fn do_scraping() -> Result<String> {
    let client = build_client()?;
    let channel = rabbitmq::setup_rmq().await?;

    let start_timer = std::time::Instant::now();

    let first_page_content = fetch_page(&client, BASE_URL).await?;
    let number_of_pages;
    let mut offers;
    {
        let first_page_html = Html::parse_document(&first_page_content);
        number_of_pages = get_number_of_pages(&first_page_html)?;
        offers = get_data_from_script_tag(&first_page_html)?;
        // first_page_html dropped here
    }

    info!(number_of_pages, "discovered total pages");

    let semaphore = Arc::new(Semaphore::new(PAGES_PER_BUFFER));
    let mut set = JoinSet::new();
    for page_number in 2..=number_of_pages {
        let client = client.clone();
        let url = build_url(page_number);
        let sem = semaphore.clone();
        set.spawn(async move {
            let _permit = sem.acquire().await.unwrap();
            fetch_page(&client, &url).await
        });
    }

    let mut pages = Vec::new();
    while let Some(result) = set.join_next().await {
        match result {
            Ok(Ok(html)) => pages.push(html),
            Ok(Err(e)) => warn!("fetch error: {e}"),
            Err(e) => warn!("task panicked: {e}"),
        }
    }

    // TODO: optimize parsing
    for page in &pages {
        let parsed = Html::parse_document(page);
        match get_data_from_script_tag(&parsed) {
            Ok(mut jobs) => offers.append(&mut jobs),
            Err(e) => warn!("error while deserializing data {e}"),
        }
    }

    let elapsed = start_timer.elapsed();
    info!("scraped: {} in {}ms", offers.len(), elapsed.as_millis());

    rabbitmq::publish_offers(&channel, &offers).await?;
    Ok(format!(
        "Scraping completed successfully. Got {} offers.",
        offers.len()
    ))
}
fn build_client() -> Result<wreq::Client> {
    let client = wreq::Client::builder()
        .emulation(Emulation::Chrome124)
        .build()?;

    Ok(client)
}

fn build_url(page_number: usize) -> String {
    format!("{BASE_URL}?pn={page_number}")
}

async fn fetch_page(client: &Client, url: &str) -> Result<String> {
    info!(url, "fetching page");

    let Ok(body) = client.get(url).send().await else {
        bail!("failed to fetch page");
    };

    let Ok(body) = body.text().await else {
        bail!("failed to read response body");
    };

    Ok(body)
}

fn get_number_of_pages(html: &Html) -> Result<usize> {
    let Ok(selector) = Selector::parse(MAX_PAGES_SELECTOR) else {
        bail!("failed to parse pagination button selector");
    };

    Ok(html
        .select(&selector)
        .next()
        .ok_or_else(|| eyre::eyre!("element not found"))?
        .text()
        .collect::<String>()
        .parse::<usize>()?)
}

// instead of scraping each individual offer separetely
// we can find a <script> tag that contains all the data listed on the page and parse it
fn get_data_from_script_tag(html: &Html) -> Result<Vec<JobOffer>> {
    let selector = Selector::parse(SCRIPT_DATA_SELECTOR).unwrap();

    let data = html
        .select(&selector)
        .next()
        .ok_or_else(|| eyre::eyre!("element not found"))?
        .inner_html();

    let v: Value = serde_json::from_str(&data)?;

    let v =
        &v["props"]["pageProps"]["dehydratedState"]["queries"][0]["state"]["data"]["groupedOffers"];

    let job_offers = serde_json::from_value::<Vec<JobOffer>>(v.clone())?;

    // info!(
    //     "successfully extracted data from script tag, found {} offers",
    //     job_offers.len()
    // );

    Ok(job_offers)
}
