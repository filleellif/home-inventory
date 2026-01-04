import { defineStore } from 'pinia'
import { toRaw } from 'vue'
import type { CategoryDto, CreateCategoryDto, CategoryTreeNode } from '~/types/category'
import type { CategoryRecord } from '~/utils/db'
import { db } from '~/utils/db'

interface CategoriesState {
  categories: CategoryRecord[]
  loading: boolean
  initialized: boolean
  error: string | null
}

export const useCategoriesStore = defineStore('categories', {
  state: (): CategoriesState => ({
    categories: [],
    loading: false,
    initialized: false,
    error: null,
  }),

  getters: {
    // All categories (excluding soft-deleted)
    allCategories: (state): CategoryDto[] =>
      state.categories.filter(c => !c._deleted),

    // Build hierarchical tree structure
    categoryTree(): CategoryTreeNode[] {
      return this.buildTree(this.allCategories)
    },

    // Flattened tree for select dropdowns (preserves hierarchy order with indentation info)
    flattenedCategories(): (CategoryDto & { depth: number })[] {
      const result: (CategoryDto & { depth: number })[] = []
      const flatten = (nodes: CategoryTreeNode[], depth = 0) => {
        nodes.forEach(node => {
          result.push({ ...node, depth })
          if (node.children?.length) flatten(node.children, depth + 1)
        })
      }
      flatten(this.categoryTree)
      return result
    },

    // Get single category by ID
    getById: (state) => (id: string): CategoryRecord | undefined =>
      state.categories.find(c => c.id === id && !c._deleted),

    // Check if any categories are unsynced
    hasUnsyncedChanges: (state): boolean =>
      state.categories.some(c => !c._synced || c._localOnly),
  },

  actions: {
    // Tree builder
    buildTree(categories: CategoryDto[]): CategoryTreeNode[] {
      // Filter out any invalid categories (missing id or name)
      const validCategories = categories.filter(cat => cat && cat.id && cat.name)

      const categoryMap = new Map(
        validCategories.map(cat => [cat.id, { ...cat, children: [] as CategoryTreeNode[] }])
      )
      const rootCategories: CategoryTreeNode[] = []

      validCategories.forEach(category => {
        const node = categoryMap.get(category.id)!
        if (category.parentCategoryId) {
          const parent = categoryMap.get(category.parentCategoryId)
          if (parent) parent.children.push(node)
          else rootCategories.push(node)
        } else {
          rootCategories.push(node)
        }
      })

      const sortByName = (a: CategoryTreeNode, b: CategoryTreeNode) =>
        (a.name || '').localeCompare(b.name || '', undefined, { sensitivity: 'base' })

      const sortChildren = (nodes: CategoryTreeNode[]) => {
        nodes.sort(sortByName)
        nodes.forEach(n => n.children.length && sortChildren(n.children))
      }

      sortChildren(rootCategories)
      return rootCategories
    },

    // Initialize from IndexedDB (called by plugin)
    async initFromIndexedDB() {
      // IndexedDB is client-only
      if (import.meta.server) return
      if (this.initialized) return

      this.loading = true

      try {
        const allCategories = await db.categories.toArray()
        this.categories = allCategories
        this.initialized = true
      } catch (error: any) {
        this.error = error.message
        console.error('[categories-store] Failed to initialize from IndexedDB:', error)
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

        const categories = await $fetch<CategoryDto[]>('/categories', {
          baseURL: config.public.apiBase,
          headers,
          credentials: 'include'
        })

        // Save to IndexedDB
        const records: CategoryRecord[] = categories.map(cat => ({
          ...cat,
          _synced: true,
          _localOnly: false,
          _deleted: false,
        }))
        await db.categories.bulkPut(records)

        // Reload from IndexedDB to get consistent records
        this.categories = await db.categories.toArray()
      } catch (error: any) {
        console.error('[categories-store] Failed to refresh from API:', error)
        // Don't set error state - we still have cached data
      }
    },

    // Reload from IndexedDB (called after sync completes)
    async reloadFromIndexedDB() {
      if (import.meta.server) return

      try {
        const freshData = await db.categories.toArray()
        // Use splice to update array in place - preserves Vue's reactivity tracking
        this.categories.splice(0, this.categories.length, ...freshData)
      } catch (error: any) {
        console.error('[categories-store] Failed to reload from IndexedDB:', error)
      }
    },

    // Create category
    async create(data: CreateCategoryDto): Promise<{ success: boolean; id?: string; offline?: boolean }> {
      const { isOnline } = useOffline()
      const { queueOperation } = useSyncQueue()
      const toast = useToast()

      const tempId = `temp_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
      const now = new Date().toISOString()

      const newCategory: CategoryRecord = {
        id: tempId,
        name: data.name,
        description: data.description,
        parentCategoryId: data.parentCategoryId,
        createdAt: now,
        updatedAt: now,
        _synced: false,
        _localOnly: !isOnline.value,
        _deleted: false,
      }

      // Optimistically update store
      this.categories.push(newCategory)

      // Save to IndexedDB - convert to plain object to avoid DataCloneError
      await db.categories.put(JSON.parse(JSON.stringify(newCategory)))

      if (!isOnline.value) {
        // Queue for sync - convert payload to plain object
        await queueOperation({
          entityType: 'category',
          entityId: tempId,
          operation: 'create',
          payload: JSON.parse(JSON.stringify(toRaw(data))),
          timestamp: Date.now(),
        })
        toast.info('Category saved offline. Will sync when online.')
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

        const response = await $fetch<CategoryDto>('/categories', {
          baseURL: config.public.apiBase,
          method: 'POST',
          body: data,
          headers,
          credentials: 'include'
        })

        // Update store with real data from server
        const idx = this.categories.findIndex(c => c.id === tempId)
        if (idx !== -1) {
          this.categories[idx] = {
            ...response,
            _synced: true,
            _localOnly: false,
            _deleted: false,
          }
        }

        // Update IndexedDB - remove temp, add real
        await db.categories.delete(tempId)
        await db.categories.put({
          ...response,
          _synced: true,
          _localOnly: false,
          _deleted: false,
        })

        toast.success('Category created successfully')
        return { success: true, id: response.id }
      } catch (error: any) {
        // Rollback optimistic update
        this.categories = this.categories.filter(c => c.id !== tempId)
        await db.categories.delete(tempId)

        toast.error(error.data?.message || 'Failed to create category')
        return { success: false }
      }
    },

    // Update category
    async update(id: string, data: CreateCategoryDto): Promise<{ success: boolean; offline?: boolean }> {
      const { isOnline } = useOffline()
      const { queueOperation } = useSyncQueue()
      const toast = useToast()

      const idx = this.categories.findIndex(c => c.id === id)
      if (idx === -1) return { success: false }

      const oldCategory = { ...this.categories[idx] }
      const now = new Date().toISOString()

      // Optimistically update store
      this.categories[idx] = {
        ...oldCategory,
        name: data.name,
        description: data.description,
        parentCategoryId: data.parentCategoryId,
        updatedAt: now,
        _synced: false,
      }

      // Save to IndexedDB - convert to plain object
      await db.categories.put(JSON.parse(JSON.stringify(this.categories[idx])))

      if (!isOnline.value) {
        await queueOperation({
          entityType: 'category',
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

        const response = await $fetch<CategoryDto>(`/categories/${id}`, {
          baseURL: config.public.apiBase,
          method: 'PUT',
          body: { ...data, id },
          headers,
          credentials: 'include'
        })

        // Update with server response
        this.categories[idx] = {
          ...response,
          _synced: true,
          _localOnly: false,
          _deleted: false,
        }
        await db.categories.put(this.categories[idx])

        toast.success('Category updated successfully')
        return { success: true }
      } catch (error: any) {
        // Rollback
        this.categories[idx] = oldCategory
        await db.categories.put(oldCategory)

        toast.error(error.data?.message || 'Failed to update category')
        return { success: false }
      }
    },

    // Delete category
    async delete(id: string): Promise<{ success: boolean; offline?: boolean }> {
      const { isOnline } = useOffline()
      const { queueOperation } = useSyncQueue()
      const toast = useToast()

      const idx = this.categories.findIndex(c => c.id === id)
      if (idx === -1) return { success: false }

      const oldCategory = { ...this.categories[idx] }

      // Optimistically soft-delete in store
      this.categories[idx] = {
        ...this.categories[idx],
        _deleted: true,
        _synced: false,
      }

      // Soft delete in IndexedDB
      await db.categories.update(id, { _deleted: true, _synced: false })

      if (!isOnline.value) {
        await queueOperation({
          entityType: 'category',
          entityId: id,
          operation: 'delete',
          payload: null,
          timestamp: Date.now(),
        })
        toast.info('Category deleted offline. Will sync when online.')
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

        await $fetch(`/categories/${id}`, {
          baseURL: config.public.apiBase,
          method: 'DELETE',
          headers,
          credentials: 'include'
        })

        // Hard delete after successful sync
        await db.categories.delete(id)
        this.categories = this.categories.filter(c => c.id !== id)

        toast.success('Category deleted successfully')
        return { success: true }
      } catch (error: any) {
        // Rollback
        this.categories[idx] = oldCategory
        await db.categories.put(oldCategory)

        toast.error(error.data?.message || 'Failed to delete category')
        return { success: false }
      }
    },
  },
})
