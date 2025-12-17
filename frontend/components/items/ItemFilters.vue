<template>
  <BaseCard>
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <BaseInput
        v-model="localFilters.search"
        label="Search"
        placeholder="Search by name..."
        @update:model-value="emitFilters"
      />

      <BaseSelect
        v-model="localFilters.categoryId"
        label="Category"
        placeholder="All categories"
        @update:model-value="emitFilters"
      >
        <option v-for="category in categories" :key="category.id" :value="category.id">
          {{ category.name }}
        </option>
      </BaseSelect>

      <BaseInput
        v-model="localFilters.room"
        label="Room"
        placeholder="Filter by room..."
        @update:model-value="emitFilters"
      />
    </div>

    <div class="mt-4 flex justify-end">
      <BaseButton variant="secondary" @click="clearFilters">
        Clear Filters
      </BaseButton>
    </div>
  </BaseCard>
</template>

<script setup lang="ts">
import type { ItemFilters } from '~/types/api'
import type { CategoryDto } from '~/types/category'

interface Props {
  filters: ItemFilters
  categories?: CategoryDto[]
}

const props = defineProps<Props>()
const emit = defineEmits<{ 'update:filters': [filters: ItemFilters] }>()

const localFilters = ref<ItemFilters>({ ...props.filters })

watch(() => props.filters, (newFilters) => {
  localFilters.value = { ...newFilters }
}, { deep: true })

const emitFilters = () => {
  emit('update:filters', { ...localFilters.value })
}

const clearFilters = () => {
  localFilters.value = {}
  emitFilters()
}
</script>
