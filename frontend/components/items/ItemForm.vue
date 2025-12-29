<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <BaseCard>
      <h3 class="text-lg font-medium text-gray-900 mb-4">Basic Information</h3>
      <div class="space-y-4">
        <BaseInput
          v-model="form.name"
          label="Name"
          required
          :error="errors.name"
        />

        <BaseTextarea
          v-model="form.description"
          label="Description"
          :rows="3"
        />

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <BaseInput
            v-model.number="form.quantity"
            label="Quantity"
            type="number"
            required
            :error="errors.quantity"
          />

          <CategorySelect
            v-model="form.categoryId"
            label="Category"
          />
        </div>
      </div>
    </BaseCard>

    <BaseCard>
      <h3 class="text-lg font-medium text-gray-900 mb-4">Location</h3>
      <div class="space-y-6">
        <!-- Room -->
        <div class="space-y-3">
          <div class="space-y-3">
          <AreaSelect
            v-model="form.areaId"
            label="Area"
          />
            <!-- <div class="flex gap-2">
              <BaseInput
                v-model="form.roomQrCode"
                label="Room QR Code"
                placeholder="Scan or enter code"
                class="flex-1"
              />
              <button
                type="button"
                class="mt-6 inline-flex items-center px-3 py-2 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                @click="openScanner('room')"
              >
                <CameraIcon class="h-5 w-5" />
              </button>
            </div> -->
          </div>
        </div>
      </div>
    </BaseCard>

    <div class="flex justify-end gap-3">
      <BaseButton type="button" variant="secondary" @click="$emit('cancel')">
        Cancel
      </BaseButton>
      <BaseButton type="submit" :loading="loading">
        {{ initialData ? 'Update' : 'Create' }}
      </BaseButton>
    </div>
  </form>
</template>

<script setup lang="ts">
import type { ItemDto, CreateItemDto } from '~/types/item'
import { CameraIcon } from '@heroicons/vue/24/outline'
import QrScanner from '~/components/qr/QrScanner.vue'

interface Props {
  initialData?: ItemDto
  loading?: boolean
}

const props = defineProps<Props>()
const emit = defineEmits<{
  submit: [data: CreateItemDto]
  cancel: []
}>()

const form = ref<CreateItemDto>({
  name: props.initialData?.name || '',
  description: props.initialData?.description || '',
  quantity: props.initialData?.quantity || 1,
  areaId: props.initialData?.areaId,
  categoryId: props.initialData?.categoryId
})

const errors = ref<Record<string, string>>({})

const validate = () => {
  errors.value = {}

  if (!form.value.name) {
    errors.value.name = 'Name is required'
  }

  if (!form.value.quantity || form.value.quantity < 1) {
    errors.value.quantity = 'Quantity must be at least 1'
  }

  return Object.keys(errors.value).length === 0
}

const handleSubmit = () => {
  if (validate()) {
    emit('submit', form.value)
  }
}
</script>
