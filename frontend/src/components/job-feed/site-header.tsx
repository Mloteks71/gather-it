import { Search } from "lucide-react"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Logo } from "./logo"

function SiteHeader() {
  return (
    <header className="sticky top-0 z-20 border-b border-white/[0.07] bg-background/80 backdrop-blur-xl">
      <div className="mx-auto flex max-w-[1080px] items-center gap-6 px-6 py-3.5">
        <div className="flex flex-none items-center gap-2.5">
          <Logo />
          <span className="font-display text-[19px] font-bold tracking-tight">
           Gather IT
          </span>
        </div>

        <div className="relative flex w-full max-w-[440px] items-center">
          <Search className="pointer-events-none absolute left-3.5 size-4 text-muted-foreground" />
          <Input
            className="h-10 rounded-[10px] pl-10"
            placeholder="Search roles, companies, skills…"
          />
        </div>

        <div className="flex-1" />

        <Button variant="outline" className="h-9 flex-none rounded-[9px] px-4">
          Sign in
        </Button>
      </div>
    </header>
  )
}

export { SiteHeader }
