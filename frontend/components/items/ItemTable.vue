<template>
  <div class="bg-white shadow overflow-hidden sm:rounded-lg">
    <div v-if="loading" class="p-8 text-center">
      <LoadingSpinner size="lg" />
    </div>

    <div v-else-if="!items || items.length === 0">
      <EmptyState
        title="No items found"
        description="Get started by creating your first item"
      >
        <template #action>
          <BaseButton @click="$emit('create')">
            Add Item
          </BaseButton>
        </template>
      </EmptyState>
    </div>

    <div v-else class="overflow-x-auto">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Name
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Category
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Quantity
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Value
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
              Location
            </th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
              Actions
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="item in items" :key="item.id" class="hover:bg-gray-50">
            <td class="px-6 py-4">
              <NuxtLink :to="`/items/${item.id}`" class="text-primary-600 hover:text-primary-900 font-medium">
                {{ item.name }}
              </NuxtLink>
              <div v-if="item.description" class="text-sm text-gray-500 mt-1 truncate max-w-xs">
                {{ item.description }}
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ getCategoryName(item.categoryId) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ item.quantity }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ formatCurrency(item.currentValue, item.currentValueCurrency) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ item.room || '-' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <button
                @click="$emit('edit', item.id)"
                class="text-primary-600 hover:text-primary-900 mr-4"
              >
                Edit
              </button>
              <button
                @click="$emit('delete', item.id)"
                class="text-red-600 hover:text-red-900"
              >
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { ItemDto } from '~/types/item'

interface Props {
  items?: ItemDto[]
  loading?: boolean
  categories?: any[]
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  categories: () => []
})

defineEmits<{
  edit: [id: string]
  delete: [id: string]
  create: []
}>()

const getCategoryName = (categoryId?: string) => {
  if (!categoryId) return '-'
  const category = props.categories.find(c => c.id === categoryId)
  return category?.name || '-'
}

const formatCurrency = (value?: number, currency?: string) => {
  if (!value) return '-'
  return `${currency || '$'}${value.toFixed(2)}`
}
</script>
