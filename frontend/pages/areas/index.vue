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
      @move="handleMoveClick"
      @delete="handleDelete"
    />

    <MoveAreaModal
      :show="showMoveModal"
      :area="areaToMove"
      :all-areas="areas"
      @close="showMoveModal = false"
      @move="handleMove"
    />
  </div>
</template>

<script setup lang="ts">
import type { AreaTreeNode } from '~/types/area'

const { fetchAreas, updateArea, deleteArea } = useAreas()

const { data: areasData, pending } = await fetchAreas()

const areas = computed(() => areasData.value || [])

const showMoveModal = ref(false)
const areaToMove = ref<AreaTreeNode | null>(null)

const handleMoveClick = (area: AreaTreeNode) => {
  areaToMove.value = area
  showMoveModal.value = true
}

const handleMove = async (areaId: string, newParentId: string | null) => {
  if (!areaToMove.value) return

  const result = await updateArea(areaId, {
    id: areaId,
    name: areaToMove.value.name,
    description: areaToMove.value.description,
    parentAreaId: newParentId || undefined
  })

  if (result.success) {
    showMoveModal.value = false
    areaToMove.value = null
  }
}

const handleDelete = async (id: string) => {
  if (!confirm('Are you sure you want to delete this area? This action cannot be undone.')) {
    return
  }

  await deleteArea(id)
}
</script>
