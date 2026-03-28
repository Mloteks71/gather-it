package main

import (
	"context"
	"encoding/json"
	"log"
	"os"
	"os/signal"
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
	rabbitMQURL := os.Getenv("RABBITMQ_URL")
	if rabbitMQURL == "" {
		rabbitMQURL = "amqp://rabbituser:rabbitpass@rabbitmq:5672/"
	}

	kafkaURL := os.Getenv("KAFKA_URL")
	if kafkaURL == "" {
		kafkaURL = "kafka:9092"
	}

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

	log.Printf("[*] Waiting for messages on %s. To exit press CTRL+C", queueName)

	go func() {
		for d := range msgs {
			var jobs []model.JobDto
			if err := json.Unmarshal(d.Body, &jobs); err != nil {
				log.Printf("Error decoding JSON: %v", err)
				d.Nack(false, false)
				continue
			}

			messages := make([]kafka.Message, 0, len(jobs))
			for _, job := range jobs {
				payload, err := json.Marshal(job)
				if err != nil {
					log.Printf("Error serializing job %s: %v", job.Id, err)
					d.Nack(false, false)
					goto next
				}
				messages = append(messages, kafka.Message{
					Key:   []byte(job.Id),
					Value: payload,
				})
			}

			if err := writer.WriteMessages(context.Background(), messages...); err != nil {
				log.Printf("Failed to write to Kafka topic %s: %v", kafkaTopic, err)
				d.Nack(false, true)
				continue
			}

			log.Printf("Forwarded %d jobs individually to Kafka topic %s", len(jobs), kafkaTopic)

			if err := d.Ack(false); err != nil {
				log.Printf("Error acknowledging message: %v", err)
			}
		next:
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
