import { ChevronDown } from "lucide-react"

interface ResultsToolbarProps {
  count: number
  total: number
}

// NOTE: the sort select is not wired to any logic yet (same as before the
// redesign) — the design sorts client-side; our API would need a sort param.
// It's a styled native <select> like in the design, not the shadcn popover
// Select, so opening it uses the platform dropdown.
function ResultsToolbar({ count, total }: ResultsToolbarProps) {
  return (
    <section className="mb-[18px] flex flex-wrap items-center justify-between gap-4">
      <div className="font-mono text-[13.5px] text-muted-foreground">
        <span className="font-medium text-foreground">{count}</span> of {total}{" "}
        roles
      </div>

      <div className="flex items-center gap-2.5">
        <span className="font-mono text-[10px] font-semibold tracking-[0.14em] text-fg-faint uppercase">
          Sort
        </span>
        <div className="relative flex items-center">
          <select
            defaultValue="recent"
            className="h-9 cursor-pointer appearance-none rounded-[9px] border border-white/[0.12] bg-secondary pr-[30px] pl-[13px] text-[13px] font-medium text-card-foreground outline-none focus-visible:border-brand"
          >
            <option value="recent">Most recent</option>
            <option value="salary">Salary: high to low</option>
          </select>
          <ChevronDown
            className="pointer-events-none absolute right-[11px] size-[11px] text-fg-mid"
            strokeWidth={2.5}
          />
        </div>
      </div>
    </section>
  )
}

export { ResultsToolbar }
