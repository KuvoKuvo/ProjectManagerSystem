import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'
import http from 'node:http'
import https from 'node:https'

// Функция для быстрой проверки доступности порта бэкенда
async function checkBackendEndpoint(url: string): Promise<boolean> {
  return new Promise((resolve) => {
    const client = url.startsWith('https') ? https : http
    const req = client.request(url, { 
      method: 'HEAD', 
      timeout: 800, // Быстрый таймаут в миллисекундах
      rejectUnauthorized: false // Игнорируем самоподписанные сертификаты локалки
    }, (res) => {
      resolve(true)
    })
    req.on('error', () => resolve(false))
    req.on('timeout', () => {
      req.destroy()
      resolve(false)
    })
    req.end()
  })
}

export default defineConfig(async () => {
  const httpsTarget = 'https://localhost:7069'
  const httpTarget = 'http://localhost:5116'
  
  // Динамически проверяем, поднялся ли HTTPS у бэкенда
  const isHttpsAlive = await checkBackendEndpoint(httpsTarget)
  const activeTarget = isHttpsAlive ? httpsTarget : httpTarget

  console.log(`\x1b[32m[Vite Proxy] Backend detected! Routing API traffic to: ${activeTarget}\x1b[0m`)

  return {
    plugins: [
      vue(),
      tailwindcss(),
    ],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
    },
    server: {
      port: 5173,
      proxy: {
        '/api': {
          target: activeTarget,
          changeOrigin: true,
          secure: false // Разрешает работу с самоподписанным локальным SSL
        }
      }
    }
  }
})