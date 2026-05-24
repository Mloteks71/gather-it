use serde::{Deserialize, Serialize};

#[derive(Default, Debug, Serialize, Deserialize)]
pub struct RegisterScraper {
    pub id: String,
    pub endpoint: String,
    pub timeout: i32,
}

impl RegisterScraper {
    pub fn new() -> Self {
        Self {
            id: "pracuj-pl-scraper-rust".to_string(),
            endpoint: std::env::var("SCRAPER_ENDPOINT")
                .expect("SCRAPER_ENDPOINT environment variable is not set"),
            timeout: 60,
        }
    }
}
