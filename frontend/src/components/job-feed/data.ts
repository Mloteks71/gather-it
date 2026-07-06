import {
  type JobAdListItem,
  ExperienceLevel,
  experienceLevelLabels,
  formatRelativeTime,
  jobSiteLabels,
  OfferStatus,
  workplaceTypeLabels,
  type WorkplaceType,
} from "@/lib/api"

export type WorkType = (typeof workplaceTypeLabels)[WorkplaceType]

/** The flattened, display-ready shape a job card renders. */
export interface Job {
  id: number
  company: string
  initial: string
  logoColor: string
  logoBg: string
  title: string
  category: string
  city: string
  salary: string | null
  seniority: string | null
  workTypes: WorkType[]
  skills: string[]
  source: string
  posted: string
  isNew: boolean
}

/** Deterministic logo colours so the same company always looks the same. */
const logoPalette: { color: string; bg: string }[] = [
  { color: "#a78bfa", bg: "rgba(167,139,250,0.13)" },
  { color: "#fbbf24", bg: "rgba(251,191,36,0.13)" },
  { color: "#38bdf8", bg: "rgba(56,189,248,0.13)" },
  { color: "#fb7185", bg: "rgba(251,113,133,0.13)" },
  { color: "#34d399", bg: "rgba(52,211,153,0.13)" },
  { color: "#2dd4bf", bg: "rgba(45,212,191,0.13)" },
  { color: "#c084fc", bg: "rgba(192,132,252,0.13)" },
]

// NOTE: placeholder — the API doesn't return skills on list items (only the
// details endpoint does) and can't filter by skill. Shared by the sidebar
// skills group and the job cards until it can.
export const placeholderSkillOptions = [
  "React",
  "TypeScript",
  "Python",
  "Java",
  "Docker",
  "Kubernetes",
  "SQL",
  "AWS",
]

const placeholderCardSkills = placeholderSkillOptions.slice(0, 4)

function hashString(value: string): number {
  let hash = 0
  for (let i = 0; i < value.length; i++) {
    hash = (hash << 5) - hash + value.charCodeAt(i)
    hash |= 0
  }
  return Math.abs(hash)
}

// Ordered by how "senior" a level reads, so we can surface the strongest one.
const experiencePriority: ExperienceLevel[] = [
  ExperienceLevel.Senior,
  ExperienceLevel.Mid,
  ExperienceLevel.Junior,
  ExperienceLevel.Any,
]

function pickSeniority(levels: ExperienceLevel[]): string | null {
  for (const level of experiencePriority) {
    if (levels.includes(level)) return experienceLevelLabels[level]
  }
  return null
}

/** Converts a raw API list item into the display model used by the cards. */
export function toDisplayJob(dto: JobAdListItem): Job {
  const company = dto.companyName?.trim() || "Unknown company"
  const palette = logoPalette[hashString(company) % logoPalette.length]
  const workTypes = dto.workplaceType.map((type) => workplaceTypeLabels[type])

  return {
    id: dto.jobAdId,
    company,
    initial: company.charAt(0).toUpperCase(),
    logoColor: palette.color,
    logoBg: palette.bg,
    title: dto.title,
    // NOTE: placeholders — the list endpoint doesn't return a job category
    // or city yet.
    category: "General",
    city: "Poland",
    // NOTE: the list endpoint doesn't return salary — the card reserves the
    // slot so the layout holds once the API provides it.
    salary: null,
    seniority: pickSeniority(dto.experienceLevel),
    workTypes,
    skills: placeholderCardSkills,
    source: jobSiteLabels[dto.jobSite],
    posted: formatRelativeTime(dto.publishedAt),
    isNew: dto.offerStatus === OfferStatus.NewlyAdded,
  }
}
