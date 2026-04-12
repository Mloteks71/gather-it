use color_eyre::eyre::{self, Result, WrapErr, bail};
use scraper::{Html, Selector};
use serde_json::Value;
use tracing::info;
use wreq::Client;
use wreq_util::Emulation;

use crate::models::response::JobOffer;

mod models;

const BASE_URL: &str = "https://it.pracuj.pl/praca";
const MAX_PAGES_SELECTOR: &str = "button.listing_n19df7xb:nth-child(6)";
const SCRIPT_DATA_SELECTOR: &str = "script#__NEXT_DATA__";

fn build_client() -> Result<wreq::Client> {
    let client = wreq::Client::builder()
        .emulation(Emulation::Chrome124)
        .build()?;

    Ok(client)
}

async fn fetch_page(client: &Client, url: &str) -> Result<Html> {
    info!(url, "fetching page");
    let response = client
        .get(url)
        .send()
        .await
        .wrap_err_with(|| format!("request failed for {url}"))?;

    let status = response.status();

    if !status.is_success() {
        bail!("non-200 response for {url}: {status}");
    }

    Ok(Html::parse_document(response.text().await?.as_str()))
}

fn get_total_pages(html: &Html) -> Result<usize> {
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
// we can find a <script> tag that contains all dthe data listed on the page and parse it
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

    println!("data from script tag: {:#?}", job_offers);

    info!(
        "successfully extracted data from script tag, found {} offers",
        job_offers.len()
    );

    Ok(job_offers)
}

#[tokio::main]
async fn main() -> Result<()> {
    color_eyre::install()?;
    tracing_subscriber::fmt::init();

    let client = build_client()?;

    info!("fetching first listing page");
    let first_page_html = fetch_page(&client, BASE_URL).await?;

    let total_pages = get_total_pages(&first_page_html)?;
    info!(total_pages, "discovered total pages");

    let _offers = get_data_from_sript_tag(&first_page_html)?;

    return Ok(());
}
