import type { UseFetchOptions } from 'nuxt/app'

export const useApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase
  const { startLoading, stopLoading } = useLoading()
  const { isOnline } = useOffline()
  const { queueOperation } = useSyncQueue()
  const { getFromCache, saveToCache } = useOfflineStorage()

  const apiFetch = async <T>(
    endpoint: string,
    options: UseFetchOptions<T> = {}
  ) => {
    const method = (options.method || 'GET').toUpperCase()
    const isMutation = ['POST', 'PUT', 'DELETE', 'PATCH'].includes(method)

    // Handle offline mutations
    if (!isOnline.value && isMutation) {
      return await handleOfflineMutation<T>(endpoint, options)
    }

    // Handle offline GET requests
    if (!isOnline.value && method === 'GET') {
      const cacheKey = `${endpoint}_${JSON.stringify(options.query || {})}`
      const cached = await getFromCache(cacheKey)

      if (cached) {
        return {
          data: ref(cached) as Ref<T | null>,
          error: ref(null),
          pending: ref(false),
          refresh: async () => {},
          execute: async () => {},
          status: ref('success' as 'idle' | 'pending' | 'success' | 'error'),
          offline: true
        }
      }

      // No cache available
      const toast = useToast()
      toast.error('No cached data available offline')

      return {
        data: ref(null) as Ref<T | null>,
        error: ref({ message: 'Offline and no cache available' } as any),
        pending: ref(false),
        refresh: async () => {},
        execute: async () => {},
        status: ref('error' as 'idle' | 'pending' | 'success' | 'error'),
        offline: true
      }
    }

    // Online - normal flow
    const defaultOptions: UseFetchOptions<T> = {
      baseURL,
      credentials: 'include',
      server: baseURL.includes('localhost') ? false : true,
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
      },
      onRequest({ options }) {
        const authStore = useAuthStore()
        if (authStore.token) {
          options.headers = {
            ...options.headers as HeadersInit,
            Authorization: `Bearer ${authStore.token}`
          }
        }
      },
      onResponseError({ response }) {
        const toast = useToast()

        if (response.status === 401) {
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

    // Cache successful GET responses
    if (process.client && result.data.value && method === 'GET') {
      const cacheKey = `${endpoint}_${JSON.stringify(options.query || {})}`
      await saveToCache(cacheKey, result.data.value)
    }

    // Watch pending state to control loading indicator
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

  // Handle offline mutations (POST, PUT, DELETE)
  const handleOfflineMutation = async <T>(endpoint: string, options: UseFetchOptions<T>) => {
    const method = (options.method || 'POST').toUpperCase()
    const { entityType, entityId, operation } = parseEndpoint(endpoint, method)

    // Generate temp ID for creates
    const tempId = entityId || `temp_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`

    // Queue the operation
    // Convert reactive object to plain object for IndexedDB
    const plainPayload = options.body ? JSON.parse(JSON.stringify(options.body)) : null

    await queueOperation({
      entityType: entityType as 'item' | 'area' | 'category',
      entityId: tempId,
      operation: operation as 'create' | 'update' | 'delete',
      payload: plainPayload,
      timestamp: Date.now()
    })

    // Update local storage optimistically
    const { saveItem, saveArea, saveCategory, deleteItem, deleteArea, deleteCategory } = useOfflineStorage()

    if (operation === 'delete') {
      // Soft delete locally
      if (entityType === 'item') await deleteItem(tempId)
      else if (entityType === 'area') await deleteArea(tempId)
      else if (entityType === 'category') await deleteCategory(tempId)
    } else {
      // Save locally
      const data = { id: tempId, ...options.body } as any

      if (entityType === 'item') {
        await saveItem(data, false, operation === 'create')
      } else if (entityType === 'area') {
        await saveArea(data, false, operation === 'create')
      } else if (entityType === 'category') {
        await saveCategory(data, false, operation === 'create')
      }
    }

    // Return optimistic response
    const responseData = operation === 'delete' ? null : { id: tempId, ...options.body }

    return {
      data: ref(responseData) as Ref<T | null>,
      error: ref(null),
      pending: ref(false),
      refresh: async () => {},
      execute: async () => {},
      status: ref('success' as 'idle' | 'pending' | 'success' | 'error'),
      offline: true
    }
  }

  // Parse endpoint to determine entity type, ID, and operation
  const parseEndpoint = (endpoint: string, method: string): { entityType: string; entityId: string | null; operation: string } => {
    const cleanEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint

    // Parse patterns like "/items", "/items/123", "/areas/456"
    const parts = cleanEndpoint.split('/')

    // Map plural to singular entity types
    const pluralToSingular: Record<string, string> = {
      'items': 'item',
      'areas': 'area',
      'categories': 'category'
    }

    const entityType = pluralToSingular[parts[0]] || parts[0]

    let entityId: string | null = null
    let operation = 'create'

    if (parts.length > 1 && parts[1]) {
      entityId = parts[1]
      operation = method === 'DELETE' ? 'delete' : 'update'
    }

    console.log('Parsed endpoint:', { endpoint, method, entityType, entityId, operation })
    return { entityType, entityId, operation }
  }

  return {
    apiFetch
  }
}
