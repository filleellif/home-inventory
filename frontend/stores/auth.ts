import { defineStore } from 'pinia'
import type { User, LoginCredentials, RegisterCredentials } from '~/types/auth'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as User | null,
    token: null as string | null,
    isAuthenticated: false,
  }),

  getters: {
    currentUser: (state) => state.user,
    isLoggedIn: (state) => state.isAuthenticated,
  },

  actions: {
    async login(credentials: LoginCredentials) {
      // TODO: Implement when backend has auth
      // const response = await $fetch('/auth/login', { ... })

      // Mock for now
      console.log('Login called with:', credentials)
      this.user = { id: '1', email: credentials.email, name: 'Test User' }
      this.token = 'mock-token'
      this.isAuthenticated = true

      navigateTo('/')
    },

    async register(credentials: RegisterCredentials) {
      // TODO: Implement when backend has auth
      console.log('Register called with:', credentials)
    },

    logout() {
      this.user = null
      this.token = null
      this.isAuthenticated = false
      navigateTo('/auth/login')
    },

    // Initialize auth state from storage
    initAuth() {
      // TODO: Load from localStorage/cookie when backend has auth
      if (process.client) {
        const token = localStorage.getItem('auth_token')
        if (token) {
          this.token = token
          this.isAuthenticated = true
          // Fetch user data
        }
      }
    },
  },
})
