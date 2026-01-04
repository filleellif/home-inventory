import { db, type SyncOperation } from '~/utils/db'
import { nextTick } from 'vue'

export const useSyncQueue = () => {
  const { isOnline } = useOffline()
  const isSyncing = useState('isSyncing', () => false)
  const pendingCount = useState('pendingSyncCount', () => 0)
  const failedCount = useState('failedSyncCount', () => 0)

  // Reload all stores from IndexedDB after sync
  // Uses nextTick to avoid reactivity issues during component rendering
  const reloadStores = async () => {
    // Wait for Vue to finish current render cycle
    await nextTick()

    try {
      // Get store instances
      const areasStore = useAreasStore()
      const categoriesStore = useCategoriesStore()
      const itemsStore = useItemsStore()

      // Reload sequentially to avoid race conditions
      await areasStore.reloadFromIndexedDB()
      await categoriesStore.reloadFromIndexedDB()
      await itemsStore.reloadFromIndexedDB()

      console.log('[sync-queue] Stores reloaded after sync')
    } catch (error) {
      console.error('[sync-queue] Failed to reload stores:', error)
    }
  }

  // Queue an operation for syncing
  const queueOperation = async (operation: Omit<SyncOperation, 'id' | 'status' | 'retryCount'>) => {
    const op: SyncOperation = {
      id: `op_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
      status: 'pending',
      retryCount: 0,
      ...operation
    }

    await db.syncQueue.add(op)
    await updateCounts()

    // If online, try to sync immediately
    if (isOnline.value) {
      processQueue()
    }
  }

  // Process the entire sync queue
  const processQueue = async () => {
    if (isSyncing.value || !isOnline.value) return

    isSyncing.value = true

    try {
      // Get all pending operations
      const operations = await db.syncQueue
        .where('status')
        .equals('pending')
        .toArray()

      if (operations.length === 0) {
        return
      }

      // Sort by dependency order and timestamp
      const sorted = sortByDependency(operations)

      // Process each operation sequentially
      for (const op of sorted) {
        await processSingleOperation(op)
      }

      // After entity sync, process media queue
      const { processMediaQueue } = useMediaQueue()
      await processMediaQueue()

      // Reload stores from IndexedDB to reflect synced changes
      await reloadStores()

    } catch (error) {
      console.error('Error processing sync queue:', error)
    } finally {
      isSyncing.value = false
      await updateCounts()
    }
  }

  // Sort operations by entity type priority and timestamp
  const sortByDependency = (operations: SyncOperation[]): SyncOperation[] => {
    const priority = { area: 1, category: 2, item: 3 }

    return operations.sort((a, b) => {
      // First by entity type priority
      const priorityDiff = priority[a.entityType] - priority[b.entityType]
      if (priorityDiff !== 0) return priorityDiff

      // Then by timestamp (oldest first)
      return a.timestamp - b.timestamp
    })
  }

  // Process a single sync operation
  const processSingleOperation = async (op: SyncOperation) => {
    console.log('Processing sync operation:', op.id, op.entityType, op.operation)
    try {
      // Re-read the operation from IndexedDB to get the latest payload
      // (it may have been updated by a previous sync that replaced temp IDs)
      const freshOp = await db.syncQueue.get(op.id)
      if (!freshOp) {
        console.log('Operation no longer exists, skipping:', op.id)
        return
      }

      await db.syncQueue.update(freshOp.id, { status: 'processing' })

      const config = useRuntimeConfig()
      const baseURL = config.public.apiBase

      // Build endpoint and HTTP method
      const { endpoint, method } = buildRequest(freshOp)

      console.log('Syncing:', method, endpoint, 'Payload:', freshOp.payload)

      // Make the API request
      const response = await $fetch(`${baseURL}${endpoint}`, {
        method,
        body: freshOp.operation === 'delete' ? undefined : freshOp.payload,
        headers: {
          'Content-Type': 'application/json'
        }
      })

      console.log('Sync response:', response)

      // Success! Update local entity and remove from queue
      await handleSyncSuccess(freshOp, response)

      console.log('Deleting from queue:', freshOp.id)
      await db.syncQueue.delete(freshOp.id)
      console.log('✓ Successfully synced and removed from queue:', freshOp.entityType, freshOp.entityId)

    } catch (error: any) {
      console.error('Sync error for operation:', op.id, error)
      await handleSyncError(op, error)
    }
  }

  // Build API endpoint and method from operation
  const buildRequest = (op: SyncOperation): { endpoint: string; method: string } => {
    const entityMap: Record<string, string> = {
      item: 'items',
      area: 'areas',
      category: 'categories'
    }

    const entityPath = entityMap[op.entityType]
    if (!entityPath) {
      console.error('Invalid entity type:', op.entityType, 'Full operation:', op)
      throw new Error(`Invalid entity type: ${op.entityType}`)
    }

    const baseEndpoint = `/${entityPath}`

    switch (op.operation) {
      case 'create':
        return { endpoint: baseEndpoint, method: 'POST' }
      case 'update':
        return { endpoint: `${baseEndpoint}/${op.entityId}`, method: 'PUT' }
      case 'delete':
        return { endpoint: `${baseEndpoint}/${op.entityId}`, method: 'DELETE' }
      default:
        console.error('Invalid operation:', op.operation, 'Full operation:', op)
        throw new Error(`Invalid operation: ${op.operation}`)
    }
  }

  // Handle successful sync
  const handleSyncSuccess = async (op: SyncOperation, serverResponse: any) => {
    const { saveItem, saveArea, saveCategory, hardDeleteItem, hardDeleteArea, hardDeleteCategory } = useOfflineStorage()

    if (op.operation === 'delete') {
      // Hard delete locally after successful server deletion
      switch (op.entityType) {
        case 'item':
          await hardDeleteItem(op.entityId)
          break
        case 'area':
          await hardDeleteArea(op.entityId)
          break
        case 'category':
          await hardDeleteCategory(op.entityId)
          break
      }
    } else {
      // Update local entity with server response
      const syncedData = {
        ...serverResponse,
        id: serverResponse.id || op.entityId
      }

      switch (op.entityType) {
        case 'item':
          await saveItem(syncedData, true, false)
          break
        case 'area':
          await saveArea(syncedData, true, false)
          break
        case 'category':
          await saveCategory(syncedData, true, false)
          break
      }

      // If this was a temp ID, delete the old temp record and update related entities
      if (op.entityId.startsWith('temp_') && serverResponse.id && serverResponse.id !== op.entityId) {
        // Delete the old temp record
        switch (op.entityType) {
          case 'item':
            await hardDeleteItem(op.entityId)
            break
          case 'area':
            await hardDeleteArea(op.entityId)
            break
          case 'category':
            await hardDeleteCategory(op.entityId)
            break
        }
        // Update any related entities that reference the old temp ID
        await updateRelatedEntities(op.entityType, op.entityId, serverResponse.id)
      }
    }
  }

  // Update entities that reference the old temp ID
  const updateRelatedEntities = async (entityType: string, oldId: string, newId: string) => {
    if (entityType === 'area') {
      // Update items that reference this area
      const items = await db.items.where('areaId').equals(oldId).toArray()
      for (const item of items) {
        await db.items.update(item.id, { areaId: newId })
      }

      // Update child areas
      const areas = await db.areas.where('parentAreaId').equals(oldId).toArray()
      for (const area of areas) {
        await db.areas.update(area.id, { parentAreaId: newId })
      }
    } else if (entityType === 'category') {
      // Update items that reference this category
      const items = await db.items.where('categoryId').equals(oldId).toArray()
      for (const item of items) {
        await db.items.update(item.id, { categoryId: newId })
      }

      // Update child categories
      const categories = await db.categories.where('parentCategoryId').equals(oldId).toArray()
      for (const category of categories) {
        await db.categories.update(category.id, { parentCategoryId: newId })
      }
    }

    // Update media queue items
    const mediaItems = await db.mediaQueue.where('entityId').equals(oldId).toArray()
    for (const media of mediaItems) {
      await db.mediaQueue.update(media.id, { entityId: newId })
    }

    // Update sync queue items - both entityId and payload references
    const syncItems = await db.syncQueue.where('entityId').equals(oldId).toArray()
    for (const sync of syncItems) {
      await db.syncQueue.update(sync.id, { entityId: newId })
    }

    // Update sync queue payloads that reference the old ID
    // This handles items/areas/categories that were created with references to temp IDs
    const allPendingSyncs = await db.syncQueue.where('status').equals('pending').toArray()
    for (const sync of allPendingSyncs) {
      if (!sync.payload) continue

      let updated = false
      const newPayload = { ...sync.payload }

      // Update areaId references (for items)
      if (entityType === 'area' && newPayload.areaId === oldId) {
        newPayload.areaId = newId
        updated = true
      }

      // Update categoryId references (for items)
      if (entityType === 'category' && newPayload.categoryId === oldId) {
        newPayload.categoryId = newId
        updated = true
      }

      // Update parentAreaId references (for child areas)
      if (entityType === 'area' && newPayload.parentAreaId === oldId) {
        newPayload.parentAreaId = newId
        updated = true
      }

      // Update parentCategoryId references (for child categories)
      if (entityType === 'category' && newPayload.parentCategoryId === oldId) {
        newPayload.parentCategoryId = newId
        updated = true
      }

      if (updated) {
        await db.syncQueue.update(sync.id, { payload: newPayload })
        console.log(`[sync-queue] Updated payload reference ${oldId} -> ${newId} in operation ${sync.id}`)
      }
    }
  }

  // Handle sync errors
  const handleSyncError = async (op: SyncOperation, error: any) => {
    const retryCount = op.retryCount + 1

    // After 5 retries, mark as failed
    if (retryCount > 5) {
      await db.syncQueue.update(op.id, {
        status: 'failed',
        retryCount,
        lastError: error.message || 'Unknown error'
      })

      const toast = useToast()
      toast.error(`Failed to sync ${op.entityType}. Check sync status.`)
    } else {
      // Queue for retry with exponential backoff
      await db.syncQueue.update(op.id, {
        status: 'pending',
        retryCount,
        lastError: error.message || 'Unknown error'
      })

      // Exponential backoff: 1min, 5min, 15min, 1hr, 4hr
      const delays = [60000, 300000, 900000, 3600000, 14400000]
      const delay = delays[retryCount - 1] || delays[delays.length - 1]

      setTimeout(() => {
        if (isOnline.value) {
          processQueue()
        }
      }, delay)
    }
  }

  // Update pending/failed counts
  const updateCounts = async () => {
    pendingCount.value = await db.syncQueue.where('status').equals('pending').count()
    failedCount.value = await db.syncQueue.where('status').equals('failed').count()
  }

  // Retry all failed operations
  const retryFailed = async () => {
    await db.syncQueue
      .where('status')
      .equals('failed')
      .modify({ status: 'pending', retryCount: 0, lastError: undefined })

    await updateCounts()

    if (isOnline.value) {
      processQueue()
    }
  }

  return {
    queueOperation,
    processQueue,
    isSyncing: readonly(isSyncing),
    pendingCount: readonly(pendingCount),
    failedCount: readonly(failedCount),
    retryFailed,
    updateCounts
  }
}
