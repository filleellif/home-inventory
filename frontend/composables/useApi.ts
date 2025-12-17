import type { UseFetchOptions } from 'nuxt/app'

export const useApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase

  const apiFetch = async <T>(
    endpoint: string,
    options: UseFetchOptions<T> = {}
  ) => {
    // Merge default options with provided options
    const defaultOptions: UseFetchOptions<T> = {
      baseURL,
      credentials: 'include', // For future auth cookies
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
      },
      onRequest({ options }) {
        // Add auth token when available
        const authStore = useAuthStore()
        if (authStore.token) {
          options.headers = {
            ...options.headers as HeadersInit,
            Authorization: `Bearer ${authStore.token}`
          }
        }
      },
      onResponseError({ response }) {
        // Handle global errors
        const toast = useToast()

        if (response.status === 401) {
          // Redirect to login
          navigateTo('/auth/login')
        } else if (response.status >= 500) {
          toast.error('Server error. Please try again later.')
        }
      }
    }

    return useFetch<T>(endpoint, {
      ...defaultOptions,
      ...options,
      headers: {
        ...defaultOptions.headers,
        ...options.headers,
      } as HeadersInit
    })
  }

  return {
    apiFetch
  }
}
