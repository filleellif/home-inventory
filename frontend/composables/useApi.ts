import type { UseFetchOptions } from 'nuxt/app'

export const useApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase
  const { startLoading, stopLoading } = useLoading()

  const apiFetch = async <T>(
    endpoint: string,
    options: UseFetchOptions<T> = {}
  ) => {
    // Merge default options with provided options
    const defaultOptions: UseFetchOptions<T> = {
      baseURL,
      credentials: 'include', // For future auth cookies
      // Skip SSR for localhost APIs (not accessible from server)
      server: baseURL.includes('localhost') ? false : true,
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

    const result = await useFetch<T>(endpoint, {
      ...defaultOptions,
      ...options,
      headers: {
        ...defaultOptions.headers,
        ...options.headers,
      } as HeadersInit
    })

    // Watch pending state to control loading indicator (client-side only)
    if (process.client) {
      watch(result.pending, (isPending) => {
        if (isPending) {
          startLoading()
        } else {
          stopLoading()
        }
      }, { immediate: true })
    }

    return result
  }

  return {
    apiFetch
  }
}
