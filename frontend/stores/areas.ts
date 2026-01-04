import { defineStore } from 'pinia'
import { toRaw } from 'vue'
import type { AreaDto, CreateAreaDto, UpdateAreaDto, AreaTreeNode } from '~/types/area'
import type { AreaRecord } from '~/utils/db'
import { db } from '~/utils/db'

interface AreasState {
  areas: AreaRecord[]
  loading: boolean
  initialized: boolean
  error: string | null
}

export const useAreasStore = defineStore('areas', {
  state: (): AreasState => ({
    areas: [],
    loading: false,
    initialized: false,
    error: null,
  }),

  getters: {
    // All areas (excluding soft-deleted)
    allAreas: (state): AreaDto[] =>
      state.areas.filter(a => !a._deleted),

    // Build hierarchical tree structure
    areaTree(): AreaTreeNode[] {
      return this.buildTree(this.allAreas)
    },

    // Flattened tree for select dropdowns (preserves hierarchy order with indentation info)
    flattenedAreas(): (AreaDto & { depth: number })[] {
      const result: (AreaDto & { depth: number })[] = []
      const flatten = (nodes: AreaTreeNode[], depth = 0) => {
        nodes.forEach(node => {
          result.push({ ...node, depth })
          if (node.children?.length) flatten(node.children, depth + 1)
        })
      }
      flatten(this.areaTree)
      return result
    },

    // Get single area by ID
    getById: (state) => (id: string): AreaRecord | undefined =>
      state.areas.find(a => a.id === id && !a._deleted),

    // Check if any areas are unsynced
    hasUnsyncedChanges: (state): boolean =>
      state.areas.some(a => !a._synced || a._localOnly),
  },

  actions: {
    // Tree builder
    buildTree(areas: AreaDto[]): AreaTreeNode[] {
      // Filter out any invalid areas (missing id or name)
      const validAreas = areas.filter(area => area && area.id && area.name)

      const areaMap = new Map(
        validAreas.map(area => [area.id, { ...area, children: [] as AreaTreeNode[] }])
      )
      const rootAreas: AreaTreeNode[] = []

      validAreas.forEach(area => {
        const node = areaMap.get(area.id)!
        if (area.parentAreaId) {
          const parent = areaMap.get(area.parentAreaId)
          if (parent) parent.children.push(node)
          else rootAreas.push(node)
        } else {
          rootAreas.push(node)
        }
      })

      const sortByName = (a: AreaTreeNode, b: AreaTreeNode) =>
        (a.name || '').localeCompare(b.name || '', undefined, { sensitivity: 'base' })

      const sortChildren = (nodes: AreaTreeNode[]) => {
        nodes.sort(sortByName)
        nodes.forEach(n => n.children.length && sortChildren(n.children))
      }

      sortChildren(rootAreas)
      return rootAreas
    },

    // Initialize from IndexedDB (called by plugin)
    async initFromIndexedDB() {
      // IndexedDB is client-only
      if (import.meta.server) return
      if (this.initialized) return

      this.loading = true

      try {
        const allAreas = await db.areas.toArray()
        this.areas = allAreas
        this.initialized = true
      } catch (error: any) {
        this.error = error.message
        console.error('[areas-store] Failed to initialize from IndexedDB:', error)
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
        const baseURL = config.public.apiBase
        const authStore = useAuthStore()

        const headers: Record<string, string> = {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
        }

        if (authStore.token) {
          headers['Authorization'] = `Bearer ${authStore.token}`
        }

        const areas = await $fetch<AreaDto[]>('/areas', {
          baseURL,
          headers,
          credentials: 'include'
        })

        // Save to IndexedDB
        const records: AreaRecord[] = areas.map(area => ({
          ...area,
          _synced: true,
          _localOnly: false,
          _deleted: false,
        }))
        await db.areas.bulkPut(records)

        // Reload from IndexedDB to get consistent records
        this.areas = await db.areas.toArray()
      } catch (error: any) {
        console.error('[areas-store] Failed to refresh from API:', error)
        // Don't set error state - we still have cached data
      }
    },

    // Reload from IndexedDB (called after sync completes)
    async reloadFromIndexedDB() {
      if (import.meta.server) return

      try {
        const freshData = await db.areas.toArray()
        // Use splice to update array in place - preserves Vue's reactivity tracking
        this.areas.splice(0, this.areas.length, ...freshData)
      } catch (error: any) {
        console.error('[areas-store] Failed to reload from IndexedDB:', error)
      }
    },

    // Create area
    async create(data: CreateAreaDto): Promise<{ success: boolean; id?: string; offline?: boolean }> {
      const { isOnline } = useOffline()
      const { queueOperation } = useSyncQueue()
      const toast = useToast()

      const tempId = `temp_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
      const now = new Date().toISOString()

      // Build fullPath
      let fullPath = data.name
      let parentAreaName: string | undefined
      if (data.parentAreaId) {
        const parent = this.getById(data.parentAreaId)
        if (parent) {
          fullPath = `${parent.fullPath} > ${data.name}`
          parentAreaName = parent.name
        }
      }

      const newArea: AreaRecord = {
        id: tempId,
        name: data.name,
        description: data.description,
        parentAreaId: data.parentAreaId,
        parentAreaName,
        fullPath,
        itemCount: 0,
        childCount: 0,
        createdAt: now,
        updatedAt: now,
        _synced: false,
        _localOnly: !isOnline.value,
        _deleted: false,
      }

      // Optimistically update store
      this.areas.push(newArea)

      // Save to IndexedDB - convert to plain object to avoid DataCloneError
      await db.areas.put(JSON.parse(JSON.stringify(newArea)))

      if (!isOnline.value) {
        // Queue for sync - convert payload to plain object
        await queueOperation({
          entityType: 'area',
          entityId: tempId,
          operation: 'create',
          payload: JSON.parse(JSON.stringify(toRaw(data))),
          timestamp: Date.now(),
        })
        toast.info('Area saved offline. Will sync when online.')
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

        const response = await $fetch<AreaDto>('/areas', {
          baseURL: config.public.apiBase,
          method: 'POST',
          body: data,
          headers,
          credentials: 'include'
        })

        // Update store with real data from server
        const idx = this.areas.findIndex(a => a.id === tempId)
        if (idx !== -1) {
          this.areas[idx] = {
            ...response,
            _synced: true,
            _localOnly: false,
            _deleted: false,
          }
        }

        // Update IndexedDB - remove temp, add real
        await db.areas.delete(tempId)
        await db.areas.put({
          ...response,
          _synced: true,
          _localOnly: false,
          _deleted: false,
        })

        toast.success('Area created successfully')
        return { success: true, id: response.id }
      } catch (error: any) {
        // Rollback optimistic update
        this.areas = this.areas.filter(a => a.id !== tempId)
        await db.areas.delete(tempId)

        toast.error(error.data?.message || 'Failed to create area')
        return { success: false }
      }
    },

    // Update area
    async update(id: string, data: UpdateAreaDto): Promise<{ success: boolean; offline?: boolean }> {
      const { isOnline } = useOffline()
      const { queueOperation } = useSyncQueue()
      const toast = useToast()

      const idx = this.areas.findIndex(a => a.id === id)
      if (idx === -1) return { success: false }

      const oldArea = { ...this.areas[idx] }
      const now = new Date().toISOString()

      // Build updated fullPath
      let fullPath = data.name
      let parentAreaName: string | undefined
      if (data.parentAreaId) {
        const parent = this.getById(data.parentAreaId)
        if (parent) {
          fullPath = `${parent.fullPath} > ${data.name}`
          parentAreaName = parent.name
        }
      }

      // Optimistically update store
      this.areas[idx] = {
        ...oldArea,
        name: data.name,
        description: data.description,
        parentAreaId: data.parentAreaId,
        fullPath,
        parentAreaName,
        updatedAt: now,
        _synced: false,
      }

      // Save to IndexedDB - convert to plain object
      await db.areas.put(JSON.parse(JSON.stringify(this.areas[idx])))

      if (!isOnline.value) {
        await queueOperation({
          entityType: 'area',
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

        const response = await $fetch<AreaDto>(`/areas/${id}`, {
          baseURL: config.public.apiBase,
          method: 'PUT',
          body: { ...data, id },
          headers,
          credentials: 'include'
        })

        // Update with server response
        this.areas[idx] = {
          ...response,
          _synced: true,
          _localOnly: false,
          _deleted: false,
        }
        await db.areas.put(this.areas[idx])

        toast.success('Area updated successfully')
        return { success: true }
      } catch (error: any) {
        // Rollback
        this.areas[idx] = oldArea
        await db.areas.put(oldArea)

        toast.error(error.data?.message || 'Failed to update area')
        return { success: false }
      }
    },

    // Delete area
    async delete(id: string): Promise<{ success: boolean; offline?: boolean }> {
      const { isOnline } = useOffline()
      const { queueOperation } = useSyncQueue()
      const toast = useToast()

      const idx = this.areas.findIndex(a => a.id === id)
      if (idx === -1) return { success: false }

      const oldArea = { ...this.areas[idx] }

      // Optimistically soft-delete in store
      this.areas[idx] = {
        ...this.areas[idx],
        _deleted: true,
        _synced: false,
      }

      // Soft delete in IndexedDB
      await db.areas.update(id, { _deleted: true, _synced: false })

      if (!isOnline.value) {
        await queueOperation({
          entityType: 'area',
          entityId: id,
          operation: 'delete',
          payload: null,
          timestamp: Date.now(),
        })
        toast.info('Area deleted offline. Will sync when online.')
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

        await $fetch(`/areas/${id}`, {
          baseURL: config.public.apiBase,
          method: 'DELETE',
          headers,
          credentials: 'include'
        })

        // Hard delete after successful sync
        await db.areas.delete(id)
        this.areas = this.areas.filter(a => a.id !== id)

        toast.success('Area deleted successfully')
        return { success: true }
      } catch (error: any) {
        // Rollback
        this.areas[idx] = oldArea
        await db.areas.put(oldArea)

        toast.error(error.data?.message || 'Failed to delete area')
        return { success: false }
      }
    },
  },
})
