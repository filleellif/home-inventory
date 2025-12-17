<template>
  <div class="max-w-3xl mx-auto space-y-6">
    <div>
      <h1 class="text-3xl font-bold text-gray-900">Create New Item</h1>
      <p class="mt-2 text-gray-600">Add a new item to your inventory</p>
    </div>

    <ItemForm
      :loading="loading"
      @submit="handleCreate"
      @cancel="navigateTo('/items')"
    />
  </div>
</template>

<script setup lang="ts">
import type { CreateItemDto } from '~/types/item'

const { createItem } = useItems()
const loading = ref(false)

const handleCreate = async (data: CreateItemDto) => {
  loading.value = true
  try {
    const result = await createItem(data)
    if (result.success && result.data) {
      navigateTo(`/items/${result.data.id}`)
    }
  } finally {
    loading.value = false
  }
}
</script>
