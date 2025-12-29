<template>
  <span
    v-if="shouldShowBadge"
    class="inline-flex items-center gap-1 px-2 py-0.5 rounded-md text-xs font-medium"
    :class="badgeClasses"
    :title="badgeTitle"
  >
    <svg v-if="isLocalOnly" class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
    </svg>
    <svg v-else-if="!isSynced" class="w-3 h-3 animate-spin" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
    </svg>
    <svg v-else class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
    </svg>
    <span>{{ badgeText }}</span>
  </span>
</template>

<script setup lang="ts">
interface Props {
  item?: {
    id?: string
    _synced?: boolean
    _localOnly?: boolean
  }
}

const props = defineProps<Props>()

// Determine sync status from item properties
const isLocalOnly = computed(() => {
  if (!props.item?.id) return false
  // Check if ID starts with temp_ or if _localOnly flag is set
  return props.item.id.startsWith('temp_') || props.item._localOnly === true
})

const isSynced = computed(() => {
  if (!props.item) return true
  return props.item._synced === true
})

const shouldShowBadge = computed(() => {
  // Show badge if item is not synced or is local only
  return isLocalOnly.value || !isSynced.value
})

const badgeClasses = computed(() => {
  if (isLocalOnly.value) {
    return 'bg-yellow-100 text-yellow-800'
  }
  if (!isSynced.value) {
    return 'bg-blue-100 text-blue-800'
  }
  return 'bg-green-100 text-green-800'
})

const badgeText = computed(() => {
  if (isLocalOnly.value) {
    return 'Pending sync'
  }
  if (!isSynced.value) {
    return 'Syncing...'
  }
  return 'Synced'
})

const badgeTitle = computed(() => {
  if (isLocalOnly.value) {
    return 'This item was created offline and will sync when online'
  }
  if (!isSynced.value) {
    return 'This item is currently being synced'
  }
  return 'This item is synced with the server'
})
</script>
