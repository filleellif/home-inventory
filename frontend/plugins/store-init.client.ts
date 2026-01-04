export default defineNuxtPlugin(async (nuxtApp) => {
  // Only run on client
  if (!import.meta.client) return

  const areasStore = useAreasStore()
  const categoriesStore = useCategoriesStore()
  const itemsStore = useItemsStore()
  const { isOnline } = useOffline()
  const { stopBlockingLoading } = useLoading()

  // Blocking loading is already started by default in useLoading composable

  // Initialize stores from IndexedDB immediately
  // This provides instant data even before API calls
  let hasCache = false
  try {
    await Promise.all([
      areasStore.initFromIndexedDB(),
      categoriesStore.initFromIndexedDB(),
      itemsStore.initFromIndexedDB(),
    ])
    console.log('[store-init] Stores initialized from IndexedDB')
    // Check if we have any cached data
    hasCache = itemsStore.items.length > 0 || areasStore.areas.length > 0 || categoriesStore.categories.length > 0
  } catch (error) {
    console.error('[store-init] Failed to initialize stores:', error)
  }

  // If we have cached data, stop blocking - API refresh will happen in background
  // If no cache, keep blocking until API fetch completes
  if (hasCache) {
    stopBlockingLoading()
  }

  // If online, refresh from API
  if (isOnline.value) {
    setTimeout(async () => {
      try {
        await Promise.all([
          areasStore.refreshFromApi(),
          categoriesStore.refreshFromApi(),
          itemsStore.refreshFromApi(),
        ])
        console.log('[store-init] Stores refreshed from API')
      } catch (error) {
        console.error('[store-init] Failed to refresh stores from API:', error)
      } finally {
        // If we didn't have cache, now we can stop blocking
        if (!hasCache) {
          stopBlockingLoading()
        }
      }
    }, hasCache ? 1000 : 0) // No delay if no cache - fetch immediately
  } else if (!hasCache) {
    // Offline with no cache - stop blocking, user will see empty state
    stopBlockingLoading()
  }

  // Re-sync when coming back online (background refresh, no blocking)
  watch(isOnline, async (online) => {
    if (online) {
      setTimeout(async () => {
        try {
          await Promise.all([
            areasStore.refreshFromApi(),
            categoriesStore.refreshFromApi(),
            itemsStore.refreshFromApi(),
          ])
          console.log('[store-init] Stores refreshed after coming online')
        } catch (error) {
          console.error('[store-init] Failed to refresh stores after coming online:', error)
        }
      }, 2000)
    }
  })
})
