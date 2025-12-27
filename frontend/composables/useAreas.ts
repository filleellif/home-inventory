import type { AreaDto, CreateAreaDto, UpdateAreaDto, AreaTreeNode } from '~/types/area'

export const useAreas = () => {
  const { apiFetch } = useApi()
  const toast = useToast()

  // Fetch all areas
  const fetchAreas = async () => {
    const { data, error, pending, refresh } = await apiFetch<AreaDto[]>('/areas', {
      key: 'areas'
    })

    return { data, error, pending, refresh }
  }

  // Fetch single area
  const fetchArea = async (id: string) => {
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
      const { data, error } = await apiFetch<{ id: string }>('/areas', {
        method: 'POST',
        body: area,
      })

      if (error.value) {
        toast.error(error.value.data?.message || 'Failed to create area')
        return { success: false, error: error.value }
      }

      // Invalidate the areas cache to ensure the list is refreshed
      refreshNuxtData('areas')

      toast.success('Area created successfully')
      return { success: true, data: data.value }
    } catch (err) {
      toast.error('Failed to create area')
      return { success: false, error: err }
    }
  }

  // Update area
  const updateArea = async (id: string, area: UpdateAreaDto) => {
    try {
      const { error } = await apiFetch(`/areas/${id}`, {
        method: 'PUT',
        body: { ...area, id },
      })

      if (error.value) {
        toast.error(error.value.data?.message || 'Failed to update area')
        return { success: false, error: error.value }
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
      const { error } = await apiFetch(`/areas/${id}`, {
        method: 'DELETE',
      })

      if (error.value) {
        toast.error(error.value.data?.message || 'Failed to delete area')
        return { success: false, error: error.value }
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
