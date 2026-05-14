use color_eyre::eyre::{self, Result, bail};
use futures::stream::{self, StreamExt};
use scraper::{Html, Selector};
use serde_json::Value;
use tracing::{info, warn};
use wreq::Client;
use wreq_util::Emulation;

use crate::models::response::JobOffer;

mod models;
mod rabbitmq;

const BASE_URL: &str = "https://it.pracuj.pl/praca";
const PAGES_PER_BUFFER: usize = 10;
const MAX_PAGES_SELECTOR: &str = "button.listing_n19df7xb:nth-child(6)";
const SCRIPT_DATA_SELECTOR: &str = "script#__NEXT_DATA__";

fn build_client() -> Result<wreq::Client> {
    let client = wreq::Client::builder()
        .emulation(Emulation::Chrome124)
        .build()?;

    Ok(client)
}

fn build_url(page_number: usize) -> String {
    format!("https://it.pracuj.pl/praca?pn={page_number}")
}

async fn fetch_page(client: &Client, url: String) -> Result<Html> {
    info!(url, "fetching page");

    let body = client.get(url).send().await?;

    Ok(Html::parse_document(body.text().await?.as_str()))
}

fn get_number_of_pages(html: &Html) -> Result<usize> {
    let Ok(selector) = Selector::parse(MAX_PAGES_SELECTOR) else {
        bail!("failed to parse pagination button selector");
    };

    Ok(html
        .select(&selector)
        .next()
        .unwrap()
        .text()
        .collect::<String>()
        .parse::<usize>()?)
}

// instead of scraping each individual offer separetely
// we can find a <script> tag that contains all the data listed on the page and parse it
fn get_data_from_sript_tag(html: &Html) -> Result<Vec<JobOffer>> {
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

    info!(
        "successfully extracted data from script tag, found {} offers",
        job_offers.len()
    );

    Ok(job_offers)
}

#[tokio::main]
async fn main() -> Result<()> {
    color_eyre::install()?;
    let _ = dotenvy::dotenv();
    tracing_subscriber::fmt::init();

    let rabbitmq_url = std::env::var("RABBITMQ_URL").expect("RABBITMQ_URL must be set");

    let client = build_client()?;

    let channel = rabbitmq::setup_rmq(&rabbitmq_url).await?;

    let start_timer = std::time::Instant::now();

    let first_page_html = (fetch_page(&client, BASE_URL.to_string()).await)?;

    let number_of_pages = get_number_of_pages(&first_page_html)?;

    info!(number_of_pages, "discovered total pages");

    let mut offers = get_data_from_sript_tag(&first_page_html)?;

    let pages_to_fetch = stream::iter(2..=number_of_pages)
        .map(|page_number| fetch_page(&client, build_url(page_number)))
        .buffer_unordered(PAGES_PER_BUFFER)
        .collect::<Vec<_>>()
        .await;

    for resp in pages_to_fetch.iter() {
        match resp {
            Ok(html) => match get_data_from_sript_tag(html) {
                Ok(mut jobs) => offers.append(&mut jobs),
                Err(e) => warn!("error while deserializing data {e}"),
            },
            Err(e) => warn!("error while fetching data {e}"),
        }
    }

    let end_timer = std::time::Instant::now();

    info!(
        "scraped: {} in {}ms",
        offers.len(),
        std::time::Duration::as_millis(&(end_timer - start_timer))
    );

    rabbitmq::publish_offers(&channel, &offers).await?;

    Ok(())
}
