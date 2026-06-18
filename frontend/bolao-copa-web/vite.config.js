import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  base: '/bolao-copa-2026/',
  server: {
    port: 5173
  }
})
