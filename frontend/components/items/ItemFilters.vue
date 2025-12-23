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

      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">
          QR Code
        </label>
        <div class="flex gap-2">
          <BaseInput
            v-model="localFilters.qrCode"
            placeholder="Scan or enter QR code..."
            @update:model-value="emitFilters"
          />
          <button
            type="button"
            class="inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            @click="scannerOpen = true"
          >
            <CameraIcon class="h-5 w-5" />
          </button>
        </div>
      </div>
    </div>

    <QrScanner :open="scannerOpen" @close="scannerOpen = false" @scan="handleQrScan" />

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
import { CameraIcon } from '@heroicons/vue/24/outline'
import QrScanner from '~/components/qr/QrScanner.vue'

interface Props {
  filters: ItemFilters
  categories?: CategoryDto[]
}

const props = defineProps<Props>()
const emit = defineEmits<{ 'update:filters': [filters: ItemFilters] }>()

const localFilters = ref<ItemFilters>({ ...props.filters })
const scannerOpen = ref(false)

watch(() => props.filters, (newFilters) => {
  localFilters.value = { ...newFilters }
}, { deep: true })

const emitFilters = () => {
  emit('update:filters', { ...localFilters.value })
}

const handleQrScan = (qrCode: string) => {
  localFilters.value.qrCode = qrCode
  emitFilters()
}

const clearFilters = () => {
  localFilters.value = {}
  emitFilters()
}
</script>
