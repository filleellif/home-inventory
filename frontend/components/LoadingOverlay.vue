<template>
  <Transition
    enter-active-class="transition-opacity duration-200"
    leave-active-class="transition-opacity duration-200"
    enter-from-class="opacity-0"
    leave-to-class="opacity-0"
  >
    <div
      v-if="showOverlay"
      class="fixed inset-0 z-[100] flex items-center justify-center bg-gray-900/50 backdrop-blur-sm"
    >
      <div class="flex flex-col items-center gap-4 rounded-lg bg-white p-8 shadow-xl">
        <div
          class="h-12 w-12 animate-spin rounded-full border-4 border-blue-200 border-t-blue-600"
          role="status"
        >
          <span class="sr-only">Loading...</span>
        </div>
        <p class="text-lg font-medium text-gray-700">{{ displayMessage }}</p>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
const { isBlockingLoading, blockingLoadingMessage } = useLoading()

// Only access Pinia store on client side
const isSyncing = computed(() => {
  if (import.meta.server) return false
  const offlineStore = useOfflineStore()
  return offlineStore.isSyncing
})

const showOverlay = computed(() => {
  return isBlockingLoading.value || isSyncing.value
})

const displayMessage = computed(() => {
  if (isSyncing.value) {
    return 'Syncing changes...'
  }
  return blockingLoadingMessage.value || 'Loading...'
})
</script>
