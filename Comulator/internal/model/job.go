package model

import "time"

type SalaryRangeDto struct {
	From         *float64 `json:"From"`
	To           *float64 `json:"To"`
	Currency     string   `json:"Currency"`
	ContractType *string  `json:"ContractType"`
}

type JobDto struct {
	ExternalId       string           `json:"ExternalId"`
	Slug             string           `json:"Slug"`
	Title            string           `json:"Title"`
	CompanyName      string           `json:"CompanyName"`
	SourceSite       int              `json:"SourceSite"`
	Skills           []string         `json:"Skills"`
	WorkplaceTypes   []string         `json:"WorkplaceTypes"`
	ExperienceLevels []string         `json:"ExperienceLevels"`
	Locations        []string         `json:"Locations"`
	Salaries         []SalaryRangeDto `json:"Salaries"`
	PublishedAt      *time.Time       `json:"PublishedAt"`
	LogoUrl          *string          `json:"LogoUrl"`
}
