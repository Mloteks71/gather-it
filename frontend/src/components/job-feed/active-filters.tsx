import { X } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"

export interface FilterToken {
  facet: string
  value: string
  onRemove: () => void
}

interface ActiveFiltersProps {
  tokens: FilterToken[]
  onClearAll: () => void
}

function ActiveFilters({ tokens, onClearAll }: ActiveFiltersProps) {
  if (tokens.length === 0) return null

  return (
    <div className="mb-5 flex flex-wrap items-center gap-2">
      {tokens.map((token) => (
        <Badge
          key={`${token.facet}:${token.value}`}
          variant="brand"
          className="gap-2 rounded-full py-1.5 pr-2 pl-[13px]"
        >
          <span className="font-mono text-[9.5px] tracking-[0.08em] text-brand-soft uppercase">
            {token.facet}
          </span>
          <span className="text-[12.5px] font-medium text-card-foreground">
            {token.value}
          </span>
          <Button
            variant="ghost"
            size="icon-xs"
            onClick={token.onRemove}
            className="size-auto rounded-full p-0 text-brand-soft hover:bg-transparent hover:text-white"
            aria-label={`Remove ${token.value} filter`}
          >
            <X className="size-3.5" />
          </Button>
        </Badge>
      ))}
      <Button
        variant="ghost"
        onClick={onClearAll}
        className="h-auto px-1 py-1.5 text-[12.5px] font-normal text-fg-dim hover:bg-transparent hover:text-card-foreground"
      >
        Clear all
      </Button>
    </div>
  )
}

export { ActiveFilters }
