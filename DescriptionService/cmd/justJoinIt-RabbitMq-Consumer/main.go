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
		SourceURL:    "https://api.example.com/offers",
		TargetURL:    "https://api.example.com/processed",
		RequestDelay: 250 * time.Millisecond,
		MaxRetries:   3,
		RetryDelay:   2 * time.Second,
		Headers: map[string]string{
			"Authorization": "Bearer your-token",
			"User-Agent":    "RequestProcessor/1.0",
		},
	}

	// RabbitMQ connection URL
	rabbitMQURL := "amqp://rabbituser:rabbitpass@localhost:5672/%2F/JustJoinItDescription"

	// Connect to RabbitMQ
	conn, err := amqp.Dial(rabbitMQURL)
	if err != nil {
		log.Fatalf("Failed to connect to RabbitMQ: %v", err)
	}
	defer conn.Close()

	// Create a channel
	ch, err := conn.Channel()
	if err != nil {
		log.Fatalf("Failed to open a channel: %v", err)
	}
	defer ch.Close()

	// Declare a queue (this will create it if it doesn't exist)
	queueName := "JustJoinItDescription"
	q, err := ch.QueueDeclare(
		queueName, // name
		false,     // durable
		true,      // delete when unused
		false,     // exclusive
		false,     // no-wait
		nil,       // arguments
	)
	if err != nil {
		log.Fatalf("Failed to declare a queue: %v", err)
	}

	// Consume messages from the queue
	msgs, err := ch.Consume(
		q.Name,                          // queue
		"JustJoinItDescriptionConsumer", // consumer
		false,                           // auto-ack (set to false to manually acknowledge)
		false,                           // exclusive
		false,                           // no-local
		false,                           // no-wait
		nil,                             // args
	)
	if err != nil {
		log.Fatalf("Failed to register a consumer: %v", err)
	}

	// Set up a channel to handle shutdown signals
	stopChan := make(chan os.Signal, 1)
	signal.Notify(stopChan, syscall.SIGINT, syscall.SIGTERM)

	log.Printf(" [*] Waiting for messages. To exit press CTRL+C")

	// Process messages in a goroutine
	go func() {
		for d := range msgs {
			log.Printf("Received a message: %s", d.Body)

			// Simulate message processing
			time.Sleep(1 * time.Second)

			var payload model.DescriptionInput
			if err := json.Unmarshal(d.Body, &payload); err != nil {
				log.Printf("Error decoding JSON: %v", err)
				d.Nack(false, false) // Discard malformed message
				continue
			}

			// Access your data
			processor := justJoinIt.NewProcessor(config)
			// for _, item := range payload.Items {
			// 	log.Printf("Processing item ID: %d, Description: %s", item.Id, item.Slug)

			// 	// Your business logic here
			// 	if err := processItem(item); err != nil {
			// 		log.Printf("Error processing item %d: %v", item.Id, err)
			// 		d.Nack(false, true) // Requeue on failure
			// 		continue
			// 	}
			// }
			// Process items (will send batch automatically at the end)
			if err := processor.ProcessItems(payload.Items); err != nil {
				log.Fatalf("Processing failed: %v", err)
			}

			log.Println("Processing completed successfully")

			// // Acknowledge after successful processing
			// d.Ack(false)

			// Acknowledge the message
			if err := d.Ack(false); err != nil {
				log.Printf("Error acknowledging message: %v", err)
			}
		}
	}()

	// Wait for shutdown signal
	<-stopChan
	log.Println("Shutting down consumer...")
}
