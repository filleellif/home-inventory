<template>
  <BaseSelect
    :model-value="modelValue"
    :label="label"
    :placeholder="placeholder"
    @update:model-value="$emit('update:modelValue', $event)"
  >
    <option v-for="area in areas" :key="area.id" :value="area.id">
      {{ getIndentedName(area) }}
    </option>
  </BaseSelect>
</template>

<script setup lang="ts">
import type { AreaDto } from '~/types/area';

interface Props {
  modelValue?: string
  label?: string
  placeholder?: string
}

defineProps<Props>()
defineEmits<{ 'update:modelValue': [value: string] }>()

const { fetchAreas, buildAreaTree } = useAreas()
const { data: areasData } = await fetchAreas()

// Flatten tree into hierarchical list (sorted by name, maintaining hierarchy)
const areas = computed(() => {
  if (!areasData.value) return []

  const tree = buildAreaTree(areasData.value)
  const flattened: AreaDto[] = []

  const flatten = (nodes: any[]) => {
    nodes.forEach(node => {
      flattened.push(node)
      if (node.children?.length > 0) {
        flatten(node.children)
      }
    })
  }

  flatten(tree)
  return flattened
})

const getIndentedName = (area: AreaDto) => {
  // Use the full path provided by the API (e.g., "Basement > Storage")
  return area.fullPath
}
</script>
