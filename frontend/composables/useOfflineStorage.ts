import { db, type ItemRecord, type AreaRecord, type CategoryRecord } from '~/utils/db'
import type { ItemDto } from '~/types/item'
import type { AreaDto } from '~/types/area'
import type { CategoryDto } from '~/types/category'

export const useOfflineStorage = () => {
  // ============ ITEMS ============

  const saveItem = async (item: ItemDto, synced = false, localOnly = false) => {
    const record: ItemRecord = {
      ...item,
      _synced: synced,
      _localOnly: localOnly,
      _deleted: false
    }
    await db.items.put(record)
  }

  const getItem = async (id: string): Promise<ItemRecord | undefined> => {
    return await db.items.get(id)
  }

  const getItems = async (filters?: {
    searchQuery?: string
    categoryId?: string
    areaId?: string
  }): Promise<ItemRecord[]> => {
    const allItems = await db.items.toArray()

    // Filter out deleted items and apply other filters
    return allItems.filter(item => {
      // Skip deleted items
      if (item._deleted) return false

      if (filters?.categoryId && item.categoryId !== filters.categoryId) {
        return false
      }
      if (filters?.areaId && item.areaId !== filters.areaId) {
        return false
      }
      if (filters?.searchQuery) {
        const searchLower = filters.searchQuery.toLowerCase()
        return item.name.toLowerCase().includes(searchLower) ||
               item.description?.toLowerCase().includes(searchLower)
      }
      return true
    })
  }

  const deleteItem = async (id: string) => {
    // Soft delete
    await db.items.update(id, { _deleted: true, _synced: false })
  }

  const hardDeleteItem = async (id: string) => {
    await db.items.delete(id)
  }

  // ============ AREAS ============

  const saveArea = async (area: AreaDto, synced = false, localOnly = false) => {
    const record: AreaRecord = {
      ...area,
      _synced: synced,
      _localOnly: localOnly,
      _deleted: false
    }
    await db.areas.put(record)
  }

  const getArea = async (id: string): Promise<AreaRecord | undefined> => {
    return await db.areas.get(id)
  }

  const getAreas = async (): Promise<AreaRecord[]> => {
    const allAreas = await db.areas.toArray()
    return allAreas.filter(area => !area._deleted)
  }

  const deleteArea = async (id: string) => {
    await db.areas.update(id, { _deleted: true, _synced: false })
  }

  const hardDeleteArea = async (id: string) => {
    await db.areas.delete(id)
  }

  // ============ CATEGORIES ============

  const saveCategory = async (category: CategoryDto, synced = false, localOnly = false) => {
    const record: CategoryRecord = {
      ...category,
      _synced: synced,
      _localOnly: localOnly,
      _deleted: false
    }
    await db.categories.put(record)
  }

  const getCategory = async (id: string): Promise<CategoryRecord | undefined> => {
    return await db.categories.get(id)
  }

  const getCategories = async (): Promise<CategoryRecord[]> => {
    const allCategories = await db.categories.toArray()
    return allCategories.filter(category => !category._deleted)
  }

  const deleteCategory = async (id: string) => {
    await db.categories.update(id, { _deleted: true, _synced: false })
  }

  const hardDeleteCategory = async (id: string) => {
    await db.categories.delete(id)
  }

  // ============ CACHE METADATA ============

  const saveToCache = async (key: string, data: any) => {
    await db.metadata.put({ key, value: JSON.stringify(data) })
  }

  const getFromCache = async (key: string): Promise<any | null> => {
    const record = await db.metadata.get(key)
    return record ? JSON.parse(record.value) : null
  }

  const clearCache = async () => {
    await db.metadata.clear()
  }

  // ============ BULK OPERATIONS ============

  const saveItemsBulk = async (items: ItemDto[], synced = true) => {
    const records: ItemRecord[] = items.map(item => ({
      ...item,
      _synced: synced,
      _localOnly: false,
      _deleted: false
    }))
    await db.items.bulkPut(records)
  }

  const saveAreasBulk = async (areas: AreaDto[], synced = true) => {
    const records: AreaRecord[] = areas.map(area => ({
      ...area,
      _synced: synced,
      _localOnly: false,
      _deleted: false
    }))
    await db.areas.bulkPut(records)
  }

  const saveCategoriesBulk = async (categories: CategoryDto[], synced = true) => {
    const records: CategoryRecord[] = categories.map(category => ({
      ...category,
      _synced: synced,
      _localOnly: false,
      _deleted: false
    }))
    await db.categories.bulkPut(records)
  }

  // ============ DATABASE MANAGEMENT ============

  const clearAll = async () => {
    await db.items.clear()
    await db.areas.clear()
    await db.categories.clear()
    await db.syncQueue.clear()
    await db.mediaQueue.clear()
    await db.metadata.clear()
  }

  return {
    // Items
    saveItem,
    getItem,
    getItems,
    deleteItem,
    hardDeleteItem,

    // Areas
    saveArea,
    getArea,
    getAreas,
    deleteArea,
    hardDeleteArea,

    // Categories
    saveCategory,
    getCategory,
    getCategories,
    deleteCategory,
    hardDeleteCategory,

    // Cache
    saveToCache,
    getFromCache,
    clearCache,

    // Bulk operations
    saveItemsBulk,
    saveAreasBulk,
    saveCategoriesBulk,

    // Database management
    clearAll
  }
}
