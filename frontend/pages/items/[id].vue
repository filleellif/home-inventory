<template>
  <ClientOnly>
    <div v-if="pending" class="flex justify-center items-center h-64">
      <LoadingSpinner size="lg" />
    </div>

    <div v-else-if="item" class="space-y-6">
    <nav class="text-sm text-gray-500">
      <NuxtLink to="/" class="hover:text-gray-700">Items</NuxtLink>
      <span class="mx-2">/</span>
      <span class="text-gray-900">{{ item.name }}</span>
    </nav>

    <div class="bg-white shadow rounded-lg p-6">
      <div class="flex justify-between items-start">
        <div>
          <h1 class="text-3xl font-bold">{{ item.name }}</h1>
          <p v-if="item.description" class="mt-2 text-gray-600">{{ item.description }}</p>
        </div>
        <div class="flex gap-2">
          <BaseButton variant="secondary" @click="navigateTo(`/items/${id}/edit`)">
            Edit
          </BaseButton>
          <BaseButton variant="danger" @click="handleDelete">
            Delete
          </BaseButton>
        </div>
      </div>

      <dl class="mt-6 grid grid-cols-1 gap-x-4 gap-y-6 sm:grid-cols-2">
        <div>
          <dt class="text-sm font-medium text-gray-500">Quantity</dt>
          <dd class="mt-1 text-sm text-gray-900">{{ item.quantity }}</dd>
        </div>

        <div v-if="item.categoryId">
          <dt class="text-sm font-medium text-gray-500">Category</dt>
          <dd class="mt-1 text-sm text-gray-900">{{ getCategoryName(item.categoryId) }}</dd>
        </div>

        <div v-if="formatLocation(item)" class="sm:col-span-2">
          <dt class="text-sm font-medium text-gray-500">Location</dt>
          <dd class="mt-1 text-sm text-gray-900">{{ formatLocation(item) }}</dd>
        </div>

        <div v-if="item.roomQrCode || item.shelfQrCode || item.boxQrCode" class="sm:col-span-2">
          <dt class="text-sm font-medium text-gray-500">QR Codes</dt>
          <dd class="mt-1 flex flex-wrap gap-3">
            <span v-if="item.roomQrCode" class="inline-flex items-center px-3 py-1 rounded-md text-sm bg-gray-100 text-gray-900 font-mono">
              Room: {{ item.roomQrCode }}
            </span>
            <span v-if="item.shelfQrCode" class="inline-flex items-center px-3 py-1 rounded-md text-sm bg-gray-100 text-gray-900 font-mono">
              Shelf: {{ item.shelfQrCode }}
            </span>
            <span v-if="item.boxQrCode" class="inline-flex items-center px-3 py-1 rounded-md text-sm bg-gray-100 text-gray-900 font-mono">
              Box: {{ item.boxQrCode }}
            </span>
          </dd>
        </div>

        <div class="sm:col-span-2">
          <dt class="text-sm font-medium text-gray-500">Last Updated</dt>
          <dd class="mt-1 text-sm text-gray-900">{{ formatDate(item.updatedAt) }}</dd>
        </div>
      </dl>
    </div>
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
const route = useRoute()
const id = route.params.id as string

const { fetchItem, deleteItem } = useItems()
const { fetchCategories } = useCategories()
const { fetchArea } = useAreas()

const { data: item, pending } = await fetchItem(id)
const { data: categoriesData } = await fetchCategories()

// Fetch area information if item has a location
const areaData = ref(null)
watch(() => item.value?.areaId, async (areaId) => {
  if (areaId) {
    const { data } = await fetchArea(areaId)
    areaData.value = data.value
  }
}, { immediate: true })

const categories = computed(() => categoriesData.value || [])

const getCategoryName = (categoryId: string) => {
  const category = categories.value.find(c => c.id === categoryId)
  return category?.name || '-'
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString()
}

const formatLocation = (item: any) => {
  // Use the area's full path if available
  if (areaData.value?.fullPath) {
    return areaData.value.fullPath
  }
  return null
}

const handleDelete = async () => {
  if (confirm('Are you sure you want to delete this item?')) {
    const result = await deleteItem(id)
    if (result.success) {
      navigateTo('/')
    }
  }
}
</script>
