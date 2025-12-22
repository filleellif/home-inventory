<template>
  <div class="space-y-6">
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Items</h1>
        <p class="mt-2 text-gray-600">Manage your inventory items</p>
      </div>
      <BaseButton @click="navigateTo('/items/new')">
        Add Item
      </BaseButton>
    </div>

    <ItemFilters
      v-model:filters="filters"
      :categories="categories"
    />

    <ClientOnly>
      <ItemTable
        :items="items?.items"
        :loading="pending"
        :categories="categories"
        @edit="handleEdit"
        @delete="handleDelete"
        @create="navigateTo('/items/new')"
      />

      <BasePagination
        v-if="items && items.totalPages"
        :current-page="items.pageNumber"
        :total-pages="items.totalPages"
        :has-previous="items.hasPreviousPage"
        :has-next="items.hasNextPage"
        @page-change="goToPage"
      />
    </ClientOnly>
  </div>
</template>

<script setup lang="ts">
import type { ItemFilters } from '~/types/api'

const { fetchItems, deleteItem } = useItems()
const { fetchCategories } = useCategories()
const { currentPage, goToPage } = usePagination()

const filters = ref<ItemFilters>({})

const { data: categoriesData } = await fetchCategories()
const categories = computed(() => categoriesData.value || [])

const { data: items, pending, refresh } = await fetchItems(currentPage.value, 20, filters.value)

// Watch for page changes
watch(currentPage, async () => {
  await refresh()
})

// Watch for filter changes
watch(filters, async () => {
  currentPage.value = 1 // Reset to first page on filter change
  await refresh()
}, { deep: true })

const handleEdit = (id: string) => {
  navigateTo(`/items/${id}/edit`)
}

const handleDelete = async (id: string) => {
  if (confirm('Are you sure you want to delete this item?')) {
    const result = await deleteItem(id)
    if (result.success) {
      await refresh()
    }
  }
}
</script>
