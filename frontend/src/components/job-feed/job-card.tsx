import { Bookmark } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card } from "@/components/ui/card"
import { cn } from "@/lib/utils"
import { type Job, workTypeColor } from "./data"

function Dot({ color }: { color: string }) {
  return (
    <span
      className="inline-block size-[7px] flex-none rounded-full"
      style={{ background: color }}
    />
  )
}

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
        "flex flex-none items-center justify-center font-display font-bold",
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
      className="rounded-md px-[9px] py-1 font-mono text-[11.5px] font-normal text-[#b8bdc7]"
    >
      {label}
    </Badge>
  )
}

function SaveButton({ iconClassName = "size-[18px]" }: { iconClassName?: string }) {
  return (
    <Button
      type="button"
      variant="ghost"
      size="icon"
      aria-label="Save role"
      className="size-auto flex-none rounded-md p-0.5 text-muted-foreground hover:bg-transparent hover:text-muted-foreground hover:opacity-70"
    >
      <Bookmark className={iconClassName} />
    </Button>
  )
}

const cardHover =
  "cursor-pointer transition-all hover:border-white/[0.16] hover:bg-surface-hover"

/* ── Classic ──────────────────────────────────────────────────────────── */

function JobCardClassic({ job }: { job: Job }) {
  return (
    <Card className={cn("rounded-[14px] px-[22px] py-5", cardHover, "hover:-translate-y-px")}>
      <div className="flex items-start justify-between gap-4">
        <div className="flex min-w-0 items-center gap-3.5">
          <LogoBadge job={job} className="size-12 rounded-[12px] text-[20px]" />
          <div className="min-w-0">
            <div className="flex flex-wrap items-center gap-x-2.5 gap-y-1">
              <span className="font-display text-[16.5px] font-semibold tracking-tight text-card-foreground">
                {job.title}
              </span>
              <Badge variant="brand" size="sm">
                {job.seniority}
              </Badge>
            </div>
            <div className="mt-1.5 text-[13.5px] text-[#9ba1ab]">
              {job.company} · {job.size} · {job.industry}
            </div>
          </div>
        </div>
        <div className="flex flex-none flex-col items-end gap-2.5">
          <SaveButton />
          <span className="font-mono text-[14.5px] font-medium whitespace-nowrap text-positive">
            {job.salary}
          </span>
        </div>
      </div>

      <div className="mt-[15px] flex flex-wrap items-center gap-x-[13px] gap-y-1.5 font-mono text-[11.5px] text-[#7c828c]">
        <span className="inline-flex items-center gap-1.5 text-[#9ba1ab]">
          <Dot color={job.sourceColor} />
          {job.source}
        </span>
        <span className="text-[#3a3e46]">·</span>
        <span className="inline-flex items-center gap-1.5">
          <Dot color={workTypeColor[job.workType]} />
          {job.workType}
        </span>
        <span className="text-[#3a3e46]">·</span>
        <span>{job.location}</span>
        <span className="text-[#3a3e46]">·</span>
        <span>{job.posted}</span>
      </div>

      <div className="mt-3.5 flex flex-wrap gap-[7px]">
        {job.skills.slice(0, 4).map((skill) => (
          <SkillChip key={skill} label={skill} />
        ))}
      </div>
    </Card>
  )
}

/* ── Compact ──────────────────────────────────────────────────────────── */

function JobCardCompact({ job }: { job: Job }) {
  return (
    <Card
      className={cn(
        "grid grid-cols-[auto_1fr_auto_auto] items-center gap-4 rounded-[11px] px-[18px] py-[13px]",
        cardHover
      )}
    >
      <LogoBadge job={job} className="size-9 rounded-[9px] text-[15px]" />

      <div className="min-w-0">
        <div className="flex flex-wrap items-center gap-x-2.5 gap-y-1">
          <span className="font-display text-[14.5px] font-semibold text-card-foreground">
            {job.title}
          </span>
          <span className="text-[12.5px] text-[#7c828c]">{job.company}</span>
          <span className="text-[9.5px] font-semibold tracking-[0.04em] text-brand-soft uppercase">
            {job.seniority}
          </span>
        </div>
        <div className="mt-1 flex flex-wrap items-center gap-x-2.5 gap-y-1 font-mono text-[11px] text-[#7c828c]">
          <span className="inline-flex items-center gap-1.5">
            <Dot color={workTypeColor[job.workType]} />
            {job.workType} · {job.location}
          </span>
          <span className="text-[#3a3e46]">·</span>
          <span>{job.posted}</span>
          <span className="text-[#3a3e46]">·</span>
          <span className="text-[#5a6068]">{job.skills.slice(0, 3).join(" · ")}</span>
        </div>
      </div>

      <div className="flex flex-col items-end gap-1">
        <span className="font-mono text-[13px] font-medium whitespace-nowrap text-positive">
          {job.salary}
        </span>
        <span className="inline-flex items-center gap-1.5 font-mono text-[10px] text-[#7c828c]">
          <Dot color={job.sourceColor} />
          {job.source}
        </span>
      </div>

      <SaveButton iconClassName="size-4" />
    </Card>
  )
}

/* ── Detailed ─────────────────────────────────────────────────────────── */

function JobCardDetailed({ job }: { job: Job }) {
  return (
    <Card className={cn("rounded-2xl px-[26px] py-6", cardHover)}>
      <div className="flex items-start justify-between gap-4">
        <div className="flex items-center gap-[15px]">
          <LogoBadge job={job} className="size-[54px] rounded-[14px] text-[23px]" />
          <div>
            <div className="text-[14.5px] font-medium text-foreground">
              {job.company}
            </div>
            <div className="mt-[3px] text-[12.5px] text-[#7c828c]">
              {job.size} · {job.industry}
            </div>
          </div>
        </div>
        <div className="flex items-center gap-3.5">
          <span className="inline-flex items-center gap-1.5 font-mono text-[11px] tracking-[0.03em] text-[#9ba1ab] uppercase">
            <Dot color={job.sourceColor} />
            {job.source}
          </span>
          <span className="font-mono text-[11.5px] text-[#7c828c]">
            {job.posted}
          </span>
          <SaveButton iconClassName="size-[19px]" />
        </div>
      </div>

      <h3 className="mt-[17px] font-display text-[22px] font-semibold tracking-tight text-card-foreground">
        {job.title}
      </h3>

      <div className="mt-[13px] flex flex-wrap gap-2">
        <Badge variant="brand" className="rounded-[7px] px-[11px] py-[5px] text-xs">
          {job.seniority}
        </Badge>
        <Badge
          variant="outline"
          className="gap-1.5 rounded-[7px] px-[11px] py-[5px] text-[12.5px] font-normal text-[#c4c9d2]"
        >
          <Dot color={workTypeColor[job.workType]} />
          {job.workType}
        </Badge>
        <Badge
          variant="outline"
          className="rounded-[7px] px-[11px] py-[5px] text-[12.5px] font-normal text-[#c4c9d2]"
        >
          {job.location}
        </Badge>
      </div>

      <div className="mt-[15px] flex flex-wrap gap-[7px]">
        {job.skills.map((skill) => (
          <SkillChip key={skill} label={skill} />
        ))}
      </div>

      <div className="my-5 h-px bg-white/[0.07]" />

      <div className="flex items-center justify-between">
        <span className="font-mono text-[19px] font-medium text-positive">
          {job.salary}
        </span>
        <span className="text-[13.5px] font-semibold text-brand-soft">
          View role →
        </span>
      </div>
    </Card>
  )
}

export { JobCardClassic, JobCardCompact, JobCardDetailed }
