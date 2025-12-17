<template>
  <div class="max-w-3xl mx-auto space-y-6">
    <div v-if="pending" class="flex justify-center items-center h-64">
      <LoadingSpinner size="lg" />
    </div>

    <template v-else-if="item">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Edit Item</h1>
        <p class="mt-2 text-gray-600">Update item information</p>
      </div>

      <ItemForm
        :initial-data="item"
        :loading="loading"
        @submit="handleUpdate"
        @cancel="navigateTo(`/items/${id}`)"
      />
    </template>

    <div v-else class="text-center py-12">
      <p class="text-gray-500">Item not found</p>
      <BaseButton class="mt-4" @click="navigateTo('/items')">
        Back to Items
      </BaseButton>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { CreateItemDto } from '~/types/item'

const route = useRoute()
const id = route.params.id as string

const { fetchItem, updateItem } = useItems()
const { data: item, pending } = await fetchItem(id)

const loading = ref(false)

const handleUpdate = async (data: CreateItemDto) => {
  loading.value = true
  try {
    const result = await updateItem(id, { ...data, id })
    if (result.success) {
      navigateTo(`/items/${id}`)
    }
  } finally {
    loading.value = false
  }
}
</script>
