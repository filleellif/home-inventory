import type { AreaDto, CreateAreaDto, UpdateAreaDto, AreaTreeNode } from '~/types/area'

export const useAreas = () => {
  const store = useAreasStore()

  // Backward-compatible wrapper for fetchAreas
  const fetchAreas = async () => {
    // Ensure store is initialized
    if (!store.initialized) {
      await store.initFromIndexedDB()
    }

    return {
      data: computed(() => store.allAreas),
      error: computed(() => store.error),
      pending: computed(() => store.loading),
      refresh: () => store.refreshFromApi(),
    }
  }

  // Fetch single area by ID
  const fetchArea = async (id: string) => {
    // Ensure store is initialized
    if (!store.initialized) {
      await store.initFromIndexedDB()
    }

    return {
      data: computed(() => store.getById(id) || null),
      error: ref(null),
      pending: computed(() => store.loading),
    }
  }

  // Generate new QR code (delegates to API)
  const generateQrCode = async () => {
    const toast = useToast()
    try {
      const config = useRuntimeConfig()
      const authStore = useAuthStore()

      const headers: Record<string, string> = {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
      }

      if (authStore.token) {
        headers['Authorization'] = `Bearer ${authStore.token}`
      }

      const data = await $fetch<{ qrCode: string }>('/items/generate-qr', {
        baseURL: config.public.apiBase,
        method: 'POST',
        headers,
        credentials: 'include'
      })

      return { success: true, data }
    } catch (err: any) {
      toast.error(err.data?.message || 'Failed to generate QR code')
      return { success: false, error: err }
    }
  }

  // Create area - delegate to store
  const createArea = (area: CreateAreaDto) => store.create(area)

  // Update area - delegate to store
  const updateArea = (id: string, area: UpdateAreaDto) => store.update(id, area)

  // Delete area - delegate to store
  const deleteArea = (id: string) => store.delete(id)

  // Helper: Build area tree - delegate to store
  const buildAreaTree = (areas: AreaDto[]): AreaTreeNode[] => store.buildTree(areas)

  return {
    fetchAreas,
    fetchArea,
    generateQrCode,
    createArea,
    updateArea,
    deleteArea,
    buildAreaTree,
    // Direct store access for components that want it
    store,
  }
}
