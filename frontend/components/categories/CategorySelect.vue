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

const categories = computed(() => categoriesData.value || [])

const getIndentedName = (category: CategoryDto) => {
  // Simple indentation based on parent (can be enhanced with tree traversal)
  return category.parentCategoryId ? `  ${category.name}` : category.name
}
</script>
