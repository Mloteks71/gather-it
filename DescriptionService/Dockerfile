FROM golang:1.24 AS builder

WORKDIR /app

COPY go.mod go.sum ./
RUN go mod download

COPY . .

RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -o descriptionservice ./cmd/justJoinIt-RabbitMq-Consumer

FROM alpine:latest

RUN apk --no-cache add ca-certificates netcat-openbsd bash

WORKDIR /root/

COPY --from=builder /app/descriptionservice .
COPY --from=builder /app/wait-for-it.sh .

RUN chmod +x wait-for-it.sh

ENTRYPOINT ["./wait-for-it.sh", "rabbitmq", "5672", "--", "./descriptionservice"]

EXPOSE 8090
