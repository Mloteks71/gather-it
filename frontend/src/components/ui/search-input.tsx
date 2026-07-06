import * as React from "react"
import { Search } from "lucide-react"

import { Input } from "@/components/ui/input"
import { cn } from "@/lib/utils"

const sizeStyles = {
  default: {
    icon: "left-3.5 size-4",
    input: "h-10 rounded-[10px] pl-10",
  },
  sm: {
    icon: "left-2.5 size-[13px]",
    input: "h-8 rounded-lg pl-[30px] text-[12.5px]",
  },
}

interface SearchInputProps
  extends Omit<React.ComponentProps<typeof Input>, "size"> {
  size?: keyof typeof sizeStyles
}

/**
 * An Input with the search icon overlaid, as used in the site header and the
 * sidebar skills filter. `className` styles the wrapper element.
 */
function SearchInput({
  size = "default",
  className,
  ...props
}: SearchInputProps) {
  return (
    <div className={cn("relative flex items-center", className)}>
      <Search
        className={cn(
          "pointer-events-none absolute text-muted-foreground",
          sizeStyles[size].icon
        )}
      />
      <Input className={sizeStyles[size].input} {...props} />
    </div>
  )
}

export { SearchInput }
