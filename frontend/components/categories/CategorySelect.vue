<template>
  <BaseSelect
    :model-value="modelValue"
    :label="label"
    :placeholder="placeholder"
    @update:model-value="$emit('update:modelValue', $event)"
  >
    <option v-for="category in categories" :key="category.id" :value="category.id">
      {{ getIndentedName(category) }}
    </option>
  </BaseSelect>
</template>

<script setup lang="ts">
import type { CategoryDto } from '~/types/category'

interface Props {
  modelValue?: string
  label?: string
  placeholder?: string
}

defineProps<Props>()
defineEmits<{ 'update:modelValue': [value: string] }>()

const { fetchCategories, buildCategoryTree } = useCategories()
const { data: categoriesData } = await fetchCategories()

// Flatten tree into hierarchical list (sorted by name, maintaining hierarchy)
const categories = computed(() => {
  if (!categoriesData.value) return []

  const tree = buildCategoryTree(categoriesData.value)
  const flattened: Array<CategoryDto & { fullPath: string }> = []

  const flatten = (nodes: any[], parentPath = '') => {
    nodes.forEach(node => {
      const fullPath = parentPath ? `${parentPath} > ${node.name}` : node.name
      flattened.push({
        ...node,
        fullPath
      })
      if (node.children?.length > 0) {
        flatten(node.children, fullPath)
      }
    })
  }

  flatten(tree)
  return flattened
})

const getIndentedName = (category: CategoryDto & { fullPath?: string }) => {
  // Show full hierarchical path (e.g., "Electronics > Computers > Laptops")
  return category.fullPath || category.name
}
</script>
