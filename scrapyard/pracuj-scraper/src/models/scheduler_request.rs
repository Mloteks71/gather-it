use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct RegisterScraper {
    pub id: String,
    pub endpoint: String,
    pub timeout: u64,
}

impl Default for RegisterScraper {
    fn default() -> Self {
        Self {
            id: "pracuj-pl-scraper-rust".to_string(),
            endpoint: std::env::var("SCRAPER_ENDPOINT")
                .expect("SCRAPER_ENDPOINT environment variable is not set"),
            timeout: 60,
        }
    }
}
