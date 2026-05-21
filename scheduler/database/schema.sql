BEGIN;

CREATE TYPE scraper_status AS ENUM ('completed', 'failed');

CREATE TABLE registered_scraper (
    scraper_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    endpoint VARCHAR(255) NOT NULL,
    timeout INT NOT NULL
);

CREATE TABLE scraper_job (
    scraper_job_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    scraper_id INT NOT NULL REFERENCES registered_scraper(scraper_id) ON DELETE CASCADE,
    start_time TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE scraper_job_log (
    scraper_job_log_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    scraper_job_id INT NOT NULL REFERENCES scraper_job(scraper_job_id) ON DELETE CASCADE,
    number_of_job_ads INT NOT NULL,
    status scraper_status NOT NULL,
    message VARCHAR(255) NULL,
    log_time TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

COMMIT;
