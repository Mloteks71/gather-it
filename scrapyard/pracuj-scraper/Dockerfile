# syntax=docker/dockerfile:1.7
FROM lukemathwalker/cargo-chef:latest-rust-1.92 AS chef
WORKDIR /app

FROM chef AS planner
COPY . .
RUN cargo chef prepare --recipe-path recipe.json

FROM chef AS builder
RUN apt-get update && apt-get install -y --no-install-recommends \
    pkg-config \
    cmake \
    clang \
    libclang-dev \
    build-essential \
    libxml2-dev \
    libsqlite3-dev \
    && rm -rf /var/lib/apt/lists/*
COPY --from=planner /app/recipe.json recipe.json
RUN --mount=type=cache,target=/usr/local/cargo/registry \
    --mount=type=cache,target=/app/target \
    cargo chef cook --release --recipe-path recipe.json
COPY . .
RUN --mount=type=cache,target=/usr/local/cargo/registry \
    --mount=type=cache,target=/app/target \
    cargo build --release --locked && \
    cp target/release/pracuj-scraper /pracuj-scraper

FROM debian:trixie-slim
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    libxml2 \
    libsqlite3-0 \
    wget \
    && rm -rf /var/lib/apt/lists/* \
    && useradd -u 10001 -m app
USER app
WORKDIR /app
COPY --from=builder --chown=app:app /pracuj-scraper ./pracuj-scraper
EXPOSE 8800
CMD ["./pracuj-scraper"]
