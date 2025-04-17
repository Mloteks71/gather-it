package model

import (
	"time"
)

// Config holds the configuration for the processor
type Config struct {
	SourceURL    string            // URL to make requests to
	TargetURL    string            // URL to send processed data to
	Headers      map[string]string // Headers to include in requests
	RequestDelay time.Duration     // Delay between requests
	MaxRetries   int               // Maximum number of retries for failed requests
	RetryDelay   time.Duration     // Delay between retries
}

// DescriptionInputItem represents the input struct with ID and Slug
type DescriptionInputItem struct {
	Id   int    `json:"id"`
	Slug string `json:"slug"`
}

type DescriptionInput struct {
	Items []DescriptionInputItem `json:"Items"` // Assuming your message contains an "Items" array
}

// DescriptionOutputItem represents the processed output to send
type DescriptionOutputItem struct {
	Id             int    `json:"id"`
	CutDescription string `json:"cutDescription"`
}
