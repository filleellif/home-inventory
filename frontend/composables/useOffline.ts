export const useOffline = () => {
  const isOnline = useState('isOnline', () => true)
  const lastOnlineTime = useState('lastOnlineTime', () => Date.now())

  // Initialize online status
  const init = () => {
    if (process.client) {
      isOnline.value = navigator.onLine

      // Listen to browser events
      window.addEventListener('online', handleOnline)
      window.addEventListener('offline', handleOffline)

      // Periodic connectivity check (every 30 seconds)
      const intervalId = setInterval(checkConnectivity, 30000)

      // Cleanup on unmount
      onBeforeUnmount(() => {
        window.removeEventListener('online', handleOnline)
        window.removeEventListener('offline', handleOffline)
        clearInterval(intervalId)
      })
    }
  }

  const handleOnline = () => {
    isOnline.value = true
    lastOnlineTime.value = Date.now()

    // Trigger sync when coming back online
    if (process.client) {
      triggerSync()
    }
  }

  const handleOffline = () => {
    isOnline.value = false
  }

  const checkConnectivity = async () => {
    if (!process.client) return

    try {
      const config = useRuntimeConfig()
      const baseURL = config.public.apiBase

      // Ping health endpoint with HEAD request
      const response = await fetch(`${baseURL}/health`, {
        method: 'HEAD',
        cache: 'no-cache'
      })

      const wasOffline = !isOnline.value
      isOnline.value = response.ok

      // If we were offline and now we're online, trigger sync
      if (wasOffline && isOnline.value) {
        handleOnline()
      }
    } catch (error) {
      // Network error means we're offline
      if (isOnline.value) {
        isOnline.value = false
      }
    }
  }

  const triggerSync = () => {
    // Trigger sync queue processing
    // This will be called by useSyncQueue when it's loaded
    if (process.client && window) {
      window.dispatchEvent(new CustomEvent('sync-trigger'))
    }
  }

  return {
    isOnline: readonly(isOnline),
    lastOnlineTime: readonly(lastOnlineTime),
    checkConnectivity,
    triggerSync,
    init
  }
}
