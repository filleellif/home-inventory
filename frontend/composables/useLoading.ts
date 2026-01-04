export const useLoading = () => {
  const isLoading = useState<boolean>('global-loading', () => false)
  const loadingCount = useState<number>('loading-count', () => 0)

  // Blocking loading state - shows full-screen overlay
  // Starts as true so overlay shows immediately before stores are initialized
  const isBlockingLoading = useState<boolean>('blocking-loading', () => true)
  const blockingLoadingCount = useState<number>('blocking-loading-count', () => 1)
  const blockingLoadingMessage = useState<string>('blocking-loading-message', () => 'Loading...')

  const startLoading = () => {
    loadingCount.value++
    isLoading.value = true
  }

  const stopLoading = () => {
    loadingCount.value = Math.max(0, loadingCount.value - 1)
    if (loadingCount.value === 0) {
      isLoading.value = false
    }
  }

  const startBlockingLoading = (message: string = 'Loading...') => {
    blockingLoadingCount.value++
    blockingLoadingMessage.value = message
    isBlockingLoading.value = true
  }

  const stopBlockingLoading = () => {
    blockingLoadingCount.value = Math.max(0, blockingLoadingCount.value - 1)
    if (blockingLoadingCount.value === 0) {
      isBlockingLoading.value = false
      blockingLoadingMessage.value = ''
    }
  }

  return {
    isLoading: readonly(isLoading),
    startLoading,
    stopLoading,
    isBlockingLoading: readonly(isBlockingLoading),
    blockingLoadingMessage: readonly(blockingLoadingMessage),
    startBlockingLoading,
    stopBlockingLoading,
  }
}
