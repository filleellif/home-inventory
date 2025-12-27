<template>
  <li>
    <div class="px-6 py-4 flex items-center hover:bg-gray-50 cursor-pointer" :style="{ paddingLeft: `${level * 2 + 1.5}rem` }" @click="navigateTo(`/areas/${area.id}`)">
      <div class="flex-1 min-w-0">
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
    </div>

    <AreaTreeNode
      v-for="child in area.children"
      :key="child.id"
      :area="child"
      :level="level + 1"
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
</script>
