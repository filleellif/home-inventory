<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <BaseInput
      v-model="form.name"
      label="Name"
      required
      :error="errors.name"
      placeholder="e.g., Basement, Garage, Storage Room"
    />

    <BaseSelect
      v-model="form.parentAreaId"
      label="Parent Area"
      :disabled="loadingAreas"
    >
      <option :value="undefined">None (Top Level)</option>
      <option
        v-for="area in sortedAreas"
        :key="area.id"
        :value="area.id"
      >
        {{ area.fullPath }}
      </option>
    </BaseSelect>

    <BaseTextarea
      v-model="form.description"
      label="Description"
      rows="3"
      placeholder="Optional description of this area"
    />

    <!-- QR Code Preview (auto-generated from ID) -->
    <div v-if="qrCodeValue" class="space-y-2">
      <label class="block text-sm font-medium text-gray-700">
        QR Code (Auto-generated)
      </label>
      <div class="flex items-center justify-center p-4 bg-gray-50 rounded-lg border border-gray-200">
        <canvas ref="qrCanvas" />
      </div>
      <p class="text-xs text-gray-500 text-center font-mono">{{ qrCodeValue }}</p>
      <div class="text-center">
        <button
          type="button"
          class="text-sm text-blue-600 hover:text-blue-800"
          @click="downloadQrLabel"
        >
          Download QR Label
        </button>
      </div>
    </div>

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
import type { AreaDto, CreateAreaDto } from '~/types/area'
import QRCode from 'qrcode'

interface Props {
  initialData?: AreaDto
  loading?: boolean
}

const props = defineProps<Props>()
const emit = defineEmits<{
  submit: [data: CreateAreaDto]
  cancel: []
}>()

const form = ref<CreateAreaDto>({
  name: props.initialData?.name || '',
  parentAreaId: props.initialData?.parentAreaId || undefined,
  description: props.initialData?.description || ''
})

const errors = ref<Record<string, string>>({})
const qrCanvas = ref<HTMLCanvasElement>()

// Fetch all areas for parent selection
const { fetchAreas, buildAreaTree } = useAreas()
const availableAreas = ref<AreaDto[]>([])
const loadingAreas = ref(false)

const loadAreas = async () => {
  loadingAreas.value = true
  try {
    const { data, error } = await fetchAreas()
    if (!error.value && data.value) {
      // Filter out current area to prevent self-reference
      availableAreas.value = data.value.filter(
        (area) => area.id !== props.initialData?.id
      )
    }
  } catch (err) {
    console.error('Failed to load areas:', err)
  } finally {
    loadingAreas.value = false
  }
}

// Flatten tree into hierarchical list (sorted by name, maintaining hierarchy)
const sortedAreas = computed(() => {
  const tree = buildAreaTree(availableAreas.value)
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

// For existing areas, use the area's ID as QR code. For new areas, we'll show it after creation.
const qrCodeValue = computed(() => props.initialData?.id || '')

const renderQrCode = async () => {
  if (!qrCodeValue.value || !qrCanvas.value) return

  try {
    await QRCode.toCanvas(qrCanvas.value, qrCodeValue.value, {
      width: 200,
      margin: 2,
      color: {
        dark: '#000000',
        light: '#FFFFFF',
      },
    })
  } catch (err) {
    console.error('Failed to render QR code:', err)
  }
}

const downloadQrLabel = async () => {
  if (!qrCodeValue.value) return

  try {
    const url = `/api/areas/qr-label/${encodeURIComponent(qrCodeValue.value)}${
      form.value.name ? `?label=${encodeURIComponent(form.value.name)}` : ''
    }`

    const response = await fetch(url)
    const blob = await response.blob()
    const blobUrl = URL.createObjectURL(blob)

    const link = document.createElement('a')
    link.href = blobUrl
    link.download = `qr-label-${qrCodeValue.value}.png`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    URL.revokeObjectURL(blobUrl)
  } catch (err) {
    console.error('Failed to download label:', err)
  }
}

const validate = () => {
  errors.value = {}

  if (!form.value.name) {
    errors.value.name = 'Name is required'
  }

  return Object.keys(errors.value).length === 0
}

const handleSubmit = () => {
  if (validate()) {
    emit('submit', form.value)
  }
}

watch(() => qrCodeValue.value, () => {
  nextTick(() => renderQrCode())
})

onMounted(() => {
  nextTick(() => renderQrCode())
  loadAreas()
})
</script>
