export const useToast = () => {
  const toasts = useState<Array<{ id: number; message: string; type: string }>>('toasts', () => [])

  let id = 0

  const addToast = (message: string, type: 'success' | 'error' | 'info' = 'info') => {
    const toastId = id++
    toasts.value.push({ id: toastId, message, type })

    setTimeout(() => {
      removeToast(toastId)
    }, 5000)
  }

  const removeToast = (id: number) => {
    const index = toasts.value.findIndex(t => t.id === id)
    if (index > -1) {
      toasts.value.splice(index, 1)
    }
  }

  return {
    toasts: readonly(toasts),
    success: (message: string) => addToast(message, 'success'),
    error: (message: string) => addToast(message, 'error'),
    info: (message: string) => addToast(message, 'info'),
    remove: removeToast,
  }
}
