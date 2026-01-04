<template>
  <ClientOnly>
    <div v-if="pending" class="flex justify-center items-center h-64">
      <LoadingSpinner size="lg" />
    </div>

    <div v-else-if="area" class="space-y-6">
      <nav class="text-sm text-gray-500">
        <NuxtLink to="/areas" class="hover:text-gray-700">Areas</NuxtLink>
        <span class="mx-2">/</span>
        <NuxtLink :to="`/areas/${id}`" class="hover:text-gray-700">{{ area.name }}</NuxtLink>
        <span class="mx-2">/</span>
        <span class="text-gray-900">Edit</span>
      </nav>

      <h1 class="text-3xl font-bold">Edit Area</h1>

      <BaseCard>
        <AreaForm
          :initial-data="area"
          :loading="isUpdating"
          @submit="handleSubmit"
          @cancel="navigateTo(`/areas/${id}`)"
        />
      </BaseCard>
    </div>

    <div v-else class="text-center py-12">
      <p class="text-gray-500">Area not found</p>
      <BaseButton class="mt-4" @click="navigateTo('/areas')">
        Back to Areas
      </BaseButton>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import type { CreateAreaDto } from '~/types/area'
import AreaForm from '~/components/areas/AreaForm.vue'

const route = useRoute()
const id = route.params.id as string

const { fetchArea, updateArea } = useAreas()

const { data: area, pending } = await fetchArea(id)
const isUpdating = ref(false)

const handleSubmit = async (data: CreateAreaDto) => {
  isUpdating.value = true
  const result = await updateArea(id, { ...data, id })
  isUpdating.value = false

  if (result.success) {
    navigateTo(`/areas/${id}`)
  }
}
</script>
