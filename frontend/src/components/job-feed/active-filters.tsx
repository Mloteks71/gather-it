import { X } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { activeTokens } from "./data"

function ActiveFilters() {
  if (activeTokens.length === 0) return null

  return (
    <div className="mt-3.5 flex flex-wrap items-center gap-2">
      {activeTokens.map((token) => (
        <Badge
          key={`${token.facet}:${token.value}`}
          variant="brand"
          className="gap-[7px] rounded-full py-1.5 pr-2 pl-[11px]"
        >
          <span className="font-mono text-[9.5px] tracking-[0.05em] text-brand-soft uppercase">
            {token.facet}
          </span>
          <span className="text-[12.5px] font-medium text-card-foreground">
            {token.value}
          </span>
          <Button
            variant="ghost"
            size="icon-xs"
            className="size-auto rounded-full p-0 text-brand-soft hover:bg-transparent hover:text-white"
            aria-label={`Remove ${token.value} filter`}
          >
            <X className="size-3.5" />
          </Button>
        </Badge>
      ))}
      <Button
        variant="link"
        className="h-auto px-1 py-1.5 text-[12px] font-normal text-muted-foreground no-underline hover:text-[#c4c9d2] hover:no-underline"
      >
        Clear all
      </Button>
    </div>
  )
}

export { ActiveFilters }
