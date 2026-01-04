<template>
  <div class="space-y-6">
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Items</h1>
        <p class="mt-2 text-gray-600">Manage your inventory items</p>
      </div>
      <BaseButton @click="openCreateModal">
        Add Item
      </BaseButton>
    </div>

    <ItemFilters
      v-model:filters="filters"
      :categories="categories"
    />

    <ClientOnly>
      <ItemTable
        :items="items.items"
        :loading="itemsStore.loading"
        :categories="categories"
        @edit="handleEdit"
        @delete="handleDelete"
        @create="openCreateModal"
      />

      <BasePagination
        v-if="items.totalPages > 0"
        :current-page="items.pageNumber"
        :total-pages="items.totalPages"
        :has-previous="items.hasPreviousPage"
        :has-next="items.hasNextPage"
        @page-change="goToPage"
      />
    </ClientOnly>

    <BaseModal :open="isCreateModalOpen" @close="closeCreateModal">
      <div class="mb-4">
        <h3 class="text-lg font-medium text-gray-900">Create Item</h3>
      </div>
      <ItemForm
        :loading="creating"
        @submit="handleCreate"
        @cancel="closeCreateModal"
      />
    </BaseModal>
  </div>
</template>

<script setup lang="ts">
import type { ItemFilters } from '~/types/api'
import type { CreateItemDto } from '~/types/item'

const { fetchCategories } = useCategories()
const itemsStore = useItemsStore()

const filters = ref<ItemFilters>({})
const currentPage = ref(1)

const { data: categoriesData } = await fetchCategories()
const categories = computed(() => categoriesData.value || [])

// Initialize store on client
onMounted(async () => {
  if (!itemsStore.initialized) {
    await itemsStore.initFromIndexedDB()
  }
})

// Sync filters to store
watch(filters, (newFilters) => {
  itemsStore.setFilters({
    search: newFilters.search || '',
    categoryId: newFilters.categoryId,
  })
}, { deep: true, immediate: true })

// Sync page to store
watch(currentPage, (page) => {
  itemsStore.goToPage(page)
}, { immediate: true })

// Get items from store (reactive)
const items = computed(() => itemsStore.paginationInfo)

// Go to page
const goToPage = (page: number) => {
  currentPage.value = page
}

// Reset to page 1 when filters change
watch(() => filters.value, () => {
  if (currentPage.value !== 1) {
    currentPage.value = 1
  }
}, { deep: true })

const { isOpen: isCreateModalOpen, open: openCreateModal, close: closeCreateModal } = useModal()
const creating = ref(false)

const handleCreate = async (data: CreateItemDto) => {
  creating.value = true
  try {
    // Create a plain copy of the data to avoid reactivity issues
    const plainData: CreateItemDto = {
      name: data.name,
      description: data.description,
      quantity: data.quantity,
      areaId: data.areaId,
      categoryId: data.categoryId,
    }
    const result = await itemsStore.create(plainData)
    if (result.success) {
      closeCreateModal()
      // No need to refresh - store is reactive
    }
  } finally {
    creating.value = false
  }
}

const handleEdit = (id: string) => {
  navigateTo(`/items/${id}/edit`)
}

const handleDelete = async (id: string) => {
  if (confirm('Are you sure you want to delete this item?')) {
    await itemsStore.delete(id)
    // No need to refresh - store is reactive
  }
}
</script>
