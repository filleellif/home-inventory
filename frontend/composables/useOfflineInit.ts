export const useOfflineInit = () => {
  const { isOnline } = useOffline()
  const { saveItemsBulk, saveAreasBulk, saveCategoriesBulk, getFromCache, saveToCache } = useOfflineStorage()
  const isInitialized = useState('offlineInitialized', () => false)
  const isInitializing = useState('offlineInitializing', () => false)

  const initializeOfflineData = async () => {
    // Don't initialize if already done or in progress
    if (isInitialized.value || isInitializing.value) {
      return
    }

    // Check if we've already initialized before
    const lastInitTime = await getFromCache('lastOfflineInit')
    if (lastInitTime) {
      const hoursSinceInit = (Date.now() - parseInt(lastInitTime)) / (1000 * 60 * 60)
      // Re-sync if it's been more than 24 hours
      if (hoursSinceInit < 24) {
        isInitialized.value = true
        return
      }
    }

    // Only initialize if online
    if (!isOnline.value) {
      console.log('Skipping offline data initialization - no connection')
      return
    }

    isInitializing.value = true

    try {
      console.log('Initializing offline data...')

      const config = useRuntimeConfig()
      const baseURL = config.public.apiBase

      // Fetch all data in parallel
      const [areasResult, categoriesResult, itemsResult] = await Promise.all([
        $fetch('/areas', { baseURL }).catch(err => {
          console.error('Failed to fetch areas:', err)
          return []
        }),
        $fetch('/categories', { baseURL }).catch(err => {
          console.error('Failed to fetch categories:', err)
          return []
        }),
        $fetch('/items', {
          baseURL,
          query: { pageNumber: 1, pageSize: 10000 } // Large page size to get all items
        }).catch(err => {
          console.error('Failed to fetch items:', err)
          return { items: [] }
        })
      ])

      // Save to IndexedDB
      const areas = Array.isArray(areasResult) ? areasResult : []
      const categories = Array.isArray(categoriesResult) ? categoriesResult : []
      const items = Array.isArray((itemsResult as any)?.items) ? (itemsResult as any).items : []

      console.log(`Caching ${areas.length} areas, ${categories.length} categories, ${items.length} items`)

      await Promise.all([
        saveAreasBulk(areas, true),
        saveCategoriesBulk(categories, true),
        saveItemsBulk(items, true)
      ])

      // Save initialization timestamp
      await saveToCache('lastOfflineInit', Date.now().toString())

      isInitialized.value = true
      console.log('Offline data initialization complete')

      // Show success toast
      const toast = useToast()
      toast.success(`${items.length} items cached for offline use`)

    } catch (error) {
      console.error('Failed to initialize offline data:', error)
      const toast = useToast()
      toast.error('Failed to cache data for offline use')
    } finally {
      isInitializing.value = false
    }
  }

  // Force refresh offline data
  const refreshOfflineData = async () => {
    isInitialized.value = false
    await initializeOfflineData()
  }

  return {
    initializeOfflineData,
    refreshOfflineData,
    isInitialized: readonly(isInitialized),
    isInitializing: readonly(isInitializing)
  }
}
