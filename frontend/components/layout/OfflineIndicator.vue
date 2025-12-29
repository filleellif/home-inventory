<template>
  <div class="fixed bottom-0 left-0 right-0 z-50">
    <!-- Offline Banner -->
    <Transition
      enter-active-class="transition-transform duration-300 ease-out"
      enter-from-class="translate-y-full"
      enter-to-class="translate-y-0"
      leave-active-class="transition-transform duration-300 ease-in"
      leave-from-class="translate-y-0"
      leave-to-class="translate-y-full"
    >
      <div
        v-if="!offlineStore.isOnline"
        class="bg-yellow-500 text-white px-4 py-2 shadow-md"
      >
        <div class="max-w-7xl mx-auto flex items-center justify-center gap-2">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18.364 5.636a9 9 0 010 12.728m0 0l-2.829-2.829m2.829 2.829L21 21M15.536 8.464a5 5 0 010 7.072m0 0l-2.829-2.829m-4.243 2.829a4.978 4.978 0 01-1.414-2.83m-1.414 5.658a9 9 0 01-2.167-9.238m7.824 2.167a1 1 0 111.414 1.414m-1.414-1.414L3 3m8.293 8.293l1.414 1.414" />
          </svg>
          <span class="font-medium">You're offline</span>
          <span class="text-sm opacity-90">Changes will sync when connection is restored</span>
        </div>
      </div>
    </Transition>

    <!-- Pending Sync Banner -->
    <Transition
      enter-active-class="transition-transform duration-300 ease-out"
      enter-from-class="translate-y-full"
      enter-to-class="translate-y-0"
      leave-active-class="transition-transform duration-300 ease-in"
      leave-from-class="translate-y-0"
      leave-to-class="translate-y-full"
    >
      <div
        v-if="offlineStore.isOnline && offlineStore.hasPendingChanges"
        class="bg-blue-500 text-white px-4 py-2 shadow-md"
        :style="{ marginTop: !offlineStore.isOnline ? '40px' : '0' }"
      >
        <div class="max-w-7xl mx-auto flex items-center justify-center gap-3">
          <LoadingSpinner v-if="offlineStore.isSyncing" size="sm" class="text-white" />
          <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
          <span class="text-sm">
            <span v-if="offlineStore.isSyncing">Syncing changes...</span>
            <span v-else>
              {{ offlineStore.pendingSyncCount + offlineStore.pendingMediaCount }}
              {{ offlineStore.pendingSyncCount + offlineStore.pendingMediaCount === 1 ? 'change' : 'changes' }}
              pending sync
            </span>
          </span>
          <button
            v-if="!offlineStore.isSyncing"
            @click="offlineStore.startSync()"
            class="text-sm underline hover:no-underline"
          >
            Sync now
          </button>
        </div>
      </div>
    </Transition>

    <!-- Failed Sync Banner -->
    <Transition
      enter-active-class="transition-transform duration-300 ease-out"
      enter-from-class="translate-y-full"
      enter-to-class="translate-y-0"
      leave-active-class="transition-transform duration-300 ease-in"
      leave-from-class="translate-y-0"
      leave-to-class="translate-y-full"
    >
      <div
        v-if="offlineStore.hasFailedSyncs"
        class="bg-red-500 text-white px-4 py-2 shadow-md"
        :style="{ marginTop: getMarginTop() }"
      >
        <div class="max-w-7xl mx-auto flex items-center justify-center gap-3">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <span class="text-sm">
            {{ offlineStore.failedSyncCount }}
            {{ offlineStore.failedSyncCount === 1 ? 'sync' : 'syncs' }} failed
          </span>
          <button
            @click="offlineStore.retryFailedSyncs()"
            class="text-sm underline hover:no-underline"
          >
            Retry
          </button>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
const offlineStore = useOfflineStore()
const { isOnline, init } = useOffline()
const { processQueue } = useSyncQueue()
const { processMediaQueue } = useMediaQueue()

// Initialize offline detection
onMounted(() => {
  // Initialize offline detection
  init()

  // Update store when online status changes
  watch(isOnline, (online) => {
    offlineStore.updateOnlineStatus(online)

    // Trigger sync when coming back online
    if (online) {
      processQueue()
      processMediaQueue()
    }
  }, { immediate: true })

  // Initial counts update
  offlineStore.updateSyncCounts()

  // Periodic sync count updates (every 5 seconds)
  const interval = setInterval(() => {
    offlineStore.updateSyncCounts()
  }, 5000)

  onBeforeUnmount(() => {
    clearInterval(interval)
  })
})

const getMarginTop = () => {
  let margin = 0
  if (!offlineStore.isOnline) margin += 40
  if (offlineStore.hasPendingChanges) margin += 40
  return `${margin}px`
}
</script>
