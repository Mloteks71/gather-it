package justJoinIt

import (
	"bytes"
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"log"
	"net/http"
	"regexp"
	"strings"
	"time"

	model "gather.it/DescriptionService/internal/sharedModel"
)

type Processor struct {
	config model.Config
	client *http.Client
	output []model.DescriptionOutputItem
}

func NewProcessor(config model.Config) *Processor {
	return &Processor{
		config: config,
		client: &http.Client{
			Timeout: 30 * time.Second,
		},
		output: make([]model.DescriptionOutputItem, 0),
	}
}

func (p *Processor) ProcessItems(items []model.DescriptionInputItem) error {
	for _, item := range items {
		respBody, err := p.makeRequest(item)
		if err != nil {
			return fmt.Errorf("failed to make request for item %d: %v", item.Id, err)
		}
		cutDescription, err := p.processResponse(respBody, item.Slug)
		if err != nil {
			return fmt.Errorf("failed to process response for item %d: %v", item.Id, err)
		}
		p.output = append(p.output, model.DescriptionOutputItem{
			Id:             item.Id,
			CutDescription: cutDescription,
		})
		if p.config.RequestDelay > 0 {
			time.Sleep(p.config.RequestDelay)
		}
	}

	if len(p.output) > 0 {
		if err := p.sendBatch(); err != nil {
			return fmt.Errorf("failed to send batch: %v", err)
		}
	}

	return nil
}

func (p *Processor) makeRequest(item model.DescriptionInputItem) (string, error) {
	req, err := http.NewRequest("GET", p.config.SourceURL+item.Slug, nil)
	if err != nil {
		log.Printf("error: %s", err)
		return "", err
	}

	for key, value := range p.config.Headers {
		req.Header.Add(key, value)
	}

	q := req.URL.Query()
	req.URL.RawQuery = q.Encode()

	var respBody string
	var lastErr error

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
			log.Printf("error: %s", resp.StatusCode)
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

func (p *Processor) processResponse(body string, slug string) (string, error) {
	startMarker := "OfferDetailsShell"
	startPos := strings.Index(body, startMarker)
	if startPos == -1 {
		return "", errors.New("OfferDetailsShell not found in response")
	}

	startPos += len(startMarker)
	if startPos >= len(body) {
		return "", errors.New("start position exceeds response length")
	}

	trueStartPos := strings.Index(body[startPos:], "<")
	trueStartPos += startPos

	endMarker := `"label":`
	endPos := strings.Index(body[trueStartPos:], endMarker)
	if endPos == -1 {
		log.Printf("label marker not found after OfferDetailsShell for %s", slug)
		return "", errors.New("label marker not found after OfferDetailsShell")
	}

	cutDescription := body[trueStartPos : trueStartPos+endPos-4]

	cleaned := p.cleanHTML(cutDescription)

	return strings.TrimSpace(cleaned), nil
}

func (p *Processor) cleanHTML(input string) string {
	reBr := regexp.MustCompile(`(?i)<br\s*/?>`)
	withNewlines := reBr.ReplaceAllString(input, "\n")

	reTags := regexp.MustCompile(`(?i)<[^>]*>`)
	cleaned := reTags.ReplaceAllString(withNewlines, "")

	reSpaces := regexp.MustCompile(`\s+`)
	cleaned = reSpaces.ReplaceAllString(cleaned, " ")

	cleaned = strings.TrimSpace(cleaned)

	return cleaned
}

func (p *Processor) sendBatch() error {
	if len(p.output) == 0 {
		return nil
	}

	requestBody, err := json.Marshal(p.output)
	if err != nil {
		return err
	}

	req, err := http.NewRequest("POST", p.config.TargetURL, bytes.NewBuffer(requestBody))
	if err != nil {
		log.Printf("Error: %s", err)
		return err
	}

	req.Header.Set("Content-Type", "application/json")

	var lastErr error

	for attempt := 0; attempt <= p.config.MaxRetries; attempt++ {
		if attempt > 0 {
			time.Sleep(p.config.RetryDelay)
		}

		req, err := http.NewRequest("POST", p.config.TargetURL, bytes.NewBuffer(requestBody))
		if err != nil {
			log.Printf("Error creating request: %s", err)
			lastErr = err
			continue
		}
		req.Header.Set("Content-Type", "application/json")

		resp, err := p.client.Do(req)
		if err != nil {
			lastErr = err
			continue
		}
		defer resp.Body.Close()

		if resp.StatusCode != http.StatusCreated {
			lastErr = fmt.Errorf("unexpected status code: %d", resp.StatusCode)
			continue
		}

		lastErr = nil
		break
	}

	return lastErr
}
