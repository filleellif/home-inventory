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
          rows="3"
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
      <h3 class="text-lg font-medium text-gray-900 mb-4">Financial Information</h3>
      <div class="space-y-4">
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <BaseInput
            v-model.number="form.purchasePrice"
            label="Purchase Price"
            type="number"
            step="0.01"
          />

          <BaseInput
            v-model="form.purchaseCurrency"
            label="Currency"
            placeholder="USD"
          />
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <BaseInput
            v-model.number="form.currentValue"
            label="Current Value"
            type="number"
            step="0.01"
          />

          <BaseInput
            v-model="form.currentValueCurrency"
            label="Currency"
            placeholder="USD"
          />
        </div>

        <BaseInput
          v-model="form.purchaseDate"
          label="Purchase Date"
          type="date"
        />
      </div>
    </BaseCard>

    <BaseCard>
      <h3 class="text-lg font-medium text-gray-900 mb-4">Location</h3>
      <div class="space-y-4">
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <BaseInput
            v-model="form.room"
            label="Room"
          />

          <BaseInput
            v-model="form.storageSpot"
            label="Storage Spot"
          />
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <BaseInput
            v-model.number="form.gpsLatitude"
            label="GPS Latitude"
            type="number"
            step="any"
          />

          <BaseInput
            v-model.number="form.gpsLongitude"
            label="GPS Longitude"
            type="number"
            step="any"
          />
        </div>
      </div>
    </BaseCard>

    <BaseCard>
      <h3 class="text-lg font-medium text-gray-900 mb-4">Tags</h3>
      <div>
        <div v-if="form.tags.length > 0" class="flex gap-2 flex-wrap mb-4">
          <span
            v-for="(tag, index) in form.tags"
            :key="index"
            class="inline-flex items-center px-3 py-1 rounded-full text-sm bg-blue-100 text-blue-800"
          >
            {{ tag }}
            <button
              type="button"
              @click="removeTag(index)"
              class="ml-2 text-blue-600 hover:text-blue-800"
            >
              ×
            </button>
          </span>
        </div>
        <div class="flex gap-2">
          <BaseInput
            v-model="newTag"
            placeholder="Add tag..."
            @keyup.enter="addTag"
          />
          <BaseButton type="button" variant="secondary" @click="addTag">
            Add
          </BaseButton>
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
  purchasePrice: props.initialData?.purchasePrice,
  purchaseCurrency: props.initialData?.purchaseCurrency || 'USD',
  currentValue: props.initialData?.currentValue,
  currentValueCurrency: props.initialData?.currentValueCurrency || 'USD',
  purchaseDate: props.initialData?.purchaseDate,
  room: props.initialData?.room || '',
  storageSpot: props.initialData?.storageSpot || '',
  gpsLatitude: props.initialData?.gpsLatitude,
  gpsLongitude: props.initialData?.gpsLongitude,
  categoryId: props.initialData?.categoryId,
  tags: props.initialData?.tags || []
})

const errors = ref<Record<string, string>>({})
const newTag = ref('')

const addTag = () => {
  if (newTag.value.trim() && !form.value.tags.includes(newTag.value.trim())) {
    form.value.tags.push(newTag.value.trim())
    newTag.value = ''
  }
}

const removeTag = (index: number) => {
  form.value.tags.splice(index, 1)
}

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
