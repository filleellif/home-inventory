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
  // Use the full path provided by the API (e.g., "storage room --> shelf 1")
  return area.fullPath
}
</script>
