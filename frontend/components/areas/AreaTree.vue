<template>
  <div v-if="loading" class="p-8 text-center">
    <LoadingSpinner size="lg" />
  </div>

  <div v-else-if="!areas || areas.length === 0">
    <EmptyState
      title="No areas found"
      description="Get started by creating your first area"
    >
      <template #action>
        <BaseButton @click="navigateTo('/areas/new')">
          Add Area
        </BaseButton>
      </template>
    </EmptyState>
  </div>

  <div v-else class="bg-white shadow overflow-hidden sm:rounded-lg">
    <ul class="divide-y divide-gray-200">
      <AreaTreeNode
        v-for="area in treeData"
        :key="area.id"
        :area="area"
        :level="0"
      />
    </ul>
  </div>
</template>

<script setup lang="ts">
import type { AreaDto, AreaTreeNode as TreeNode } from '~/types/area'

interface Props {
  areas: AreaDto[]
  loading?: boolean
}

const props = defineProps<Props>()

const { buildAreaTree } = useAreas()

const treeData = computed(() => {
  return buildAreaTree(props.areas)
})
</script>
