<template>
  <div v-if="loading" class="p-8 text-center">
    <LoadingSpinner size="lg" />
  </div>

  <div v-else-if="!categories || categories.length === 0">
    <EmptyState
      title="No categories found"
      description="Get started by creating your first category"
    >
      <template #action>
        <BaseButton @click="$emit('create')">
          Add Category
        </BaseButton>
      </template>
    </EmptyState>
  </div>

  <div v-else class="bg-white shadow overflow-hidden sm:rounded-lg">
    <ul class="divide-y divide-gray-200">
      <CategoryTreeNode
        v-for="category in treeData"
        :key="category.id"
        :category="category"
        :level="0"
      />
    </ul>
  </div>
</template>

<script setup lang="ts">
import type { CategoryDto, CategoryTreeNode as TreeNode } from '~/types/category'

interface Props {
  categories: CategoryDto[]
  loading?: boolean
}

const props = defineProps<Props>()
defineEmits<{ create: [] }>()

const { buildCategoryTree } = useCategories()

const treeData = computed(() => {
  return buildCategoryTree(props.categories)
})
</script>
