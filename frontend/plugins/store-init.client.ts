export default defineNuxtPlugin(async (nuxtApp) => {
  // Only run on client
  if (!import.meta.client) return

  const areasStore = useAreasStore()
  const categoriesStore = useCategoriesStore()
  const itemsStore = useItemsStore()
  const { isOnline } = useOffline()

  // Initialize stores from IndexedDB immediately
  // This provides instant data even before API calls
  try {
    await Promise.all([
      areasStore.initFromIndexedDB(),
      categoriesStore.initFromIndexedDB(),
      itemsStore.initFromIndexedDB(),
    ])
    console.log('[store-init] Stores initialized from IndexedDB')
  } catch (error) {
    console.error('[store-init] Failed to initialize stores:', error)
  }

  // If online, refresh from API in background after initial render
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
      }
    }, 1000)
  }

  // Re-sync when coming back online
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
