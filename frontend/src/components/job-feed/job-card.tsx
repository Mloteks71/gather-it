import { useState } from "react"
import { Bookmark } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card } from "@/components/ui/card"
import { cn } from "@/lib/utils"
import { formatSalary, getJobAd, type JobAdDetails } from "@/lib/api"
import { type Job } from "./data"

function LogoBadge({
  job,
  className,
}: {
  job: Job
  className?: string
}) {
  return (
    <div
      className={cn(
        "flex flex-none items-center justify-center font-sans font-bold",
        className
      )}
      style={{ background: job.logoBg, color: job.logoColor }}
    >
      {job.initial}
    </div>
  )
}

function SkillChip({ label }: { label: string }) {
  return (
    <Badge
      variant="outline"
      className="rounded-md px-[9px] py-1 font-mono text-[11.5px] font-normal text-fg-soft"
    >
      {label}
    </Badge>
  )
}

// NOTE: saving a role is not wired to any logic yet (same as before the
// redesign) — the design toggles a filled bookmark per job.
function SaveButton() {
  return (
    <Button
      type="button"
      variant="ghost"
      size="icon"
      aria-label="Save role"
      onClick={(event) => event.stopPropagation()}
      className="size-auto flex-none rounded-md p-0.5 text-muted-foreground hover:bg-transparent hover:text-muted-foreground hover:opacity-70"
    >
      <Bookmark className="size-[18px]" />
    </Button>
  )
}

/* ── Expanded detail section (lazy-loaded from GET /api/job-ads/{id}) ────── */

function DetailSection({ label, text }: { label: string; text: string }) {
  return (
    <div>
      <div className="mb-1.5 font-mono text-[10.5px] tracking-[0.05em] text-fg-dim uppercase">
        {label}
      </div>
      <p className="text-[13.5px] leading-relaxed whitespace-pre-line text-fg-soft">
        {text}
      </p>
    </div>
  )
}

function JobDetails({ details }: { details: JobAdDetails }) {
  const description = details.descriptions[0]

  return (
    <div className="flex flex-col gap-4">
      {details.salaries.length > 0 && (
        <div className="flex flex-wrap gap-2">
          {details.salaries.map((salary) => (
            <Badge
              key={salary.salaryId}
              variant="outline"
              className="rounded-[7px] px-[11px] py-[5px] font-mono text-[12.5px] font-normal text-positive"
            >
              {formatSalary(salary)}
            </Badge>
          ))}
        </div>
      )}

      {details.skills.length > 0 && (
        <div className="flex flex-wrap gap-[7px]">
          {details.skills.map((skill) => (
            <SkillChip key={skill} label={skill} />
          ))}
        </div>
      )}

      {description?.descriptionText && (
        <DetailSection label="About" text={description.descriptionText} />
      )}
      {description?.requirements && (
        <DetailSection label="Requirements" text={description.requirements} />
      )}
      {description?.benefits && (
        <DetailSection label="Benefits" text={description.benefits} />
      )}

      {details.salaries.length === 0 &&
        details.skills.length === 0 &&
        !description && (
          <p className="text-[13px] text-muted-foreground">
            No additional details available for this role.
          </p>
        )}
    </div>
  )
}

/* ── Card ─────────────────────────────────────────────────────────────────── */

// Layout follows the design: colored left edge, 52px initial square, meta row
// above a large title on the left, and a save/seniority/posted column on the
// right. NOTE for later: the design also shows salary and company
// size/industry directly on the card — the list endpoint doesn't return
// those, so salary only appears in the expanded details and the skill chips
// are placeholders (see data.ts). The design has
// no expand affordance; clicking anywhere on the card still expands it because
// that's how details load today.
function JobCardClassic({ job }: { job: Job }) {
  const [expanded, setExpanded] = useState(false)
  const [details, setDetails] = useState<JobAdDetails | null>(null)
  const [status, setStatus] = useState<"idle" | "loading" | "error">("idle")

  async function toggle() {
    const next = !expanded
    setExpanded(next)

    if (next && details === null && status !== "loading") {
      setStatus("loading")
      try {
        setDetails(await getJobAd(job.id))
        setStatus("idle")
      } catch {
        setStatus("error")
      }
    }
  }

  return (
    <Card
      className="relative rounded-[12px] border-l-[3px] py-[22px] pr-6 pl-[22px] transition-all hover:border-white/[0.16] hover:bg-surface-hover"
      style={{ borderLeftColor: job.logoColor }}
    >
      {job.isNew && (
        <Badge
          variant="outline"
          size="sm"
          className="absolute -top-[9px] right-[18px] border-positive/40 bg-card text-positive"
        >
          New
        </Badge>
      )}
      <div
        role="button"
        tabIndex={0}
        aria-expanded={expanded}
        onClick={toggle}
        onKeyDown={(event) => {
          if (event.key === "Enter" || event.key === " ") {
            event.preventDefault()
            void toggle()
          }
        }}
        className="cursor-pointer outline-none"
      >
        <div className="flex items-start gap-[18px]">
          <LogoBadge
            job={job}
            className="size-[52px] rounded-[10px] text-[21px]"
          />

          <div className="min-w-0 flex-1">
            <div className="flex items-start justify-between gap-6">
              <div className="min-w-0">
                <div className="flex flex-wrap items-center gap-x-2.5 gap-y-1">
                  <span
                    className="font-mono text-[10px] font-bold tracking-[0.12em] uppercase"
                    style={{ color: job.logoColor }}
                  >
                    {job.category}
                  </span>
                  <span className="font-mono text-[11px] tracking-[0.04em] text-fg-dim uppercase">
                    {[job.city, ...job.workTypes].join(" · ")}
                  </span>
                </div>
                <div className="mt-[7px] font-display text-[26px] leading-[1.12] text-card-foreground">
                  {job.title}
                </div>
                <div className="mt-[7px] text-[13px] text-fg-mid">
                  {job.company}
                </div>
                {job.skills.length > 0 && (
                  <div className="mt-3 flex flex-wrap gap-1.5">
                    {job.skills.map((skill) => (
                      <SkillChip key={skill} label={skill} />
                    ))}
                  </div>
                )}
              </div>

              {/* Bookmark and posted·site pin the top/bottom corners so the
                  column reads the same on every card; missing salary and
                  seniority fall back to muted placeholder text. */}
              <div className="flex flex-none flex-col items-end justify-between gap-2 self-stretch whitespace-nowrap">
                <SaveButton />
                <div className="flex flex-col items-end gap-2">
                  {job.salary ? (
                    <div className="font-display text-[26px] leading-none text-card-foreground">
                      {job.salary}
                    </div>
                  ) : (
                    <div className="text-[12px] text-fg-faint">
                      Salary undisclosed
                    </div>
                  )}
                  <div className="font-mono text-[9.5px] font-semibold tracking-[0.14em] text-fg-faint uppercase">
                    {job.seniority ?? "Unspecified"}
                  </div>
                </div>
                {/* mb offsets the skill chips' bottom padding + border so
                    this line's text baseline matches the chip text. */}
                <div className="mb-[5px] text-[11px] text-fg-dim">
                  {job.posted} · {job.source}
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {expanded && (
        <div className="mt-4 border-t border-white/[0.07] pt-4">
          {status === "loading" && (
            <p className="text-[13px] text-muted-foreground">Loading details…</p>
          )}
          {status === "error" && (
            <p className="text-[13px] text-destructive">
              Couldn’t load details. Try again later.
            </p>
          )}
          {details && <JobDetails details={details} />}
        </div>
      )}
    </Card>
  )
}

export { JobCardClassic }
