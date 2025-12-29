import type { AreaDto, CreateAreaDto, UpdateAreaDto, AreaTreeNode } from '~/types/area'

export const useAreas = () => {
  const { apiFetch } = useApi()
  const toast = useToast()

  // Fetch all areas
  const fetchAreas = async () => {
    const { isOnline } = useOffline()
    const { getAreas } = useOfflineStorage()

    // If offline, get from IndexedDB
    if (!isOnline.value) {
      const areas = await getAreas()

      return {
        data: ref(areas),
        error: ref(null),
        pending: ref(false),
        refresh: async () => {}
      }
    }

    // Online - normal API fetch
    const { data, error, pending, refresh } = await apiFetch<AreaDto[]>('/areas', {
      key: 'areas'
    })

    return { data, error, pending, refresh }
  }

  // Fetch single area
  const fetchArea = async (id: string) => {
    const { isOnline } = useOffline()
    const { getArea } = useOfflineStorage()

    // If offline, get from IndexedDB
    if (!isOnline.value) {
      const area = await getArea(id)

      return {
        data: ref(area || null),
        error: ref(area ? null : { message: 'Area not found offline' }),
        pending: ref(false)
      }
    }

    // Online - normal API fetch
    const { data, error, pending } = await apiFetch<AreaDto>(`/areas/${id}`, {
      key: `area-${id}`
    })

    return { data, error, pending }
  }

  // Generate new QR code
  const generateQrCode = async () => {
    try {
      const { data, error } = await apiFetch<{ qrCode: string }>('/items/generate-qr', {
        method: 'POST',
      })

      if (error.value) {
        toast.error(error.value.data?.message || 'Failed to generate QR code')
        return { success: false, error: error.value }
      }

      return { success: true, data: data.value }
    } catch (err) {
      toast.error('Failed to generate QR code')
      return { success: false, error: err }
    }
  }

  // Create area
  const createArea = async (area: CreateAreaDto) => {
    try {
      const result = await apiFetch<{ id: string }>('/areas', {
        method: 'POST',
        body: area,
      })

      if (result.error?.value) {
        toast.error(result.error.value.data?.message || 'Failed to create area')
        return { success: false, error: result.error.value }
      }

      // Check if this was an offline operation
      if ((result as any).offline) {
        toast.info('Area saved offline. Will sync when online.')
        refreshNuxtData('areas')
        return { success: true, data: result.data.value, offline: true }
      }

      // Invalidate the areas cache to ensure the list is refreshed
      refreshNuxtData('areas')

      toast.success('Area created successfully')
      return { success: true, data: result.data.value }
    } catch (err) {
      toast.error('Failed to create area')
      return { success: false, error: err }
    }
  }

  // Update area
  const updateArea = async (id: string, area: UpdateAreaDto) => {
    try {
      const result = await apiFetch(`/areas/${id}`, {
        method: 'PUT',
        body: { ...area, id },
      })

      if (result.error?.value) {
        toast.error(result.error.value.data?.message || 'Failed to update area')
        return { success: false, error: result.error.value }
      }

      // Check if this was an offline operation
      if ((result as any).offline) {
        toast.info('Changes saved offline. Will sync when online.')
        refreshNuxtData('areas')
        refreshNuxtData(`area-${id}`)
        return { success: true, offline: true }
      }

      // Invalidate the areas cache to ensure the list is refreshed
      refreshNuxtData('areas')
      refreshNuxtData(`area-${id}`)

      toast.success('Area updated successfully')
      return { success: true }
    } catch (err) {
      toast.error('Failed to update area')
      return { success: false, error: err }
    }
  }

  // Delete area
  const deleteArea = async (id: string) => {
    try {
      const result = await apiFetch(`/areas/${id}`, {
        method: 'DELETE',
      })

      if (result.error?.value) {
        toast.error(result.error.value.data?.message || 'Failed to delete area')
        return { success: false, error: result.error.value }
      }

      // Check if this was an offline operation
      if ((result as any).offline) {
        toast.info('Area deleted offline. Will sync when online.')
        refreshNuxtData('areas')
        return { success: true, offline: true }
      }

      // Invalidate the areas cache to ensure the list is refreshed
      refreshNuxtData('areas')

      toast.success('Area deleted successfully')
      return { success: true }
    } catch (err) {
      toast.error('Failed to delete area')
      return { success: false, error: err }
    }
  }

  // Helper: Build area tree for hierarchical display
  const buildAreaTree = (areas: AreaDto[]): AreaTreeNode[] => {
    const areaMap = new Map(
      areas.map(area => [area.id, { ...area, children: [] as AreaTreeNode[] }])
    )
    const rootAreas: AreaTreeNode[] = []

    areas.forEach(area => {
      const areaWithChildren = areaMap.get(area.id)!

      if (area.parentAreaId) {
        const parent = areaMap.get(area.parentAreaId)
        if (parent) {
          parent.children.push(areaWithChildren)
        } else {
          rootAreas.push(areaWithChildren)
        }
      } else {
        rootAreas.push(areaWithChildren)
      }
    })

    return rootAreas
  }

  return {
    fetchAreas,
    fetchArea,
    generateQrCode,
    createArea,
    updateArea,
    deleteArea,
    buildAreaTree,
  }
}
