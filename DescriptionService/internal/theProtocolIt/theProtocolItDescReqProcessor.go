package theProtocolIt

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io"
	"log"
	"net/http"
	"strings"
	"time"

	"golang.org/x/net/html"

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
		requirements, project, benefits, workstyle, err := p.processResponse(respBody, item.Slug)
		// if err != nil {
		// 	return fmt.Errorf("failed to process response for item %d: %v", item.Id, err)
		// }
		p.output = append(p.output, model.DescriptionOutputItem{
			Id:           item.Id,
			Requirements: requirements,
			AboutProject: project,
			Benefits:     benefits,
			Workstyle:    workstyle,
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
			log.Printf("TheProtocol error: %d", resp.StatusCode)
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

func (p *Processor) processResponse(body string, slug string) (string, string, string, string, error) {
	parsedHtml, err := html.Parse(strings.NewReader(body))

	requirementsNode := FindElementByID(parsedHtml, "REQUIREMENTS")
	requirementsPart := GetTextContent(requirementsNode)

	projectNode := FindElementByID(parsedHtml, "PROJECT")
	projectPart := GetTextContent(projectNode)

	benefitsNode := FindElementByID(parsedHtml, "PROGRESS_AND_BENEFITS")
	benefitsPart := GetTextContent(benefitsNode)

	workstyleNode := FindElementByID(parsedHtml, "WORKSTYLE")
	workstylePart := GetTextContent(workstyleNode)

	return requirementsPart, benefitsPart, workstylePart, projectPart, err
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
			lastErr = fmt.Errorf("ComulatorAPI unexpected status code: %d", resp.StatusCode)
			continue
		}

		lastErr = nil
		break
	}

	return lastErr
}

func FindElementByID(n *html.Node, id string) *html.Node {
	if n.Type == html.ElementNode {
		for _, attr := range n.Attr {
			if attr.Key == "id" && attr.Val == id {
				return n
			}
		}
	}

	// Recursively search child nodes
	for c := n.FirstChild; c != nil; c = c.NextSibling {
		if result := FindElementByID(c, id); result != nil {
			return result
		}
	}

	return nil
}

func GetTextContent(n *html.Node) string {
	if n == nil {
		return ""
	}

	var buf strings.Builder
	var f func(*html.Node)
	f = func(n *html.Node) {
		if n.Type == html.TextNode {
			buf.WriteString(n.Data)
		}
		for c := n.FirstChild; c != nil; c = c.NextSibling {
			f(c)
		}
	}
	f(n)
	return buf.String()
}
