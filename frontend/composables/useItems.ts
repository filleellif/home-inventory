import type { ItemDto, CreateItemDto, UpdateItemDto } from '~/types/item'
import type { PaginatedList } from '~/types/pagination'
import type { ItemFilters } from '~/types/api'

export const useItems = () => {
  const { apiFetch } = useApi()
  const toast = useToast()

  // Fetch paginated items
  const fetchItems = async (
    searchQuery = '',
    categoryId: string | undefined = undefined,
    pageNumber = 1,
    pageSize = 20
  ) => {
    const { isOnline } = useOffline()
    const { getItems } = useOfflineStorage()

    // If offline, get from IndexedDB
    if (!isOnline.value) {
      const items = await getItems({ searchQuery, categoryId })

      // Simple client-side pagination
      const start = (pageNumber - 1) * pageSize
      const end = start + pageSize
      const paginatedItems = items.slice(start, end)

      const paginatedResult: PaginatedList<ItemDto> = {
        items: paginatedItems,
        pageNumber,
        totalPages: Math.ceil(items.length / pageSize),
        totalCount: items.length,
        hasPreviousPage: pageNumber > 1,
        hasNextPage: end < items.length
      }

      return {
        data: ref(paginatedResult),
        error: ref(null),
        pending: ref(false),
        refresh: async () => {}
      }
    }

    // Online - normal API fetch
    const query: Record<string, any> = {
      searchQuery,
      pageNumber,
      pageSize,
    }

    if (categoryId) {
      query.categoryId = categoryId
    }

    const cacheKey = `items-${searchQuery}-${categoryId || 'all'}-${pageNumber}-${pageSize}`
    const { data, error, pending, refresh } = await apiFetch<PaginatedList<ItemDto>>(
      '/items',
      {
        query,
        key: cacheKey
      }
    )

    return { data, error, pending, refresh }
  }

  // Fetch single item
  const fetchItem = async (id: string) => {
    const { isOnline } = useOffline()
    const { getItem } = useOfflineStorage()

    // If offline, get from IndexedDB
    if (!isOnline.value) {
      const item = await getItem(id)

      return {
        data: ref(item || null),
        error: ref(item ? null : { message: 'Item not found offline' }),
        pending: ref(false)
      }
    }

    // Online - normal API fetch
    const { data, error, pending } = await apiFetch<ItemDto>(`/items/${id}`, {
      key: `item-${id}`
    })

    return { data, error, pending }
  }

  // Fetch items by QR code
  const fetchItemsByQrCode = async (qrCode: string) => {
    const { data, error, pending } = await apiFetch<ItemDto[]>(
      `/items/by-qrcode/${encodeURIComponent(qrCode)}`,
      {
        key: `items-qr-${qrCode}`
      }
    )

    return { data, error, pending }
  }

  // Create item
  const createItem = async (item: CreateItemDto) => {
    try {
      const result = await apiFetch<{ id: string }>('/items', {
        method: 'POST',
        body: item,
      })

      if (result.error?.value) {
        toast.error(result.error.value.data?.message || 'Failed to create item')
        return { success: false, error: result.error.value }
      }

      // Check if this was an offline operation
      if ((result as any).offline) {
        toast.info('Item saved offline. Will sync when online.')
        return { success: true, data: result.data.value, offline: true }
      }

      toast.success('Item created successfully')
      return { success: true, data: result.data.value }
    } catch (err) {
      toast.error('Failed to create item')
      return { success: false, error: err }
    }
  }

  // Update item
  const updateItem = async (id: string, item: UpdateItemDto) => {
    try {
      const result = await apiFetch(`/items/${id}`, {
        method: 'PUT',
        body: { ...item, id },
      })

      if (result.error?.value) {
        toast.error(result.error.value.data?.message || 'Failed to update item')
        return { success: false, error: result.error.value }
      }

      // Check if this was an offline operation
      if ((result as any).offline) {
        toast.info('Changes saved offline. Will sync when online.')
        return { success: true, offline: true }
      }

      toast.success('Item updated successfully')
      return { success: true }
    } catch (err) {
      toast.error('Failed to update item')
      return { success: false, error: err }
    }
  }

  // Delete item
  const deleteItem = async (id: string) => {
    try {
      const result = await apiFetch(`/items/${id}`, {
        method: 'DELETE',
      })

      if (result.error?.value) {
        toast.error(result.error.value.data?.message || 'Failed to delete item')
        return { success: false, error: result.error.value }
      }

      // Check if this was an offline operation
      if ((result as any).offline) {
        toast.info('Item deleted offline. Will sync when online.')
        return { success: true, offline: true }
      }

      toast.success('Item deleted successfully')
      return { success: true }
    } catch (err) {
      toast.error('Failed to delete item')
      return { success: false, error: err }
    }
  }

  return {
    fetchItems,
    fetchItem,
    fetchItemsByQrCode,
    createItem,
    updateItem,
    deleteItem,
  }
}
