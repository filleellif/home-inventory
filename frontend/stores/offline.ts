import { defineStore } from 'pinia'

export const useOfflineStore = defineStore('offline', {
  state: () => ({
    isOnline: true,
    lastSyncTime: null as number | null,
    pendingSyncCount: 0,
    failedSyncCount: 0,
    pendingMediaCount: 0,
    isSyncing: false,
    syncError: null as string | null,
  }),

  getters: {
    hasPendingChanges: (state) => state.pendingSyncCount > 0 || state.pendingMediaCount > 0,
    hasFailedSyncs: (state) => state.failedSyncCount > 0,
    syncStatus: (state) => {
      if (!state.isOnline) return 'offline'
      if (state.isSyncing) return 'syncing'
      if (state.failedSyncCount > 0) return 'failed'
      if (state.pendingSyncCount > 0 || state.pendingMediaCount > 0) return 'pending'
      return 'synced'
    },
  },

  actions: {
    updateOnlineStatus(online: boolean) {
      this.isOnline = online
      if (online && this.hasPendingChanges) {
        this.startSync()
      }
    },

    async startSync() {
      if (this.isSyncing || !this.isOnline) return

      const { processQueue } = useSyncQueue()
      const { processMediaQueue } = useMediaQueue()

      this.isSyncing = true
      this.syncError = null

      try {
        // Process sync queue first
        await processQueue()

        // Then process media queue
        await processMediaQueue()

        this.lastSyncTime = Date.now()
        this.syncError = null
      } catch (error: any) {
        this.syncError = error.message || 'Sync failed'
        console.error('Sync error:', error)
      } finally {
        this.isSyncing = false
        await this.updateSyncCounts()
      }
    },

    async updateSyncCounts() {
      // Get counts directly from IndexedDB to avoid composable lifecycle issues
      const { db } = await import('~/utils/db')

      this.pendingSyncCount = await db.syncQueue.where('status').equals('pending').count()
      this.failedSyncCount = await db.syncQueue.where('status').equals('failed').count()
      this.pendingMediaCount = await db.mediaQueue.count()
    },

    async retryFailedSyncs() {
      const { retryFailed } = useSyncQueue()
      await retryFailed()
      await this.updateSyncCounts()

      if (this.isOnline) {
        this.startSync()
      }
    },

    formatLastSyncTime(): string {
      if (!this.lastSyncTime) return 'Never'

      const diff = Date.now() - this.lastSyncTime
      const minutes = Math.floor(diff / 60000)

      if (minutes < 1) return 'Just now'
      if (minutes < 60) return `${minutes}m ago`

      const hours = Math.floor(minutes / 60)
      if (hours < 24) return `${hours}h ago`

      const days = Math.floor(hours / 24)
      return `${days}d ago`
    },
  },
})
