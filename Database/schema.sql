BEGIN;

CREATE TYPE job_site AS ENUM ('just_join_it', 'the_protocol_it', 'solid_jobs');
CREATE TYPE contract_type AS ENUM ('undefined', 'uop', 'b2b', 'uz', 'any', 'internship', 'contract', 'replacement');
CREATE TYPE workplace_type AS ENUM ('remote', 'hybrid', 'on_site');
CREATE TYPE experience_level AS ENUM ('undefined', 'junior', 'mid', 'senior', 'any');
CREATE TYPE offer_status AS ENUM ('newly_added', 'active', 'inactive');

CREATE TABLE job_ad (
    job_ad_id INT PRIMARY KEY,
    external_id VARCHAR(255) NOT NULL,
    title VARCHAR(500) NOT NULL,
    offer_status offer_status NOT NULL,
    workplace_type workplace_type NOT NULL,
    experience_level experience_level NOT NULL,
    company_id INT NOT NULL,
    job_site job_site NOT NULL,
    slug VARCHAR(500) NOT NULL,
    expired_at TIMESTAMP WITH TIME ZONE,
    published_at TIMESTAMP WITH TIME ZONE
);

CREATE TABLE skill (
    skill_id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    variants text[] NOT NULL
);

CREATE TABLE skill_snapshot (
    skill_snapshot_id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    job_ad_ids INT[] NOT NULL
);

CREATE TABLE company (
    company_id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    variants text[] NOT NULL
);

CREATE TABLE company_snapshot (
    company_snapshot_id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    job_ad_ids INT[] NOT NULL
);

CREATE TABLE city (
    city_id INT PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE description (
    description_id INT PRIMARY KEY,
    job_ad_id INT NOT NULL REFERENCES job_ad(job_ad_id) ON DELETE CASCADE,
    description_text TEXT NULL,
    requirements TEXT NULL,
    benefits TEXT NULL,
    workstyle TEXT NULL,
    about_project TEXT NULL
);

CREATE TABLE salary (
    salary_id INT PRIMARY KEY,
    contract_type contract_type NOT NULL,
    salary_min REAL,
    salary_max REAL,
    job_ad_id INT NOT NULL REFERENCES job_ad(job_ad_id) ON DELETE CASCADE
);

COMMIT;
