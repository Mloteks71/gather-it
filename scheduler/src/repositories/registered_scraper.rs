use crate::models::{register_scraper::RegisterScraper, registered_scraper::RegisteredScraper};

#[allow(dead_code)]
pub async fn get_all_registered_scrapers<'e, E>(
    executor: E,
) -> Result<Vec<RegisteredScraper>, sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    let result = sqlx::query_as!(
        RegisteredScraper,
        r"SELECT
            id,
            endpoint,
            timeout
        FROM registered_scraper",
    )
    .fetch_all(executor)
    .await?;

    Ok(result)
}

#[allow(dead_code)]
pub async fn get_registered_scraper_by_id<'e, E>(
    executor: E,
    scraper_id: i32,
) -> Result<RegisteredScraper, sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    let result = sqlx::query_as!(
        RegisteredScraper,
        r"SELECT
            id,
            endpoint,
            timeout
        FROM registered_scraper
        WHERE id = $1",
        scraper_id
    )
    .fetch_one(executor)
    .await?;

    Ok(result)
}

pub async fn is_scraper_registered<'e, E>(
    executor: E,
    external_id: &str,
) -> Result<bool, sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    sqlx::query!(
        "SELECT EXISTS(SELECT 1 FROM registered_scraper WHERE external_id = $1)",
        external_id
    )
    .fetch_one(executor)
    .await
    .map(|record| record.exists.unwrap_or(false))
}

pub async fn register_scraper<'e, E>(
    executor: E,
    scraper: &RegisterScraper,
) -> Result<(), sqlx::Error>
where
    E: sqlx::Executor<'e, Database = sqlx::Postgres>,
{
    sqlx::query!(
        r"INSERT INTO registered_scraper (external_id, endpoint, timeout)
            VALUES ($1, $2, $3)",
        scraper.id,
        scraper.endpoint,
        scraper.timeout,
    )
    .execute(executor)
    .await?;

    Ok(())
}
