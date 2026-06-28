export type WorkType = "Remote" | "Hybrid" | "On-site"

export type Layout = "classic" | "compact" | "detailed"

export interface Job {
  id: number
  company: string
  initial: string
  logoColor: string
  logoBg: string
  title: string
  seniority: string
  category: string
  workType: WorkType
  location: string
  salary: string
  posted: string
  source: string
  sourceColor: string
  size: string
  industry: string
  skills: string[]
}

/** Maps a work type to the small status-dot colour used across the cards. */
export const workTypeColor: Record<WorkType, string> = {
  Remote: "#34d399",
  Hybrid: "#fbbf24",
  "On-site": "#8a909b",
}

export const jobs: Job[] = [
  {
    id: 1,
    company: "Parallel",
    initial: "P",
    logoColor: "#a78bfa",
    logoBg: "rgba(167,139,250,0.13)",
    title: "Senior Frontend Engineer",
    seniority: "Senior",
    category: "Engineering",
    workType: "Remote",
    location: "Europe",
    salary: "€70k–95k",
    posted: "2h ago",
    source: "LinkedIn",
    sourceColor: "#5b8cff",
    size: "50–200",
    industry: "Fintech",
    skills: ["React", "TypeScript", "GraphQL", "Vite"],
  },
  {
    id: 2,
    company: "Lumen Systems",
    initial: "L",
    logoColor: "#fbbf24",
    logoBg: "rgba(251,191,36,0.13)",
    title: "Product Manager, Platform",
    seniority: "Senior",
    category: "Product",
    workType: "Hybrid",
    location: "Berlin",
    salary: "€85k–110k",
    posted: "1d ago",
    source: "Wellfound",
    sourceColor: "#d4d4d8",
    size: "200–500",
    industry: "Enterprise SaaS",
    skills: ["Roadmapping", "SQL", "B2B SaaS", "Discovery"],
  },
  {
    id: 3,
    company: "Nimbus Cloud",
    initial: "N",
    logoColor: "#38bdf8",
    logoBg: "rgba(56,189,248,0.13)",
    title: "DevOps Engineer",
    seniority: "Mid",
    category: "DevOps",
    workType: "Remote",
    location: "Global",
    salary: "$120k–150k",
    posted: "3h ago",
    source: "Indeed",
    sourceColor: "#6ea8fe",
    size: "500–1k",
    industry: "Cloud Infra",
    skills: ["Kubernetes", "Terraform", "AWS", "Go"],
  },
  {
    id: 4,
    company: "Corewave",
    initial: "C",
    logoColor: "#fb7185",
    logoBg: "rgba(251,113,133,0.13)",
    title: "QA Automation Engineer",
    seniority: "Mid",
    category: "QA",
    workType: "On-site",
    location: "Kraków",
    salary: "18–24k PLN/mo",
    posted: "5d ago",
    source: "Glassdoor",
    sourceColor: "#34d399",
    size: "50–200",
    industry: "E-commerce",
    skills: ["Playwright", "Python", "CI/CD", "REST"],
  },
  {
    id: 5,
    company: "Helix Security",
    initial: "H",
    logoColor: "#34d399",
    logoBg: "rgba(52,211,153,0.13)",
    title: "Cybersecurity Analyst",
    seniority: "Mid",
    category: "Security",
    workType: "Hybrid",
    location: "Warsaw",
    salary: "€60k–80k",
    posted: "6h ago",
    source: "LinkedIn",
    sourceColor: "#5b8cff",
    size: "200–500",
    industry: "Cybersecurity",
    skills: ["SIEM", "Threat Intel", "SOC", "Splunk"],
  },
  {
    id: 6,
    company: "Quanta Health",
    initial: "Q",
    logoColor: "#2dd4bf",
    logoBg: "rgba(45,212,191,0.13)",
    title: "Data Engineer",
    seniority: "Senior",
    category: "Data",
    workType: "Remote",
    location: "Global",
    salary: "$130k–165k",
    posted: "4h ago",
    source: "Wellfound",
    sourceColor: "#d4d4d8",
    size: "200–500",
    industry: "HealthTech",
    skills: ["Spark", "Airflow", "dbt", "Snowflake"],
  },
]

export interface FilterOption {
  label: string
  count: number
  active: boolean
}

export interface FilterFacet {
  label: string
  options: FilterOption[]
}

export const filterFacets: FilterFacet[] = [
  {
    label: "Role",
    options: [
      { label: "Engineering", count: 2, active: true },
      { label: "Product", count: 1, active: false },
      { label: "Design", count: 1, active: false },
      { label: "DevOps", count: 2, active: false },
      { label: "Security", count: 1, active: false },
      { label: "Data", count: 1, active: false },
      { label: "QA", count: 1, active: false },
    ],
  },
  {
    label: "Work type",
    options: [
      { label: "Remote", count: 3, active: true },
      { label: "Hybrid", count: 2, active: false },
      { label: "On-site", count: 1, active: false },
    ],
  },
  {
    label: "Seniority",
    options: [
      { label: "Mid", count: 3, active: false },
      { label: "Senior", count: 3, active: false },
      { label: "Lead", count: 0, active: false },
    ],
  },
  {
    label: "Skills",
    options: [
      { label: "React", count: 1, active: false },
      { label: "TypeScript", count: 1, active: false },
      { label: "Go", count: 2, active: false },
      { label: "AWS", count: 1, active: false },
      { label: "Python", count: 1, active: false },
    ],
  },
]

export const activeTokens = [
  { facet: "role", value: "Engineering" },
  { facet: "type", value: "Remote" },
]
