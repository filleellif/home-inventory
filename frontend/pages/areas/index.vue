<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Areas</h1>
        <p class="mt-2 text-gray-600">Organize your spaces</p>
      </div>
      <BaseButton @click="navigateTo('/areas/new')">
        Add Area
      </BaseButton>
    </div>

    <AreaTree
      :areas="areas"
      :loading="pending"
    />
  </div>
</template>

<script setup lang="ts">
const { fetchAreas } = useAreas()

const { data: areasData, pending } = await fetchAreas()

const areas = computed(() => areasData.value || [])
</script>
