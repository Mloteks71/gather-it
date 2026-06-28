import { ActiveFilters } from "./active-filters"
import { FilterBar } from "./filter-bar"
import { Hero } from "./hero"
import { JobCardClassic } from "./job-card"
import { ResultsToolbar } from "./results-toolbar"
import { SiteHeader } from "./site-header"
import { jobs } from "./data"

function JobFeed() {
  return (
    <div className="min-h-svh bg-background pb-20 text-foreground">
      <SiteHeader />

      <main className="mx-auto max-w-[1080px] px-6">
        <Hero />

        <div className="border-t border-white/[0.06] pt-5">
          <FilterBar />
          <ActiveFilters />

          <div className="my-5 h-px bg-white/[0.06]" />

          <ResultsToolbar count={jobs.length} total={jobs.length} layout="classic" />

          <div className="flex flex-col gap-3">
            {jobs.map((job) => (
              <JobCardClassic key={job.id} job={job} />
            ))}
          </div>
        </div>
      </main>
    </div>
  )
}

export { JobFeed }
