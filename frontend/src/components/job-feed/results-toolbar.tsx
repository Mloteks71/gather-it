import { Button } from "@/components/ui/button"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { cn } from "@/lib/utils"
import type { Layout } from "./data"

const layouts: { key: Layout; label: string }[] = [
  { key: "classic", label: "Classic" },
  { key: "compact", label: "Compact" },
  { key: "detailed", label: "Detailed" },
]

interface ResultsToolbarProps {
  count: number
  total: number
  layout: Layout
}

function ResultsToolbar({ count, total, layout }: ResultsToolbarProps) {
  return (
    <section className="flex flex-wrap items-center justify-between gap-4 pb-[22px]">
      <div className="font-mono text-[13.5px] text-muted-foreground">
        <span className="font-medium text-foreground">{count}</span> of {total}{" "}
        roles
      </div>

      <div className="flex items-center gap-4">
        <div className="flex items-center gap-1 rounded-[10px] border border-white/[0.08] bg-white/[0.04] p-1">
          {layouts.map((item) => (
            <Button
              key={item.key}
              variant="ghost"
              className={cn(
                "h-auto rounded-[7px] px-3.5 py-1.5 text-[13px] font-semibold hover:bg-transparent",
                item.key === layout
                  ? "bg-[#262b34] text-card-foreground hover:bg-[#262b34] hover:text-card-foreground"
                  : "text-[#6b7280] hover:text-[#c4c9d2]"
              )}
            >
              {item.label}
            </Button>
          ))}
        </div>

        <div className="flex items-center gap-2">
          <span className="font-mono text-[12.5px] text-[#6b7280]">SORT</span>
          <Select defaultValue="recent">
            <SelectTrigger className="h-9 rounded-[9px] bg-white/[0.04] text-[13px] font-medium">
              <SelectValue />
            </SelectTrigger>
            <SelectContent align="end">
              <SelectItem value="recent">Most recent</SelectItem>
              <SelectItem value="salary">Salary: high to low</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>
    </section>
  )
}

export { ResultsToolbar }
