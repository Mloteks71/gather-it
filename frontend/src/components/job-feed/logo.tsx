function Logo({ size = 26 }: { size?: number }) {
  return (
    <svg width={size} height={size} viewBox="0 0 24 24" fill="none" aria-hidden>
      <rect x="3" y="4" width="18" height="4" rx="1.4" fill="#5b8cff" />
      <rect x="3" y="10" width="18" height="4" rx="1.4" fill="#5b8cff" opacity="0.6" />
      <rect x="3" y="16" width="18" height="4" rx="1.4" fill="#5b8cff" opacity="0.3" />
    </svg>
  )
}

export { Logo }
