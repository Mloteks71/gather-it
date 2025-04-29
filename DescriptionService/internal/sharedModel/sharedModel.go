package model

import (
	"time"
)

type Config struct {
	SourceURL    string
	TargetURL    string
	Headers      map[string]string
	RequestDelay time.Duration
	MaxRetries   int
	RetryDelay   time.Duration
}

type DescriptionInputItem struct {
	Id   int    `json:"id"`
	Slug string `json:"slug"`
}

type DescriptionInput []DescriptionInputItem

type DescriptionOutputItem struct {
	Id             int    `json:"Id"`
	CutDescription string `json:"Description"`
}
