<template>
  <ClientOnly>
    <div v-if="pending" class="flex justify-center items-center h-64">
      <LoadingSpinner size="lg" />
    </div>

    <div v-else-if="item" class="space-y-6">
      <nav class="text-sm text-gray-500">
        <NuxtLink to="/" class="hover:text-gray-700">Items</NuxtLink>
        <span class="mx-2">/</span>
        <NuxtLink :to="`/items/${id}`" class="hover:text-gray-700">{{ item.name }}</NuxtLink>
        <span class="mx-2">/</span>
        <span class="text-gray-900">Edit</span>
      </nav>

      <h1 class="text-3xl font-bold">Edit Item</h1>

      <BaseCard>
        <ItemForm
          :initial-data="item"
          :loading="loading"
          @submit="handleUpdate"
          @cancel="navigateTo(`/items/${id}`)"
        />
      </BaseCard>
    </div>

    <div v-else class="text-center py-12">
      <p class="text-gray-500">Item not found</p>
      <BaseButton class="mt-4" @click="navigateTo('/')">
        Back to Items
      </BaseButton>
    </div>
  </ClientOnly>
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
  const result = await updateItem(id, { ...data, id })
  loading.value = false

  if (result.success) {
    navigateTo(`/items/${id}`)
  }
}
</script>
