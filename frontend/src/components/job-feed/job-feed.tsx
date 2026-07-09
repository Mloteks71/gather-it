import { useEffect, useMemo, useState } from "react"

import { Button } from "@/components/ui/button"
import {
  getJobAds,
  jobSiteLabels,
  offerStatusLabels,
  type JobSite,
  type OfferStatus,
  type PagedResult,
  type JobAdListItem,
} from "@/lib/api"
import { ActiveFilters, type FilterToken } from "./active-filters"
import { toDisplayJob } from "./data"
import { FilterSidebar } from "./filter-sidebar"
import { JobCardClassic } from "./job-card"
import { ResultsToolbar } from "./results-toolbar"
import { SiteHeader } from "./site-header"

const PAGE_SIZE = 20

// NOTE: the design has no pagination (it renders a fixed client-side list);
// ours stays because the API is paged.
function Pagination({
  page,
  totalPages,
  onPageChange,
}: {
  page: number
  totalPages: number
  onPageChange: (page: number) => void
}) {
  if (totalPages <= 1) return null

  return (
    <div className="mt-8 flex items-center justify-center gap-4">
      <Button
        variant="outline"
        size="lg"
        disabled={page <= 1}
        onClick={() => onPageChange(page - 1)}
      >
        Previous
      </Button>
      <span className="font-mono text-[13px] text-muted-foreground">
        Page <span className="text-foreground">{page}</span> of {totalPages}
      </span>
      <Button
        variant="outline"
        size="lg"
        disabled={page >= totalPages}
        onClick={() => onPageChange(page + 1)}
      >
        Next
      </Button>
    </div>
  )
}

function JobFeed() {
  const [page, setPage] = useState(1)
  const [status, setStatus] = useState<OfferStatus | undefined>(undefined)
  const [site, setSite] = useState<JobSite | undefined>(undefined)
  // NOTE: selected skills surface as filter tokens but aren't sent to the
  // API — it can't filter by skill yet.
  const [skills, setSkills] = useState<string[]>([])

  const [data, setData] = useState<PagedResult<JobAdListItem> | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(false)
  const [reloadKey, setReloadKey] = useState(0)

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setError(false)

    getJobAds({ page, pageSize: PAGE_SIZE, status, site, signal: controller.signal })
      .then((result) => setData(result))
      .catch((err) => {
        if (err.name !== "AbortError") setError(true)
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [page, status, site, reloadKey])

  const jobs = useMemo(() => data?.items.map(toDisplayJob) ?? [], [data])
  const totalCount = data?.totalCount ?? 0
  const totalPages = Math.max(1, Math.ceil(totalCount / (data?.pageSize ?? PAGE_SIZE)))

  const changeStatus = (value?: OfferStatus) => {
    setStatus(value)
    setPage(1)
  }
  const changeSite = (value?: JobSite) => {
    setSite(value)
    setPage(1)
  }
  const toggleSkill = (skill: string, checked: boolean) => {
    setSkills((current) =>
      checked ? [...current, skill] : current.filter((s) => s !== skill)
    )
  }
  const clearFilters = () => {
    changeStatus(undefined)
    changeSite(undefined)
    setSkills([])
  }

  const tokens: FilterToken[] = []
  if (status !== undefined) {
    tokens.push({
      facet: "status",
      value: offerStatusLabels[status],
      onRemove: () => changeStatus(undefined),
    })
  }
  if (site !== undefined) {
    tokens.push({
      facet: "source",
      value: jobSiteLabels[site],
      onRemove: () => changeSite(undefined),
    })
  }
  for (const skill of skills) {
    tokens.push({
      facet: "skill",
      value: skill,
      onRemove: () => toggleSkill(skill, false),
    })
  }

  return (
    <div className="min-h-svh bg-background text-foreground">
      <SiteHeader />

      <div className="mx-auto grid max-w-[1080px] items-start gap-12 px-7 pt-[38px] lg:grid-cols-[224px_1fr]">
        <FilterSidebar
          status={status}
          site={site}
          skills={skills}
          onStatusChange={changeStatus}
          onSiteChange={changeSite}
          onSkillToggle={toggleSkill}
        />

        <main className="min-w-0 pb-[90px]">
          <ResultsToolbar count={jobs.length} total={totalCount} />

          <ActiveFilters tokens={tokens} onClearAll={clearFilters} />

          {loading && (
            <div className="py-16 text-center text-[14px] text-muted-foreground">
              Loading roles…
            </div>
          )}

          {!loading && error && (
            <div className="flex flex-col items-center gap-3 py-16 text-center">
              <p className="text-[14px] text-destructive">
                Couldn’t reach the job service.
              </p>
              <Button
                variant="outline"
                size="lg"
                onClick={() => setReloadKey((key) => key + 1)}
              >
                Retry
              </Button>
            </div>
          )}

          {!loading && !error && jobs.length === 0 && (
            <div className="py-20 text-center">
              <div className="mb-1.5 font-display text-[22px] italic text-fg-mid">
                Nothing matches just yet.
              </div>
              <div className="text-[14px] text-fg-faint">
                Try a different keyword or clear a filter.
              </div>
            </div>
          )}

          {!loading && !error && jobs.length > 0 && (
            <>
              <div className="flex flex-col gap-3">
                {jobs.map((job) => (
                  <JobCardClassic key={job.id} job={job} />
                ))}
              </div>
              <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
            </>
          )}
        </main>
      </div>
    </div>
  )
}

export { JobFeed }
