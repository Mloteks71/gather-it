BEGIN;

CREATE TYPE job_status AS ENUM ('completed', 'failed');

CREATE TABLE registered_scraper (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    external_id VARCHAR(255) NOT NULL,
    endpoint VARCHAR(255) NOT NULL,
    timeout INT NOT NULL
);

CREATE TABLE scraper_job (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    scraper_id INT NOT NULL REFERENCES registered_scraper(id) ON DELETE CASCADE,
    start_time TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE scraper_job_log (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    scraper_job_id INT NOT NULL REFERENCES scraper_job(id) ON DELETE CASCADE,
    number_of_job_ads INT NOT NULL,
    status job_status NOT NULL,
    message VARCHAR(255) NULL,
    log_time TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

COMMIT;
