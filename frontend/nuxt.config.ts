// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-01-01',
  devtools: { enabled: true },
  future: {
    compatibilityVersion: 4,
  },

  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt',
    '@vite-pwa/nuxt',
  ],

  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5000/api'
    },
    // Server-side API URL (use HTTP to avoid self-signed cert issues during SSR)
    apiBase: process.env.NUXT_API_BASE || 'http://localhost:5000/api'
  },

  typescript: {
    strict: true,
    typeCheck: false // Set to true for type checking on build
  },

  app: {
    head: {
      title: 'Home Inventory',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'Manage your home inventory' }
      ],
    }
  },

  css: ['~/assets/css/tailwind.css'],

  // Auto-import components
  components: [
    {
      path: '~/components',
      pathPrefix: false,
    }
  ],

  // PWA configuration
  pwa: {
    registerType: 'autoUpdate',
    manifest: {
      name: 'Home Inventory',
      short_name: 'Inventory',
      description: 'Manage your home inventory offline',
      theme_color: '#ffffff',
      background_color: '#ffffff',
      display: 'standalone',
      start_url: '/',
      scope: '/',
      icons: [
        {
          src: '/pwa-192x192.png',
          sizes: '192x192',
          type: 'image/png',
          purpose: 'any maskable'
        },
        {
          src: '/pwa-512x512.png',
          sizes: '512x512',
          type: 'image/png',
          purpose: 'any maskable'
        }
      ]
    },
    workbox: {
      navigateFallback: '/',
      navigateFallbackDenylist: [/^\/api\//, /^\/\w+\.(js|css|json)$/],
      cleanupOutdatedCaches: true,
      clientsClaim: true,
      skipWaiting: true,
      // Precache all built assets for complete offline support
      globPatterns: ['**/*.{js,css,html,ico,png,svg,woff,woff2}'],
      globIgnores: ['**/node_modules/**/*', '**/server/**/*'],
      runtimeCaching: [
        {
          urlPattern: ({ url }) => url.pathname.startsWith('/api/'),
          handler: 'NetworkFirst',
          options: {
            cacheName: 'api-cache',
            networkTimeoutSeconds: 3,
            expiration: {
              maxEntries: 500,
              maxAgeSeconds: 86400
            },
            cacheableResponse: {
              statuses: [0, 200]
            }
          }
        },
        {
          urlPattern: ({ url }) => {
            return url.pathname.startsWith('/_nuxt/') ||
                   url.pathname.includes('/pages/') ||
                   url.pathname.includes('/components/')
          },
          handler: 'StaleWhileRevalidate',
          options: {
            cacheName: 'nuxt-components',
            expiration: {
              maxEntries: 200,
              maxAgeSeconds: 86400
            },
            cacheableResponse: {
              statuses: [0, 200]
            }
          }
        },
        {
          urlPattern: /\.(?:js|css|woff2?|png|jpg|jpeg|svg|gif|webp)$/i,
          handler: 'CacheFirst',
          options: {
            cacheName: 'static-assets',
            expiration: {
              maxEntries: 100,
              maxAgeSeconds: 86400
            }
          }
        }
      ]
    },
    devOptions: {
      enabled: true,
      type: 'module'
    }
  }
})
