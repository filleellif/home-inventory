<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
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

    <AreaSelect
      v-model="form.areaId"
      label="Area"
    />

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
    // Emit a plain copy to avoid reactivity issues
    const submitData = {
      name: form.value.name,
      description: form.value.description,
      quantity: form.value.quantity,
      areaId: form.value.areaId,
      categoryId: form.value.categoryId,
    }
    emit('submit', submitData)
  }
}
</script>
