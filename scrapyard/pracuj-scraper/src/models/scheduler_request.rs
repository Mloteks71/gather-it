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
            endpoint: "http://localhost:3000/scrape".to_string(),
            timeout: 60,
        }
    }
}
