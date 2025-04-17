package justJoinIt

import (
	"bytes"
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"net/http"
	"regexp"
	"strings"
	"time"

	model "gather.it/DescriptionService/internal/sharedModel"
)

// Processor handles the processing of requests
type Processor struct {
	config model.Config
	client *http.Client
	output []model.DescriptionOutputItem // Stores processed items for batch sending
}

// NewProcessor creates a new Processor instance
func NewProcessor(config model.Config) *Processor {
	return &Processor{
		config: config,
		client: &http.Client{
			Timeout: 30 * time.Second,
		},
		output: make([]model.DescriptionOutputItem, 0),
	}
}

// ProcessItems processes a list of input items and stores results for batch sending
func (p *Processor) ProcessItems(items []model.DescriptionInputItem) error {
	for _, item := range items {
		// Make the request to the source URL
		respBody, err := p.makeRequest(item)
		if err != nil {
			return fmt.Errorf("failed to make request for item %d: %v", item.Id, err)
		}

		// Process the response
		cutDescription, err := p.processResponse(respBody)
		if err != nil {
			return fmt.Errorf("failed to process response for item %d: %v", item.Id, err)
		}

		// Store the processed item
		p.output = append(p.output, model.DescriptionOutputItem{
			Id:             item.Id,
			CutDescription: cutDescription,
		})

		// Delay between requests if needed
		if p.config.RequestDelay > 0 {
			time.Sleep(p.config.RequestDelay)
		}
	}

	// Send all processed items in a single batch
	if len(p.output) > 0 {
		if err := p.sendBatch(); err != nil {
			return fmt.Errorf("failed to send batch: %v", err)
		}
	}

	return nil
}

// makeRequest makes an HTTP request to the source URL
func (p *Processor) makeRequest(item model.DescriptionInputItem) (string, error) {
	req, err := http.NewRequest("GET", p.config.SourceURL, nil)
	if err != nil {
		return "", err
	}

	// Add headers
	for key, value := range p.config.Headers {
		req.Header.Add(key, value)
	}

	// Add query parameters if needed
	q := req.URL.Query()
	q.Add("slug", item.Slug)
	req.URL.RawQuery = q.Encode()

	var respBody string
	var lastErr error

	// Retry logic
	for attempt := 0; attempt <= p.config.MaxRetries; attempt++ {
		if attempt > 0 {
			time.Sleep(p.config.RetryDelay)
		}

		resp, err := p.client.Do(req)
		if err != nil {
			lastErr = err
			continue
		}

		defer resp.Body.Close()

		if resp.StatusCode != http.StatusOK {
			lastErr = fmt.Errorf("unexpected status code: %d", resp.StatusCode)
			continue
		}

		bodyBytes, err := io.ReadAll(resp.Body)
		if err != nil {
			lastErr = err
			continue
		}

		respBody = string(bodyBytes)
		lastErr = nil
		break
	}

	if lastErr != nil {
		return "", lastErr
	}

	return respBody, nil
}

// processResponse processes the response body and extracts the required substring
func (p *Processor) processResponse(body string) (string, error) {
	// Find the position of "OfferDetailsShell"
	startMarker := "OfferDetailsShell"
	startPos := strings.Index(body, startMarker)
	if startPos == -1 {
		return "", errors.New("OfferDetailsShell not found in response")
	}

	// Calculate the start position (7 characters after the end of OfferDetailsShell)
	startPos += len(startMarker) + 7
	if startPos >= len(body) {
		return "", errors.New("start position exceeds response length")
	}

	// Find the position of the first "label": after the start position
	endMarker := `"label":`
	endPos := strings.Index(body[startPos:], endMarker)
	if endPos == -1 {
		return "", errors.New("label marker not found after OfferDetailsShell")
	}

	// Extract the substring
	cutDescription := body[startPos : startPos+endPos]

	// Clean HTML tags and handle <br> tags
	cleaned := p.cleanHTML(cutDescription)

	return strings.TrimSpace(cleaned), nil
}

// cleanHTML removes HTML tags and converts <br> tags to newlines
func (p *Processor) cleanHTML(input string) string {
	// First replace <br> tags with newlines
	reBr := regexp.MustCompile(`(?i)<br\s*/?>`)
	withNewlines := reBr.ReplaceAllString(input, "\n")

	// Then remove all other HTML tags
	reTags := regexp.MustCompile(`(?i)<[^>]*>`)
	cleaned := reTags.ReplaceAllString(withNewlines, "")

	// Replace multiple spaces/newlines with single ones
	reSpaces := regexp.MustCompile(`\s+`)
	cleaned = reSpaces.ReplaceAllString(cleaned, " ")

	// Trim leading/trailing whitespace and newlines
	cleaned = strings.TrimSpace(cleaned)

	return cleaned
}

// sendBatch sends all processed items in a single request
func (p *Processor) sendBatch() error {
	if len(p.output) == 0 {
		return nil
	}

	// Prepare the request body
	requestBody, err := json.Marshal(p.output)
	if err != nil {
		return err
	}

	req, err := http.NewRequest("POST", p.config.TargetURL, bytes.NewBuffer(requestBody))
	if err != nil {
		return err
	}

	// Add headers
	for key, value := range p.config.Headers {
		req.Header.Add(key, value)
	}
	req.Header.Set("Content-Type", "application/json")

	var lastErr error

	// Retry logic
	for attempt := 0; attempt <= p.config.MaxRetries; attempt++ {
		if attempt > 0 {
			time.Sleep(p.config.RetryDelay)
		}

		resp, err := p.client.Do(req)
		if err != nil {
			lastErr = err
			continue
		}

		defer resp.Body.Close()

		if resp.StatusCode != http.StatusOK {
			lastErr = fmt.Errorf("unexpected status code: %d", resp.StatusCode)
			continue
		}

		lastErr = nil
		break
	}

	return lastErr
}
