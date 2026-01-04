<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Areas</h1>
        <p class="mt-2 text-gray-600">Organize your spaces</p>
      </div>
      <BaseButton @click="openCreateModal">
        Add Area
      </BaseButton>
    </div>

    <AreaTree
      :areas="areas"
      :loading="pending"
      @create="openCreateModal"
      @move="handleMoveClick"
      @delete="handleDelete"
    />

    <BaseModal :open="isCreateModalOpen" @close="closeCreateModal">
      <div class="mb-4">
        <h3 class="text-lg font-medium text-gray-900">Create Area</h3>
      </div>
      <AreaForm
        :loading="creating"
        @submit="handleCreate"
        @cancel="closeCreateModal"
      />
    </BaseModal>

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
import type { CreateAreaDto } from '~/types/area'

const { fetchAreas, updateArea, deleteArea, createArea } = useAreas()

const { data: areasData, pending, refresh } = await fetchAreas()

const areas = computed(() => areasData.value || [])

const { isOpen: isCreateModalOpen, open: openCreateModal, close: closeCreateModal } = useModal()
const creating = ref(false)

const handleCreate = async (data: CreateAreaDto) => {
  creating.value = true
  try {
    const result = await createArea(data)
    if (result.success) {
      closeCreateModal()
      // Refresh the areas list (works both online and offline)
      await refresh()
    }
  } finally {
    creating.value = false
  }
}

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
