package main

import (
	"encoding/json"
	"log"
	"os"
	"os/signal"
	"syscall"
	"time"

	"gather.it/DescriptionService/internal/justJoinIt"
	model "gather.it/DescriptionService/internal/sharedModel"
	amqp "github.com/rabbitmq/amqp091-go"
)

func main() {
	config := model.Config{
		SourceURL:    "https://justjoin.it/job-offer/",
		TargetURL:    "http://webapi:8080/api/JobAd/Description",
		RequestDelay: 250 * time.Millisecond,
		MaxRetries:   3,
		RetryDelay:   2 * time.Second,
		Headers: map[string]string{
			"rsc": "1",
		},
	}

	rabbitMQURL := os.Getenv("RABBITMQ_URL")
	if rabbitMQURL == "" {
		rabbitMQURL = "amqp://rabbituser:rabbitpass@rabbitmq:5672/"
	}

	conn := connectToRabbitMQ(rabbitMQURL, 10, 2*time.Second)
	defer conn.Close()

	ch, err := conn.Channel()
	if err != nil {
		log.Fatalf("Failed to open a channel: %v", err)
	}
	defer ch.Close()

	queueName := "JustJoinItDescription"

	msgs, err := ch.Consume(
		queueName,
		"JustJoinItDescriptionConsumer", // consumer tag
		false,                           // auto-ack
		false,                           // exclusive
		false,                           // no-local
		false,                           // no-wait
		nil,                             // args
	)
	if err != nil {
		log.Fatalf("Failed to register a consumer: %v", err)
	}

	stopChan := make(chan os.Signal, 1)
	signal.Notify(stopChan, syscall.SIGINT, syscall.SIGTERM)

	log.Printf(" [*] Waiting for messages. To exit press CTRL+C")

	go func() {
		for d := range msgs {

			var payload model.DescriptionInput
			if err := json.Unmarshal(d.Body, &payload); err != nil {
				log.Printf("Error decoding JSON: %v", err)
				d.Nack(false, false)
				continue
			}
			processor := justJoinIt.NewProcessor(config)
			if err := processor.ProcessItems(payload); err != nil {
				log.Printf("Processing failed: %v", err)
				d.Nack(false, true)
				continue
			}
			log.Println("Processing completed successfully")

			if err := d.Ack(false); err != nil {
				log.Printf("Error acknowledging message: %v", err)
			}
		}
	}()

	<-stopChan
	log.Println("Shutting down consumer...")
}

func connectToRabbitMQ(rabbitMQURL string, maxRetries int, retryDelay time.Duration) *amqp.Connection {
	var conn *amqp.Connection
	var err error

	for i := 0; i < maxRetries; i++ {
		conn, err = amqp.Dial(rabbitMQURL)
		if err == nil {
			log.Printf("Connected to RabbitMQ")
			return conn
		}

		log.Printf("Failed to connect to RabbitMQ (attempt %d/%d): %v", i+1, maxRetries, err)
		time.Sleep(retryDelay)
	}

	log.Fatalf("Could not connect to RabbitMQ after %d attempts: %v", maxRetries, err)
	return nil
}
