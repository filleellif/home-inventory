import type { ItemDto, CreateItemDto, UpdateItemDto } from '~/types/item'
import type { PaginatedList } from '~/types/pagination'

export const useItems = () => {
  const store = useItemsStore()

  // Get raw items data (for use with useAsyncData) - returns paginated results
  const getItemsData = async (
    searchQuery = '',
    categoryId: string | undefined = undefined,
    pageNumber = 1,
    pageSize = 20
  ): Promise<PaginatedList<ItemDto> | null> => {
    // Ensure store is initialized
    if (!store.initialized) {
      await store.initFromIndexedDB()
    }

    // Update store filters and pagination
    store.setFilters({ search: searchQuery, categoryId })
    store.pagination.pageNumber = pageNumber
    store.pagination.pageSize = pageSize

    return store.paginationInfo
  }

  // Fetch paginated items - backward compatible wrapper
  const fetchItems = async (
    searchQuery = '',
    categoryId: string | undefined = undefined,
    pageNumber = 1,
    pageSize = 20
  ) => {
    // Ensure store is initialized
    if (!store.initialized) {
      await store.initFromIndexedDB()
    }

    // Update store filters and pagination
    store.setFilters({ search: searchQuery, categoryId })
    store.pagination.pageNumber = pageNumber
    store.pagination.pageSize = pageSize

    return {
      data: computed(() => store.paginationInfo),
      error: computed(() => store.error),
      pending: computed(() => store.loading),
      refresh: () => store.refreshFromApi(),
    }
  }

  // Fetch single item
  const fetchItem = async (id: string) => {
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

  // Fetch items by QR code - delegate to store
  const fetchItemsByQrCode = async (qrCode: string) => {
    const items = await store.fetchByQrCode(qrCode)
    return {
      data: ref(items),
      error: ref(null),
      pending: ref(false),
    }
  }

  // Create item - delegate to store
  const createItem = (item: CreateItemDto) => store.create(item)

  // Update item - delegate to store
  const updateItem = (id: string, item: UpdateItemDto) => store.update(id, item)

  // Delete item - delegate to store
  const deleteItem = (id: string) => store.delete(id)

  return {
    getItemsData,
    fetchItems,
    fetchItem,
    fetchItemsByQrCode,
    createItem,
    updateItem,
    deleteItem,
    // Direct store access for components that want it
    store,
  }
}
