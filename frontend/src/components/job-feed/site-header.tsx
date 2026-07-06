import { Button } from "@/components/ui/button"
import { SearchInput } from "@/components/ui/search-input"
import { Logo } from "./logo"

// NOTE: the search input is not wired to any logic yet (same as before the
// redesign) — the design implies live filtering of the feed.
function SiteHeader() {
  return (
    <header className="sticky top-0 z-20 border-b border-white/[0.10] bg-background">
      <div className="mx-auto grid max-w-[1080px] grid-cols-[1fr_auto_1fr] items-center gap-6 px-7 py-4">
        <div className="flex items-center gap-2.5 justify-self-start">
          <Logo />
          <span className="font-display text-[24px] leading-none">
            Gather <span className="text-brand">IT</span>
          </span>
        </div>

        <SearchInput
          className="w-[420px] max-w-full justify-self-center"
          placeholder="Search roles, companies, skills…"
        />

        <Button
          variant="outline"
          size="lg"
          className="flex-none justify-self-end"
        >
          Sign in
        </Button>
      </div>
    </header>
  )
}

export { SiteHeader }
