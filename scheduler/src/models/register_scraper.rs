use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct RegisterScraper {
    // unique identifier for the scraper provided by the scraper itself
    pub id: String,
    // url to call to trigger scraping
    pub endpoint: String,
    // seconds
    pub timeout: i32,
}
