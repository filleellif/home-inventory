import Dexie, { type Table } from 'dexie'
import type { ItemDto } from '~/types/item'
import type { AreaDto } from '~/types/area'
import type { CategoryDto } from '~/types/category'

// Extend DTOs with offline metadata
export interface ItemRecord extends ItemDto {
  _synced: boolean
  _localOnly: boolean
  _deleted: boolean
}

export interface AreaRecord extends AreaDto {
  _synced: boolean
  _localOnly: boolean
  _deleted: boolean
}

export interface CategoryRecord extends CategoryDto {
  _synced: boolean
  _localOnly: boolean
  _deleted: boolean
}

export interface SyncOperation {
  id: string
  entityType: 'item' | 'area' | 'category'
  entityId: string
  operation: 'create' | 'update' | 'delete'
  payload: any
  timestamp: number
  retryCount: number
  lastError?: string
  status: 'pending' | 'processing' | 'failed'
}

export interface MediaQueueItem {
  id: string
  entityId: string
  entityType: 'item'
  mediaType: 'photo' | 'receipt'
  file: Blob
  fileName: string
  fileSize: number
  timestamp: number
  retryCount: number
  lastError?: string
}

export interface MetadataRecord {
  key: string
  value: string
}

// Dexie database class
export class HomeInventoryDB extends Dexie {
  items!: Table<ItemRecord, string>
  areas!: Table<AreaRecord, string>
  categories!: Table<CategoryRecord, string>
  syncQueue!: Table<SyncOperation, string>
  mediaQueue!: Table<MediaQueueItem, string>
  metadata!: Table<MetadataRecord, string>

  constructor() {
    super('home-inventory-db')

    // Define database schema
    this.version(2).stores({
      items: 'id, areaId, categoryId, _synced, _deleted',
      areas: 'id, parentAreaId, _synced, _deleted',
      categories: 'id, parentCategoryId, _synced, _deleted',
      syncQueue: 'id, timestamp, entityType, status, entityId',
      mediaQueue: 'id, timestamp, entityType, entityId',
      metadata: 'key'
    })
  }
}

// Export singleton instance
export const db = new HomeInventoryDB()
