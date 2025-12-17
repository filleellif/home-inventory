<template>
  <li>
    <div class="px-6 py-4 flex items-center hover:bg-gray-50" :style="{ paddingLeft: `${level * 2 + 1.5}rem` }">
      <div class="flex-1 min-w-0">
        <p class="text-sm font-medium text-gray-900">
          {{ category.name }}
        </p>
        <p v-if="category.description" class="text-sm text-gray-500 mt-1">
          {{ category.description }}
        </p>
      </div>
      <div class="text-sm text-gray-500">
        {{ category.children?.length || 0 }} subcategories
      </div>
    </div>

    <CategoryTreeNode
      v-for="child in category.children"
      :key="child.id"
      :category="child"
      :level="level + 1"
    />
  </li>
</template>

<script setup lang="ts">
import type { CategoryTreeNode } from '~/types/category'

interface Props {
  category: CategoryTreeNode
  level: number
}

defineProps<Props>()
</script>
