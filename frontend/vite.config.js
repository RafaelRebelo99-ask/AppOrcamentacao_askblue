import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Em desenvolvimento, todos os pedidos a /api/... são reencaminhados para o backend .NET.
    // Assim o front chama sempre URLs relativas (fetch('/api/...')) e não há problemas de CORS.
    proxy: {
      '/api': 'http://localhost:5126',
    },
  },
})
