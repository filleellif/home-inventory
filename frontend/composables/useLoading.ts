export const useLoading = () => {
  const isLoading = useState<boolean>('global-loading', () => false)
  const loadingCount = useState<number>('loading-count', () => 0)

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

  return {
    isLoading: readonly(isLoading),
    startLoading,
    stopLoading,
  }
}
