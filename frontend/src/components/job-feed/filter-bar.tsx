import { ChevronDown } from "lucide-react"

import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { cn } from "@/lib/utils"
import { filterFacets } from "./data"

function FilterBar() {
  return (
    <div className="flex flex-wrap items-center gap-2.5">
      {filterFacets.map((facet) => {
        const activeCount = facet.options.filter((option) => option.active).length

        return (
          <DropdownMenu key={facet.label}>
            <DropdownMenuTrigger asChild>
              <Button
                variant="outline"
                className={cn(
                  "h-9 gap-2 rounded-[9px] px-3.5 text-[13px] font-medium",
                  activeCount > 0 &&
                    "border-brand/50 bg-brand/[0.12] text-card-foreground hover:bg-brand/[0.18]"
                )}
              >
                <span>{facet.label}</span>
                {activeCount > 0 && (
                  <span className="rounded-[5px] bg-brand px-[5px] py-px font-mono text-[10px] font-bold text-brand-foreground">
                    {activeCount}
                  </span>
                )}
                <ChevronDown className="size-[11px] opacity-80" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent
              align="start"
              className="max-h-[300px] w-auto min-w-[218px]"
            >
              {facet.options.map((option) => (
                <DropdownMenuCheckboxItem
                  key={option.label}
                  checked={option.active}
                >
                  <span className="flex-1">{option.label}</span>
                  <span className="font-mono text-[11px] text-muted-foreground">
                    {option.count}
                  </span>
                </DropdownMenuCheckboxItem>
              ))}
            </DropdownMenuContent>
          </DropdownMenu>
        )
      })}
    </div>
  )
}

export { FilterBar }
