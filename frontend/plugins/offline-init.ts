export default defineNuxtPlugin((nuxtApp) => {
  if (!process.client) return

  const { isOnline } = useOffline()
  const { saveItemsBulk, saveAreasBulk, saveCategoriesBulk, getFromCache, saveToCache } = useOfflineStorage()

  const initializeOfflineData = async () => {
    // Only run if online
    if (!isOnline.value) {
      console.log('Offline - skipping data pre-cache')
      return
    }

    // Check if we've cached data in the last 24 hours
    const lastCacheTime = await getFromCache('lastOfflineInit')
    if (lastCacheTime) {
      const hoursSinceCache = (Date.now() - parseInt(lastCacheTime)) / (1000 * 60 * 60)
      if (hoursSinceCache < 24) {
        console.log('Data already cached recently')
        return
      }
    }

    console.log('Pre-caching all data for offline use...')

    try {
      const config = useRuntimeConfig()
      const baseURL = config.public.apiBase

      // Fetch all data in parallel
      const [areasResult, categoriesResult, itemsResult] = await Promise.all([
        $fetch('/areas', { baseURL }),
        $fetch('/categories', { baseURL }),
        $fetch('/items', { baseURL, query: { pageNumber: 1, pageSize: 10000 } })
      ])

      const areas = Array.isArray(areasResult) ? areasResult : []
      const categories = Array.isArray(categoriesResult) ? categoriesResult : []
      const items = Array.isArray((itemsResult as any)?.items) ? (itemsResult as any).items : []

      // Save all data to IndexedDB
      await Promise.all([
        saveAreasBulk(areas, true),
        saveCategoriesBulk(categories, true),
        saveItemsBulk(items, true)
      ])

      // Update cache timestamp
      await saveToCache('lastOfflineInit', Date.now().toString())

      console.log(`✓ Cached ${areas.length} areas, ${categories.length} categories, ${items.length} items for offline use`)

      // Show success toast
      const toast = useToast()
      toast.success(`App ready for offline use (${items.length} items cached)`)
    } catch (error) {
      console.error('Failed to pre-cache data:', error)
      // Don't show error toast - might not be critical
    }
  }

  // Initialize when app is mounted
  nuxtApp.hook('app:mounted', () => {
    // Slight delay to avoid blocking initial render
    setTimeout(() => {
      initializeOfflineData()
    }, 1000)

    // Re-cache when coming back online
    watch(isOnline, (online) => {
      if (online) {
        setTimeout(() => {
          initializeOfflineData()
        }, 2000)
      }
    })
  })
})
