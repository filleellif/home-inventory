<template>
  <ClientOnly>
    <div class="space-y-6">
      <div class="flex justify-between items-center">
        <h1 class="text-3xl font-bold">Areas</h1>
        <BaseButton @click="navigateTo('/areas/new')">
          Add Area
        </BaseButton>
      </div>

      <div v-if="pending" class="flex justify-center items-center h-64">
        <LoadingSpinner size="lg" />
      </div>

      <div v-else-if="areas && areas.length > 0" class="grid gap-4">
        <BaseCard
          v-for="area in areas"
          :key="area.id"
          class="hover:shadow-lg transition-shadow cursor-pointer"
          @click="navigateTo(`/areas/${area.id}`)"
        >
          <div class="flex justify-between items-start">
            <div class="flex-1">
              <h3 class="text-lg font-semibold text-gray-900">{{ area.name }}</h3>
              <p v-if="area.fullPath && area.fullPath !== area.name" class="mt-1 text-sm text-gray-500">
                📍 {{ area.fullPath }}
              </p>
              <p v-if="area.description" class="mt-1 text-sm text-gray-600">{{ area.description }}</p>
              <div class="mt-2 flex items-center gap-4 text-sm text-gray-500">
                <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800 font-mono">
                  {{ area.id }}
                </span>
                <span>{{ area.itemCount }} {{ area.itemCount === 1 ? 'item' : 'items' }}</span>
                <span v-if="area.childCount > 0">{{ area.childCount }} {{ area.childCount === 1 ? 'child' : 'children' }}</span>
              </div>
            </div>
            <div class="flex gap-2" @click.stop>
              <BaseButton
                variant="secondary"
                size="sm"
                @click="navigateTo(`/areas/${area.id}/edit`)"
              >
                Edit
              </BaseButton>
              <BaseButton
                variant="danger"
                size="sm"
                @click="handleDelete(area.id)"
              >
                Delete
              </BaseButton>
            </div>
          </div>
        </BaseCard>
      </div>

      <div v-else class="text-center py-12 bg-gray-50 rounded-lg">
        <p class="text-gray-500 mb-4">No areas found. Create your first area to get started.</p>
        <BaseButton @click="navigateTo('/areas/new')">
          Add Area
        </BaseButton>
      </div>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
const { fetchAreas, deleteArea } = useAreas()

const { data: areas, pending, refresh } = await fetchAreas()

const handleDelete = async (id: string) => {
  if (confirm('Are you sure you want to delete this area?')) {
    const result = await deleteArea(id)
    if (result.success) {
      refresh()
    }
  }
}
</script>
