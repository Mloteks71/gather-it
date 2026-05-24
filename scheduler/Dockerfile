FROM rust:1.94-alpine AS builder
WORKDIR /app
RUN apk add --no-cache musl-dev
COPY . .
RUN cargo build --release

FROM alpine:3.21
WORKDIR /app
COPY --from=builder /app/target/release/scheduler .
EXPOSE 8101
CMD ["./scheduler"]
