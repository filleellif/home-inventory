import { defineStore } from 'pinia'
import { toRaw } from 'vue'
import type { ItemDto, CreateItemDto, UpdateItemDto } from '~/types/item'
import type { PaginatedList } from '~/types/pagination'
import type { ItemRecord } from '~/utils/db'
import { db } from '~/utils/db'

interface ItemFilters {
  search: string
  categoryId?: string
  areaId?: string
}

interface ItemsState {
  items: ItemRecord[]
  loading: boolean
  initialized: boolean
  error: string | null
  // Pagination state
  pagination: {
    pageNumber: number
    pageSize: number
  }
  // Current filters
  filters: ItemFilters
}

export const useItemsStore = defineStore('items', {
  state: (): ItemsState => ({
    items: [],
    loading: false,
    initialized: false,
    error: null,
    pagination: {
      pageNumber: 1,
      pageSize: 20,
    },
    filters: {
      search: '',
      categoryId: undefined,
      areaId: undefined,
    },
  }),

  getters: {
    // All items (excluding soft-deleted)
    allItems: (state): ItemDto[] =>
      state.items.filter(i => !i._deleted),

    // Filtered items based on current filters
    filteredItems(): ItemDto[] {
      return this.allItems.filter(item => {
        if (this.filters.categoryId && item.categoryId !== this.filters.categoryId) {
          return false
        }
        if (this.filters.areaId && item.areaId !== this.filters.areaId) {
          return false
        }
        if (this.filters.search) {
          const search = this.filters.search.toLowerCase()
          return item.name.toLowerCase().includes(search) ||
                 item.description?.toLowerCase().includes(search)
        }
        return true
      })
    },

    // Paginated filtered items
    paginatedItems(): ItemDto[] {
      const start = (this.pagination.pageNumber - 1) * this.pagination.pageSize
      const end = start + this.pagination.pageSize
      return this.filteredItems.slice(start, end)
    },

    // Total count of filtered items
    totalCount(): number {
      return this.filteredItems.length
    },

    // Total pages
    totalPages(): number {
      return Math.ceil(this.filteredItems.length / this.pagination.pageSize)
    },

    // Pagination info matching PaginatedList interface
    paginationInfo(): PaginatedList<ItemDto> {
      return {
        items: this.paginatedItems,
        pageNumber: this.pagination.pageNumber,
        pageSize: this.pagination.pageSize,
        totalCount: this.totalCount,
        totalPages: this.totalPages,
        hasPreviousPage: this.pagination.pageNumber > 1,
        hasNextPage: this.pagination.pageNumber < this.totalPages,
      }
    },

    // Get single item by ID
    getById: (state) => (id: string): ItemRecord | undefined =>
      state.items.find(i => i.id === id && !i._deleted),

    // Items by area (for area detail views)
    itemsByArea: (state) => (areaId: string): ItemDto[] =>
      state.items.filter(i => i.areaId === areaId && !i._deleted),

    // Items by category (for category detail views)
    itemsByCategory: (state) => (categoryId: string): ItemDto[] =>
      state.items.filter(i => i.categoryId === categoryId && !i._deleted),

    // Check if any items are unsynced
    hasUnsyncedChanges: (state): boolean =>
      state.items.some(i => !i._synced || i._localOnly),
  },

  actions: {
    // Set filters (resets pagination to page 1)
    setFilters(filters: Partial<ItemFilters>) {
      this.filters = { ...this.filters, ...filters }
      this.pagination.pageNumber = 1
    },

    // Clear all filters
    clearFilters() {
      this.filters = {
        search: '',
        categoryId: undefined,
        areaId: undefined,
      }
      this.pagination.pageNumber = 1
    },

    // Go to specific page
    goToPage(page: number) {
      if (page >= 1 && page <= this.totalPages) {
        this.pagination.pageNumber = page
      }
    },

    // Set page size
    setPageSize(size: number) {
      this.pagination.pageSize = size
      this.pagination.pageNumber = 1
    },

    // Initialize from IndexedDB (called by plugin)
    async initFromIndexedDB() {
      // IndexedDB is client-only
      if (import.meta.server) return
      if (this.initialized) return

      this.loading = true

      try {
        const allItems = await db.items.toArray()
        this.items = allItems
        this.initialized = true
      } catch (error: any) {
        this.error = error.message
        console.error('[items-store] Failed to initialize from IndexedDB:', error)
      } finally {
        this.loading = false
      }
    },

    // Refresh from API (when online)
    async refreshFromApi() {
      if (import.meta.server) return

      const { isOnline } = useOffline()
      if (!isOnline.value) return

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

        // Fetch all items (large page size to get everything)
        const result = await $fetch<PaginatedList<ItemDto>>('/items', {
          baseURL: config.public.apiBase,
          query: { pageNumber: 1, pageSize: 10000 },
          headers,
          credentials: 'include'
        })

        // Save to IndexedDB
        const records: ItemRecord[] = result.items.map(item => ({
          ...item,
          _synced: true,
          _localOnly: false,
          _deleted: false,
        }))
        await db.items.bulkPut(records)

        // Reload from IndexedDB to get consistent records
        this.items = await db.items.toArray()
      } catch (error: any) {
        console.error('[items-store] Failed to refresh from API:', error)
        // Don't set error state - we still have cached data
      }
    },

    // Reload from IndexedDB (called after sync completes)
    async reloadFromIndexedDB() {
      if (import.meta.server) return

      try {
        const freshData = await db.items.toArray()
        // Use splice to update array in place - preserves Vue's reactivity tracking
        this.items.splice(0, this.items.length, ...freshData)
      } catch (error: any) {
        console.error('[items-store] Failed to reload from IndexedDB:', error)
      }
    },

    // Create item
    async create(data: CreateItemDto): Promise<{ success: boolean; id?: string; offline?: boolean }> {
      const { isOnline } = useOffline()
      const { queueOperation } = useSyncQueue()
      const toast = useToast()

      const tempId = `temp_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
      const now = new Date().toISOString()

      const newItem: ItemRecord = {
        id: tempId,
        name: String(data.name || ''),
        description: data.description ? String(data.description) : undefined,
        quantity: Number(data.quantity) || 1,
        areaId: data.areaId ? String(data.areaId) : undefined,
        categoryId: data.categoryId ? String(data.categoryId) : undefined,
        photos: [],
        receipts: [],
        createdAt: now,
        updatedAt: now,
        _synced: false,
        _localOnly: !isOnline.value,
        _deleted: false,
      }

      // Optimistically update store - use spread to ensure reactivity
      this.items = [...this.items, newItem]

      // Save to IndexedDB - convert to plain object to avoid DataCloneError
      await db.items.put(JSON.parse(JSON.stringify(newItem)))

      if (!isOnline.value) {
        // Queue for sync - convert payload to plain object
        await queueOperation({
          entityType: 'item',
          entityId: tempId,
          operation: 'create',
          payload: JSON.parse(JSON.stringify(toRaw(data))),
          timestamp: Date.now(),
        })
        toast.info('Item saved offline. Will sync when online.')
        return { success: true, id: tempId, offline: true }
      }

      // Online: make API call
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

        const response = await $fetch<ItemDto>('/items', {
          baseURL: config.public.apiBase,
          method: 'POST',
          body: data,
          headers,
          credentials: 'include'
        })

        // Handle different response formats (could be full object or just { id: "..." })
        const responseData = typeof response === 'string' ? { id: response } : response

        // Update store with real data from server
        const idx = this.items.findIndex(i => i.id === tempId)
        // Merge existing item data with response (in case API doesn't return all fields)
        const existingItem = idx !== -1 ? this.items[idx] : newItem
        const updatedItem: ItemRecord = {
          ...existingItem,
          ...responseData,
          _synced: true,
          _localOnly: false,
          _deleted: false,
        }

        if (idx !== -1) {
          // Use splice for guaranteed reactivity
          this.items.splice(idx, 1, updatedItem)
        }

        // Update IndexedDB - remove temp, add real (convert to plain object to avoid DataCloneError)
        await db.items.delete(tempId)
        await db.items.put(JSON.parse(JSON.stringify(updatedItem)))

        toast.success('Item created successfully')
        return { success: true, id: updatedItem.id }
      } catch (error: any) {
        // Rollback optimistic update
        this.items = this.items.filter(i => i.id !== tempId)
        await db.items.delete(tempId)

        toast.error(error.data?.message || 'Failed to create item')
        return { success: false }
      }
    },

    // Update item
    async update(id: string, data: UpdateItemDto): Promise<{ success: boolean; offline?: boolean }> {
      const { isOnline } = useOffline()
      const { queueOperation } = useSyncQueue()
      const toast = useToast()

      const idx = this.items.findIndex(i => i.id === id)
      if (idx === -1) return { success: false }

      const oldItem = { ...this.items[idx] }
      const now = new Date().toISOString()

      // Optimistically update store
      this.items[idx] = {
        ...oldItem,
        name: data.name,
        description: data.description,
        quantity: data.quantity,
        areaId: data.areaId,
        categoryId: data.categoryId,
        updatedAt: now,
        _synced: false,
      }

      // Save to IndexedDB - convert to plain object
      await db.items.put(JSON.parse(JSON.stringify(this.items[idx])))

      if (!isOnline.value) {
        await queueOperation({
          entityType: 'item',
          entityId: id,
          operation: 'update',
          payload: JSON.parse(JSON.stringify(toRaw(data))),
          timestamp: Date.now(),
        })
        toast.info('Changes saved offline. Will sync when online.')
        return { success: true, offline: true }
      }

      // Online: make API call
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

        await $fetch(`/items/${id}`, {
          baseURL: config.public.apiBase,
          method: 'PUT',
          body: { ...data, id },
          headers,
          credentials: 'include'
        })

        // API doesn't return the updated entity, just mark as synced
        this.items[idx] = {
          ...this.items[idx],
          _synced: true,
          _localOnly: false,
        }
        await db.items.put(JSON.parse(JSON.stringify(this.items[idx])))

        toast.success('Item updated successfully')
        return { success: true }
      } catch (error: any) {
        // Rollback - ensure plain object for IndexedDB
        const plainOldItem = JSON.parse(JSON.stringify(oldItem))
        this.items[idx] = plainOldItem
        await db.items.put(plainOldItem)

        toast.error(error.data?.message || 'Failed to update item')
        return { success: false }
      }
    },

    // Delete item
    async delete(id: string): Promise<{ success: boolean; offline?: boolean }> {
      const { isOnline } = useOffline()
      const { queueOperation } = useSyncQueue()
      const toast = useToast()

      const idx = this.items.findIndex(i => i.id === id)
      if (idx === -1) return { success: false }

      const oldItem = { ...this.items[idx] }

      // Optimistically soft-delete in store
      this.items[idx] = {
        ...this.items[idx],
        _deleted: true,
        _synced: false,
      }

      // Soft delete in IndexedDB
      await db.items.update(id, { _deleted: true, _synced: false })

      if (!isOnline.value) {
        await queueOperation({
          entityType: 'item',
          entityId: id,
          operation: 'delete',
          payload: null,
          timestamp: Date.now(),
        })
        toast.info('Item deleted offline. Will sync when online.')
        return { success: true, offline: true }
      }

      // Online: make API call
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

        await $fetch(`/items/${id}`, {
          baseURL: config.public.apiBase,
          method: 'DELETE',
          headers,
          credentials: 'include'
        })

        // Hard delete after successful sync
        await db.items.delete(id)
        this.items = this.items.filter(i => i.id !== id)

        toast.success('Item deleted successfully')
        return { success: true }
      } catch (error: any) {
        // Rollback - ensure plain object for IndexedDB
        const plainOldItem = JSON.parse(JSON.stringify(oldItem))
        this.items[idx] = plainOldItem
        await db.items.put(plainOldItem)

        toast.error(error.data?.message || 'Failed to delete item')
        return { success: false }
      }
    },

    // Fetch items by QR code (special case - API only)
    async fetchByQrCode(qrCode: string): Promise<ItemDto[]> {
      const { isOnline } = useOffline()

      if (!isOnline.value) {
        // Search local items for matching QR code (if stored)
        // For now, return empty since QR codes aren't stored in items
        return []
      }

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

        return await $fetch<ItemDto[]>(`/items/by-qrcode/${encodeURIComponent(qrCode)}`, {
          baseURL: config.public.apiBase,
          headers,
          credentials: 'include'
        })
      } catch (error: any) {
        console.error('[items-store] Failed to fetch by QR code:', error)
        return []
      }
    },
  },
})
