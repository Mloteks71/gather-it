/**
 * Client for the JobReadApi service (`GET /api/job-ads`, `GET /api/job-ads/{id}`).
 *
 * The backend serialises enums as their integer values and property names as
 * camelCase (ASP.NET Core defaults), so the shapes below mirror that exactly.
 * Enums are modelled as `const` objects rather than TS `enum`s because the
 * project's tsconfig enables `erasableSyntaxOnly`.
 */

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? "/api"

/* ── Enums (integer values match the C# `JobReadApi.Application.Enums`) ──── */

export const OfferStatus = {
  NewlyAdded: 0,
  Active: 1,
  Inactive: 2,
} as const
export type OfferStatus = (typeof OfferStatus)[keyof typeof OfferStatus]

export const WorkplaceType = {
  Remote: 0,
  Hybrid: 1,
  OnSite: 2,
} as const
export type WorkplaceType = (typeof WorkplaceType)[keyof typeof WorkplaceType]

export const ExperienceLevel = {
  Undefined: 0,
  Junior: 1,
  Mid: 2,
  Senior: 3,
  Any: 4,
} as const
export type ExperienceLevel = (typeof ExperienceLevel)[keyof typeof ExperienceLevel]

export const JobSite = {
  JustJoinIt: 0,
  TheProtocolIt: 1,
  SolidJobs: 2,
  PracujPl: 3,
} as const
export type JobSite = (typeof JobSite)[keyof typeof JobSite]

export const ContractType = {
  Undefined: 0,
  Uop: 1,
  B2b: 2,
  Uz: 3,
  Any: 4,
  Internship: 5,
  Contract: 6,
  Replacement: 7,
} as const
export type ContractType = (typeof ContractType)[keyof typeof ContractType]

/* ── Human-readable labels ──────────────────────────────────────────────── */

export const offerStatusLabels: Record<OfferStatus, string> = {
  [OfferStatus.NewlyAdded]: "New",
  [OfferStatus.Active]: "Active",
  [OfferStatus.Inactive]: "Inactive",
}

export const workplaceTypeLabels = {
  [WorkplaceType.Remote]: "Remote",
  [WorkplaceType.Hybrid]: "Hybrid",
  [WorkplaceType.OnSite]: "On-site",
} as const satisfies Record<WorkplaceType, string>

export const experienceLevelLabels: Record<ExperienceLevel, string> = {
  [ExperienceLevel.Undefined]: "Unspecified",
  [ExperienceLevel.Junior]: "Junior",
  [ExperienceLevel.Mid]: "Mid",
  [ExperienceLevel.Senior]: "Senior",
  [ExperienceLevel.Any]: "Any level",
}

export const jobSiteLabels: Record<JobSite, string> = {
  [JobSite.JustJoinIt]: "JustJoin.it",
  [JobSite.TheProtocolIt]: "TheProtocol.it",
  [JobSite.SolidJobs]: "SolidJobs",
  [JobSite.PracujPl]: "Pracuj.pl",
}

export const contractTypeLabels: Record<ContractType, string> = {
  [ContractType.Undefined]: "Contract",
  [ContractType.Uop]: "Employment",
  [ContractType.B2b]: "B2B",
  [ContractType.Uz]: "Mandate",
  [ContractType.Any]: "Any",
  [ContractType.Internship]: "Internship",
  [ContractType.Contract]: "Contract",
  [ContractType.Replacement]: "Replacement",
}

/* ── DTOs (mirror `JobReadApi.Application.Dtos`) ────────────────────────── */

export interface JobAdListItem {
  jobAdId: number
  externalId: string
  title: string
  offerStatus: OfferStatus
  workplaceType: WorkplaceType[]
  experienceLevel: ExperienceLevel[]
  companyName: string | null
  jobSite: JobSite
  slug: string
  expiredAt: string | null
  publishedAt: string | null
}

export interface SalaryDto {
  salaryId: number
  contractType: ContractType
  salaryMin: number | null
  salaryMax: number | null
}

export interface DescriptionDto {
  descriptionId: number
  descriptionText: string | null
  requirements: string | null
  benefits: string | null
  workstyle: string | null
  aboutProject: string | null
}

export interface JobAdDetails extends JobAdListItem {
  companyId: number | null
  skills: string[]
  salaries: SalaryDto[]
  descriptions: DescriptionDto[]
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
}

/* ── Requests ───────────────────────────────────────────────────────────── */

export interface GetJobAdsParams {
  page?: number
  pageSize?: number
  status?: OfferStatus
  site?: JobSite
  signal?: AbortSignal
}

async function request<T>(path: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: { Accept: "application/json" },
    signal,
  })

  if (!response.ok) {
    throw new Error(`Request to ${path} failed with status ${response.status}`)
  }

  return (await response.json()) as T
}

export function getJobAds({
  page = 1,
  pageSize = 20,
  status,
  site,
  signal,
}: GetJobAdsParams = {}): Promise<PagedResult<JobAdListItem>> {
  const params = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  })
  if (status !== undefined) params.set("status", String(status))
  if (site !== undefined) params.set("site", String(site))

  return request<PagedResult<JobAdListItem>>(`/job-ads?${params.toString()}`, signal)
}

export function getJobAd(
  jobAdId: number,
  signal?: AbortSignal
): Promise<JobAdDetails> {
  return request<JobAdDetails>(`/job-ads/${jobAdId}`, signal)
}

/* ── Formatting helpers ─────────────────────────────────────────────────── */

const relativeTime = new Intl.RelativeTimeFormat("en", { numeric: "auto" })
const salaryFormatter = new Intl.NumberFormat("pl-PL", { maximumFractionDigits: 0 })

const relativeUnits: [Intl.RelativeTimeFormatUnit, number][] = [
  ["year", 60 * 60 * 24 * 365],
  ["month", 60 * 60 * 24 * 30],
  ["day", 60 * 60 * 24],
  ["hour", 60 * 60],
  ["minute", 60],
]

/** Renders an ISO timestamp as a compact "3h ago" / "2d ago" label. */
export function formatRelativeTime(iso: string | null): string {
  if (!iso) return "—"

  const then = new Date(iso).getTime()
  if (Number.isNaN(then)) return "—"

  const diffSeconds = Math.round((then - Date.now()) / 1000)
  const abs = Math.abs(diffSeconds)

  for (const [unit, secondsInUnit] of relativeUnits) {
    if (abs >= secondsInUnit) {
      return relativeTime.format(Math.round(diffSeconds / secondsInUnit), unit)
    }
  }
  return relativeTime.format(diffSeconds, "second")
}

/** Formats a salary range like "15 000 – 20 000 PLN · B2B". */
export function formatSalary(salary: SalaryDto): string {
  const { salaryMin, salaryMax } = salary
  const contract = contractTypeLabels[salary.contractType]

  let amount: string | null = null
  if (salaryMin != null && salaryMax != null) {
    amount = `${salaryFormatter.format(salaryMin)} – ${salaryFormatter.format(salaryMax)} PLN`
  } else if (salaryMin != null) {
    amount = `from ${salaryFormatter.format(salaryMin)} PLN`
  } else if (salaryMax != null) {
    amount = `up to ${salaryFormatter.format(salaryMax)} PLN`
  }

  if (!amount) return contract
  return `${amount} · ${contract}`
}
