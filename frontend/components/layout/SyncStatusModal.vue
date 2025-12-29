<template>
  <BaseModal :open="open" @close="$emit('close')">
    <div class="space-y-4">
      <div class="flex items-center justify-between">
        <h3 class="text-lg font-semibold text-gray-900">Sync Status</h3>
        <button
          @click="$emit('close')"
          class="text-gray-400 hover:text-gray-500"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Status Overview -->
      <div class="bg-gray-50 rounded-lg p-4 space-y-3">
        <div class="flex justify-between items-center">
          <span class="text-sm text-gray-600">Connection</span>
          <span
            class="inline-flex items-center gap-1 px-2 py-1 rounded-md text-sm font-medium"
            :class="offlineStore.isOnline ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'"
          >
            <span class="w-2 h-2 rounded-full" :class="offlineStore.isOnline ? 'bg-green-500' : 'bg-red-500'" />
            {{ offlineStore.isOnline ? 'Online' : 'Offline' }}
          </span>
        </div>

        <div class="flex justify-between items-center">
          <span class="text-sm text-gray-600">Pending Changes</span>
          <span class="text-sm font-medium text-gray-900">
            {{ offlineStore.pendingSyncCount }}
          </span>
        </div>

        <div class="flex justify-between items-center">
          <span class="text-sm text-gray-600">Pending Media</span>
          <span class="text-sm font-medium text-gray-900">
            {{ offlineStore.pendingMediaCount }}
          </span>
        </div>

        <div v-if="offlineStore.failedSyncCount > 0" class="flex justify-between items-center">
          <span class="text-sm text-gray-600">Failed Syncs</span>
          <span class="text-sm font-medium text-red-600">
            {{ offlineStore.failedSyncCount }}
          </span>
        </div>

        <div v-if="offlineStore.lastSyncTime" class="flex justify-between items-center">
          <span class="text-sm text-gray-600">Last Synced</span>
          <span class="text-sm font-medium text-gray-900">
            {{ offlineStore.formatLastSyncTime() }}
          </span>
        </div>

        <div class="flex justify-between items-center">
          <span class="text-sm text-gray-600">Sync Status</span>
          <span
            class="inline-flex items-center gap-1 px-2 py-1 rounded-md text-sm font-medium"
            :class="{
              'bg-yellow-100 text-yellow-800': offlineStore.syncStatus === 'offline',
              'bg-blue-100 text-blue-800': offlineStore.syncStatus === 'syncing' || offlineStore.syncStatus === 'pending',
              'bg-green-100 text-green-800': offlineStore.syncStatus === 'synced',
              'bg-red-100 text-red-800': offlineStore.syncStatus === 'failed'
            }"
          >
            <LoadingSpinner v-if="offlineStore.syncStatus === 'syncing'" size="xs" />
            {{ syncStatusLabel }}
          </span>
        </div>
      </div>

      <!-- Error Message -->
      <div v-if="offlineStore.syncError" class="bg-red-50 border border-red-200 rounded-lg p-3">
        <div class="flex gap-2">
          <svg class="w-5 h-5 text-red-500 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <div>
            <p class="text-sm font-medium text-red-800">Sync Error</p>
            <p class="text-sm text-red-700 mt-1">{{ offlineStore.syncError }}</p>
          </div>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex gap-2 pt-2">
        <BaseButton
          v-if="offlineStore.hasPendingChanges && !offlineStore.isSyncing && offlineStore.isOnline"
          @click="handleSyncNow"
          class="flex-1"
        >
          <svg class="w-4 h-4 mr-1.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          Sync Now
        </BaseButton>

        <BaseButton
          v-if="offlineStore.hasFailedSyncs"
          @click="handleRetryFailed"
          variant="secondary"
          class="flex-1"
        >
          <svg class="w-4 h-4 mr-1.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          Retry Failed
        </BaseButton>

        <BaseButton
          v-if="offlineStore.isSyncing"
          variant="secondary"
          disabled
          class="flex-1"
        >
          <LoadingSpinner size="sm" class="mr-1.5" />
          Syncing...
        </BaseButton>
      </div>

      <!-- Info Text -->
      <div class="bg-blue-50 border border-blue-200 rounded-lg p-3">
        <div class="flex gap-2">
          <svg class="w-5 h-5 text-blue-500 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <p class="text-sm text-blue-700">
            Changes made while offline are automatically synced when you're back online.
            All data is safely stored locally until then.
          </p>
        </div>
      </div>
    </div>
  </BaseModal>
</template>

<script setup lang="ts">
interface Props {
  open: boolean
}

defineProps<Props>()
defineEmits<{ close: [] }>()

const offlineStore = useOfflineStore()

const syncStatusLabel = computed(() => {
  switch (offlineStore.syncStatus) {
    case 'offline': return 'Offline'
    case 'syncing': return 'Syncing'
    case 'pending': return 'Pending'
    case 'failed': return 'Failed'
    case 'synced': return 'Synced'
    default: return 'Unknown'
  }
})

const handleSyncNow = async () => {
  await offlineStore.startSync()
}

const handleRetryFailed = async () => {
  await offlineStore.retryFailedSyncs()
}
</script>
