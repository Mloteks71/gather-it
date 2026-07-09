import { useState } from "react"

import { Checkbox } from "@/components/ui/checkbox"
import { SearchInput } from "@/components/ui/search-input"
import { cn } from "@/lib/utils"
import {
  JobSite,
  jobSiteLabels,
  OfferStatus,
  offerStatusLabels,
} from "@/lib/api"
import { placeholderSkillOptions } from "./data"

const statusOptions = Object.values(OfferStatus)
const siteOptions = Object.values(JobSite)

function FilterSection({
  label,
  children,
}: {
  label: string
  children: React.ReactNode
}) {
  return (
    <div className="mb-[26px] border-b border-white/[0.06] pb-[22px]">
      <div className="mb-3 font-mono text-[10px] font-semibold tracking-[0.18em] text-fg-faint uppercase">
        {label}
      </div>
      {children}
    </div>
  )
}

function FilterOption({
  label,
  checked,
  onCheckedChange,
}: {
  label: string
  checked: boolean
  onCheckedChange: (checked: boolean) => void
}) {
  return (
    <label className="flex cursor-pointer items-center gap-[11px] py-[5px]">
      <Checkbox
        checked={checked}
        onCheckedChange={(value) => onCheckedChange(value === true)}
      />
      <span
        className={cn(
          "flex-1 text-[13.5px]",
          checked ? "font-medium text-card-foreground" : "text-fg-soft"
        )}
      >
        {label}
      </span>
    </label>
  )
}

function SkillsFilterGroup({
  selected,
  onToggle,
}: {
  selected: string[]
  onToggle: (skill: string, checked: boolean) => void
}) {
  const [query, setQuery] = useState("")

  const visible = placeholderSkillOptions.filter((skill) =>
    skill.toLowerCase().includes(query.trim().toLowerCase())
  )

  return (
    <FilterSection label="Skills">
      <SearchInput
        size="sm"
        className="mb-[11px]"
        value={query}
        onChange={(event) => setQuery(event.target.value)}
        placeholder="Search skills…"
      />
      {visible.map((skill) => (
        <FilterOption
          key={skill}
          label={skill}
          checked={selected.includes(skill)}
          onCheckedChange={(checked) => onToggle(skill, checked)}
        />
      ))}
      {visible.length === 0 && (
        <div className="py-[5px] text-[12.5px] text-muted-foreground">
          No matching skills.
        </div>
      )}
    </FilterSection>
  )
}

interface FilterGroupProps<T extends number> {
  label: string
  options: T[]
  labels: Record<T, string>
  selected?: T
  onChange: (value?: T) => void
}

// Rendered as a checkbox list to match the design, but the API only accepts a
// single status/site value, so the group stays single-select for now.
function FilterGroup<T extends number>({
  label,
  options,
  labels,
  selected,
  onChange,
}: FilterGroupProps<T>) {
  return (
    <FilterSection label={label}>
      {options.map((option) => (
        <FilterOption
          key={option}
          label={labels[option]}
          checked={selected === option}
          onCheckedChange={(checked) => onChange(checked ? option : undefined)}
        />
      ))}
    </FilterSection>
  )
}

interface FilterSidebarProps {
  status?: OfferStatus
  site?: JobSite
  skills: string[]
  onStatusChange: (value?: OfferStatus) => void
  onSiteChange: (value?: JobSite) => void
  onSkillToggle: (skill: string, checked: boolean) => void
}

function FilterSidebar({
  status,
  site,
  skills,
  onStatusChange,
  onSiteChange,
  onSkillToggle,
}: FilterSidebarProps) {
  return (
    <aside className="lg:sticky lg:top-[82px] lg:max-h-[calc(100vh-82px)] lg:overflow-y-auto lg:pb-10">
      <SkillsFilterGroup selected={skills} onToggle={onSkillToggle} />
      <FilterGroup
        label="Status"
        options={statusOptions}
        labels={offerStatusLabels}
        selected={status}
        onChange={onStatusChange}
      />
      <FilterGroup
        label="Source"
        options={siteOptions}
        labels={jobSiteLabels}
        selected={site}
        onChange={onSiteChange}
      />
    </aside>
  )
}

export { FilterSidebar }
