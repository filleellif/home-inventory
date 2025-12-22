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
  ],

  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'https://localhost:5001/api'
    }
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
})
