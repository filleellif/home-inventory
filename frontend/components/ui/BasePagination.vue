<template>
  <div class="flex flex-col sm:flex-row items-center justify-between gap-4">
    <div class="text-sm text-gray-700">
      Showing page {{ currentPage }} of {{ totalPages }}
    </div>

    <div class="flex gap-2">
      <BaseButton
        variant="secondary"
        :disabled="!hasPrevious"
        @click="$emit('page-change', currentPage - 1)"
      >
        Previous
      </BaseButton>

      <BaseButton
        variant="secondary"
        :disabled="!hasNext"
        @click="$emit('page-change', currentPage + 1)"
      >
        Next
      </BaseButton>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  currentPage: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}

defineProps<Props>()
defineEmits<{ 'page-change': [page: number] }>()
</script>
