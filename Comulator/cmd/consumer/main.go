package main

import (
	"context"
	"encoding/json"
	"log"
	"os"
	"os/signal"
	"strings"
	"syscall"
	"time"

	model "gather.it/Comulator/internal/model"
	amqp "github.com/rabbitmq/amqp091-go"
	kafka "github.com/segmentio/kafka-go"
)

const (
	queueName  = "MappingQueue"
	kafkaTopic = "jobs-topic"
)

func main() {
	rabbitMQURL := getRequiredEnv("RABBITMQ_URL")
	kafkaURL := getRequiredEnv("KAFKA_URL")

	conn := connectToRabbitMQ(rabbitMQURL, 10, 3*time.Second)
	defer conn.Close()

	ch, err := conn.Channel()
	if err != nil {
		log.Fatalf("Failed to open a channel: %v", err)
	}
	defer ch.Close()

	msgs, err := ch.Consume(
		queueName,
		"ComulatorConsumer",
		false, // auto-ack
		false, // exclusive
		false, // no-local
		false, // no-wait
		nil,
	)
	if err != nil {
		log.Fatalf("Failed to register consumer for %s: %v", queueName, err)
	}

	writer := kafka.NewWriter(kafka.WriterConfig{
		Brokers:      []string{kafkaURL},
		Topic:        kafkaTopic,
		Balancer:     &kafka.LeastBytes{},
		RequiredAcks: int(kafka.RequireOne),
	})
	defer writer.Close()

	stopChan := make(chan os.Signal, 1)
	signal.Notify(stopChan, syscall.SIGINT, syscall.SIGTERM)

	go func() {
		for d := range msgs {
			handleDelivery(context.Background(), writer, d)
		}
	}()

	<-stopChan
	log.Println("Shutting down consumer...")
}

func handleDelivery(ctx context.Context, writer *kafka.Writer, d amqp.Delivery) {
	var jobs []model.JobDto
	if err := json.Unmarshal(d.Body, &jobs); err != nil {
		log.Printf("Error decoding JSON: %v", err)
		d.Nack(false, false)
		return
	}

	messages := make([]kafka.Message, 0, len(jobs))
	for _, job := range jobs {
		payload, err := json.Marshal(job)
		if err != nil {
			log.Printf("Error serializing job %s: %v", job.ExternalId, err)
			d.Nack(false, false)
			return
		}

		messages = append(messages, kafka.Message{
			Key:   []byte(job.ExternalId),
			Value: payload,
		})
	}

	if err := writer.WriteMessages(ctx, messages...); err != nil {
		log.Printf("Failed to write to Kafka topic %s: %v", kafkaTopic, err)
		d.Nack(false, true)
		return
	}

	if err := d.Ack(false); err != nil {
		log.Printf("Error acknowledging message: %v", err)
	}
}

func getRequiredEnv(key string) string {
	value, exists := os.LookupEnv(key)
	if !exists || strings.TrimSpace(value) == "" {
		log.Fatalf("Missing or empty required environment variable %s. Check your .env configuration.", key)
	}

	return value
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
