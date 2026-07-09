/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Base URL of the JobReadApi service. Defaults to "/api" (see the dev proxy). */
  readonly VITE_API_BASE_URL?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
