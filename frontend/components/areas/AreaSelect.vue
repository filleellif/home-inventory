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

const { fetchAreas } = useAreas()
const { data: areasData } = await fetchAreas()

const areas = computed(() => areasData.value || [])

const getIndentedName = (area: AreaDto) => {
  // Simple indentation based on parent (can be enhanced with tree traversal)
  return area.parentAreaId ? `  ${area.name}` : area.name
}
</script>
