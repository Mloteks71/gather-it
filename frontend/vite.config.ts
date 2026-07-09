import path from "path"
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
// Proxy target for the JobReadApi service during development.
// Docker maps the API to 8002 (see Docker/docker-compose.yml); a local
// `dotnet run` uses 5139. Override with VITE_API_PROXY_TARGET if needed.
const apiTarget = process.env.VITE_API_PROXY_TARGET ?? "http://localhost:8002"

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    proxy: {
      "/api": {
        target: apiTarget,
        changeOrigin: true,
      },
    },
  },
})
