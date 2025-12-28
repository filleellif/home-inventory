<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold text-gray-900">Dashboard</h1>
      <p class="mt-2 text-gray-600">Overview of your home inventory</p>
    </div>

    <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
      <BaseCard>
        <div class="text-sm font-medium text-gray-500">Total Items</div>
        <div class="mt-2 text-3xl font-semibold text-primary-600">
          {{ stats.totalItems }}
        </div>
      </BaseCard>

      <BaseCard>
        <div class="text-sm font-medium text-gray-500">Categories</div>
        <div class="mt-2 text-3xl font-semibold text-primary-600">
          {{ stats.totalCategories }}
        </div>
      </BaseCard>

      <BaseCard>
        <div class="text-sm font-medium text-gray-500">Last Updated</div>
        <div class="mt-2 text-lg font-semibold text-primary-600">
          {{ stats.lastUpdated }}
        </div>
      </BaseCard>
    </div>

    <div>
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-xl font-semibold text-gray-900">Recent Items</h2>
        <NuxtLink to="/items" class="text-primary-600 hover:text-primary-700 text-sm font-medium">
          View all →
        </NuxtLink>
      </div>

      <ClientOnly>
        <div v-if="pending" class="flex justify-center items-center h-32">
          <LoadingSpinner size="lg" />
        </div>

        <div v-else-if="recentItems.length === 0">
          <EmptyState
            title="No items yet"
            description="Get started by adding your first item"
          >
            <template #action>
              <BaseButton @click="navigateTo('/items/new')">
                Add Item
              </BaseButton>
            </template>
          </EmptyState>
        </div>

        <div v-else class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <BaseCard
            v-for="item in recentItems"
            :key="item.id"
            class="hover:shadow-md transition-shadow cursor-pointer"
            @click="navigateTo(`/items/${item.id}`)"
          >
            <h3 class="font-medium text-gray-900">{{ item.name }}</h3>
            <p v-if="item.description" class="mt-1 text-sm text-gray-500 line-clamp-2">
              {{ item.description }}
            </p>
            <div class="mt-3 flex items-center justify-between text-sm">
              <span class="text-gray-500">Qty: {{ item.quantity }}</span>
            </div>
          </BaseCard>
        </div>
      </ClientOnly>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <BaseCard>
        <h3 class="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h3>
        <div class="space-y-2">
          <BaseButton class="w-full" @click="navigateTo('/items/new')">
            Add New Item
          </BaseButton>
          <BaseButton class="w-full" variant="secondary" @click="navigateTo('/items')">
            Browse All Items
          </BaseButton>
          <BaseButton class="w-full" variant="secondary" @click="navigateTo('/categories')">
            Manage Categories
          </BaseButton>
        </div>
      </BaseCard>

      <BaseCard>
        <h3 class="text-lg font-semibold text-gray-900 mb-4">Categories</h3>
        <div v-if="categories.length === 0" class="text-center text-gray-500 py-4">
          No categories yet
        </div>
        <ul v-else class="space-y-2">
          <li
            v-for="category in categories.slice(0, 5)"
            :key="category.id"
            class="flex items-center justify-between text-sm"
          >
            <span class="text-gray-700">{{ category.name }}</span>
            <span class="text-gray-500">{{ getItemCountForCategory(category.id) }} items</span>
          </li>
        </ul>
        <NuxtLink
          v-if="categories.length > 5"
          to="/categories"
          class="mt-4 block text-center text-primary-600 hover:text-primary-700 text-sm font-medium"
        >
          View all categories →
        </NuxtLink>
      </BaseCard>
    </div>
  </div>
</template>

<script setup lang="ts">
const { fetchItems } = useItems()
const { fetchCategories } = useCategories()

const { data: itemsData, pending } = await fetchItems('', undefined, 1, 6)
const { data: categoriesData } = await fetchCategories()

const recentItems = computed(() => itemsData.value?.items || [])
const categories = computed(() => categoriesData.value || [])

const stats = computed(() => {
  const items = itemsData.value?.items || []
  const totalItems = itemsData.value?.totalCount || 0
  const totalCategories = categories.value.length
  const lastUpdated = items.length > 0
    ? new Date(Math.max(...items.map(i => new Date(i.updatedAt).getTime()))).toLocaleDateString()
    : 'N/A'

  return {
    totalItems,
    totalCategories,
    lastUpdated
  }
})

const getItemCountForCategory = (categoryId: string) => {
  // This is a simplified version - in a real app, you'd want to fetch this from the API
  const items = itemsData.value?.items || []
  return items.filter(item => item.categoryId === categoryId).length
}
</script>
