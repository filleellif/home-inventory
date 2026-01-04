<template>
  <div class="space-y-6">
    <nav class="text-sm text-gray-500">
      <NuxtLink to="/areas" class="hover:text-gray-700">Areas</NuxtLink>
      <span class="mx-2">/</span>
      <span class="text-gray-900">New Area</span>
    </nav>

    <h1 class="text-3xl font-bold">Create Area</h1>

    <BaseCard>
      <AreaForm :loading="isCreating" @submit="handleSubmit" @cancel="navigateTo('/areas')" />
    </BaseCard>
  </div>
</template>

<script setup lang="ts">
import type { CreateAreaDto } from '~/types/area'
import AreaForm from '~/components/areas/AreaForm.vue'

const { createArea } = useAreas()
const isCreating = ref(false)

const handleSubmit = async (data: CreateAreaDto) => {
  isCreating.value = true
  const result = await createArea(data)
  isCreating.value = false

  if (result.success) {
    navigateTo('/areas')
  }
}
</script>
