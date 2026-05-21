FROM rust:1.92-alpine AS builder
WORKDIR /app
RUN apk add --no-cache musl-dev
COPY . .
ENV SQLX_OFFLINE=true
RUN cargo build --release

FROM alpine:3.21
WORKDIR /app
COPY --from=builder /app/target/release/binder .
CMD ["./binder"]
