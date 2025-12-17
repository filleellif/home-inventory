import type { ItemDto, CreateItemDto, UpdateItemDto } from '~/types/item'
import type { PaginatedList } from '~/types/pagination'
import type { ItemFilters } from '~/types/api'

export const useItems = () => {
  const { apiFetch } = useApi()
  const toast = useToast()

  // Fetch paginated items
  const fetchItems = async (
    pageNumber = 1,
    pageSize = 20,
    filters?: ItemFilters
  ) => {
    const query: Record<string, any> = {
      pageNumber,
      pageSize,
    }

    // Add filters to query (server-side filtering to be implemented)
    if (filters?.search) query.search = filters.search
    if (filters?.categoryId) query.categoryId = filters.categoryId
    if (filters?.room) query.room = filters.room

    const { data, error, pending, refresh } = await apiFetch<PaginatedList<ItemDto>>(
      '/items',
      {
        query,
        key: `items-${pageNumber}-${pageSize}-${JSON.stringify(filters)}`
      }
    )

    return { data, error, pending, refresh }
  }

  // Fetch single item
  const fetchItem = async (id: string) => {
    const { data, error, pending } = await apiFetch<ItemDto>(`/items/${id}`, {
      key: `item-${id}`
    })

    return { data, error, pending }
  }

  // Create item
  const createItem = async (item: CreateItemDto) => {
    try {
      const { data, error } = await apiFetch<{ id: string }>('/items', {
        method: 'POST',
        body: item,
      })

      if (error.value) {
        toast.error(error.value.data?.message || 'Failed to create item')
        return { success: false, error: error.value }
      }

      toast.success('Item created successfully')
      return { success: true, data: data.value }
    } catch (err) {
      toast.error('Failed to create item')
      return { success: false, error: err }
    }
  }

  // Update item
  const updateItem = async (id: string, item: UpdateItemDto) => {
    try {
      const { error } = await apiFetch(`/items/${id}`, {
        method: 'PUT',
        body: { ...item, id },
      })

      if (error.value) {
        toast.error(error.value.data?.message || 'Failed to update item')
        return { success: false, error: error.value }
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
      const { error } = await apiFetch(`/items/${id}`, {
        method: 'DELETE',
      })

      if (error.value) {
        toast.error(error.value.data?.message || 'Failed to delete item')
        return { success: false, error: error.value }
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
    createItem,
    updateItem,
    deleteItem,
  }
}
