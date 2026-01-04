<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Categories</h1>
        <p class="mt-2 text-gray-600">Organize your items</p>
      </div>
      <BaseButton @click="openCreateModal">
        Add Category
      </BaseButton>
    </div>

    <CategoryTree
      :categories="categories"
      :loading="pending"
      @create="openCreateModal"
    />

    <BaseModal :open="isCreateModalOpen" @close="closeCreateModal">
      <div class="mb-4">
        <h3 class="text-lg font-medium text-gray-900">Create Category</h3>
      </div>
      <CategoryForm
        :categories="categories"
        :loading="creating"
        @submit="handleCreate"
        @cancel="closeCreateModal"
      />
    </BaseModal>
  </div>
</template>

<script setup lang="ts">
import type { CreateCategoryDto } from '~/types/category'

const { fetchCategories, createCategory } = useCategories()
const { data: categoriesData, pending, refresh } = await fetchCategories()

const categories = computed(() => categoriesData.value || [])

const { isOpen: isCreateModalOpen, open: openCreateModal, close: closeCreateModal } = useModal()
const creating = ref(false)

const handleCreate = async (data: CreateCategoryDto) => {
  creating.value = true
  try {
    const result = await createCategory(data)
    if (result.success) {
      closeCreateModal()
      // Refresh the categories list (works both online and offline)
      await refresh()
    }
  } finally {
    creating.value = false
  }
}
</script>
