FROM rust:1.88 AS builder
WORKDIR /app

RUN apt-get update && apt-get install -y \
    pkg-config \
    cmake \
    clang \
    libclang-dev \
    build-essential \
    gcc \
    libxml2-dev \
    libsqlite3-dev \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*

ENV LIBCLANG_PATH=/usr/lib/llvm-14/lib

COPY Cargo.toml Cargo.lock ./
RUN mkdir src && echo "fn main() {}" > src/main.rs
RUN cargo build --release
RUN rm -rf src

COPY src ./src
RUN touch src/main.rs && cargo build --release

FROM debian:bookworm-slim
RUN apt-get update && apt-get install -y \
    ca-certificates \
    libxml2 \
    libsqlite3-0 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=builder /app/target/release/pracuj-scraper .
EXPOSE 8800
CMD ["./pracuj-scraper"]