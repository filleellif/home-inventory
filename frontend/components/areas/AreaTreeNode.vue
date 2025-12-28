<template>
  <li>
    <div class="px-6 py-4 flex items-center hover:bg-gray-50" :style="{ paddingLeft: `${level * 2 + 1.5}rem` }">
      <div class="flex-1 min-w-0 cursor-pointer" @click="navigateTo(`/areas/${area.id}`)">
        <p class="text-sm font-medium text-gray-900">
          {{ area.name }}
        </p>
        <p v-if="area.description" class="text-sm text-gray-500 mt-1">
          {{ area.description }}
        </p>
      </div>
      <div class="flex items-center gap-4 text-sm text-gray-500">
        <span>{{ area.itemCount }} {{ area.itemCount === 1 ? 'item' : 'items' }}</span>
        <span v-if="area.children?.length">{{ area.children.length }} {{ area.children.length === 1 ? 'subarea' : 'subareas' }}</span>
      </div>
      <div class="flex items-center gap-2 ml-4">
        <button
          @click.stop="$emit('move', area)"
          class="text-blue-600 hover:text-blue-900 text-sm font-medium"
        >
          Move
        </button>
        <button
          @click.stop="navigateTo(`/areas/${area.id}/edit`)"
          class="text-primary-600 hover:text-primary-900 text-sm font-medium"
        >
          Edit
        </button>
        <button
          @click.stop="$emit('delete', area.id)"
          class="text-red-600 hover:text-red-900 text-sm font-medium"
        >
          Delete
        </button>
      </div>
    </div>

    <AreaTreeNode
      v-for="child in area.children"
      :key="child.id"
      :area="child"
      :level="level + 1"
      @move="$emit('move', $event)"
      @delete="$emit('delete', $event)"
    />
  </li>
</template>

<script setup lang="ts">
import type { AreaTreeNode } from '~/types/area'

interface Props {
  area: AreaTreeNode
  level: number
}

defineProps<Props>()
defineEmits<{
  move: [area: AreaTreeNode]
  delete: [id: string]
}>()
</script>
