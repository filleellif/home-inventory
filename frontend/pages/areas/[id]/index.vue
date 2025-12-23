<template>
  <ClientOnly>
    <div v-if="pending" class="flex justify-center items-center h-64">
      <LoadingSpinner size="lg" />
    </div>

    <div v-else-if="area" class="space-y-6">
      <nav class="text-sm text-gray-500">
        <NuxtLink to="/areas" class="hover:text-gray-700">Areas</NuxtLink>
        <span class="mx-2">/</span>
        <span class="text-gray-900">{{ area.name }}</span>
      </nav>

      <div class="bg-white shadow rounded-lg p-6">
        <div class="flex justify-between items-start">
          <div>
            <h1 class="text-3xl font-bold">{{ area.name }}</h1>
            <p v-if="area.description" class="mt-2 text-gray-600">{{ area.description }}</p>
          </div>
          <div class="flex gap-2">
            <BaseButton variant="secondary" @click="navigateTo(`/areas/${id}/edit`)">
              Edit
            </BaseButton>
            <BaseButton variant="danger" @click="handleDelete">
              Delete
            </BaseButton>
          </div>
        </div>

        <dl class="mt-6 grid grid-cols-1 gap-x-4 gap-y-6 sm:grid-cols-2">
          <div v-if="area.fullPath">
            <dt class="text-sm font-medium text-gray-500">Location Path</dt>
            <dd class="mt-1 text-sm text-gray-900">{{ area.fullPath }}</dd>
          </div>

          <div v-if="area.parentAreaName">
            <dt class="text-sm font-medium text-gray-500">Parent Area</dt>
            <dd class="mt-1 text-sm text-gray-900">{{ area.parentAreaName }}</dd>
          </div>

          <div>
            <dt class="text-sm font-medium text-gray-500">QR Code</dt>
            <dd class="mt-1 text-sm text-gray-900 font-mono">{{ area.id }}</dd>
          </div>

          <div>
            <dt class="text-sm font-medium text-gray-500">Items in Area</dt>
            <dd class="mt-1 text-sm text-gray-900">
              {{ area.itemCount }} {{ area.itemCount === 1 ? 'item' : 'items' }}
            </dd>
          </div>

          <div>
            <dt class="text-sm font-medium text-gray-500">Child Areas</dt>
            <dd class="mt-1 text-sm text-gray-900">
              {{ area.childCount }} {{ area.childCount === 1 ? 'child' : 'children' }}
            </dd>
          </div>

          <div class="sm:col-span-2">
            <dt class="text-sm font-medium text-gray-500 mb-2">QR Code Label</dt>
            <dd class="flex items-center gap-4">
              <canvas ref="qrCanvas" />
              <BaseButton size="sm" @click="downloadQrLabel">
                Download Label
              </BaseButton>
            </dd>
          </div>

          <div class="sm:col-span-2">
            <dt class="text-sm font-medium text-gray-500">Last Updated</dt>
            <dd class="mt-1 text-sm text-gray-900">{{ formatDate(area.updatedAt) }}</dd>
          </div>
        </dl>
      </div>
    </div>

    <div v-else class="text-center py-12">
      <p class="text-gray-500">Area not found</p>
      <BaseButton class="mt-4" @click="navigateTo('/areas')">
        Back to Areas
      </BaseButton>
    </div>
  </ClientOnly>
</template>

<script setup lang="ts">
import QRCode from 'qrcode'

const route = useRoute()
const id = route.params.id as string

const { fetchArea, deleteArea } = useAreas()

const { data: area, pending } = await fetchArea(id)
const qrCanvas = ref<HTMLCanvasElement>()

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString()
}

const renderQrCode = async () => {
  console.log('Rendering QR code for', area.value?.id)
  if (!area.value?.id || !qrCanvas.value) return

  try {
    await QRCode.toCanvas(qrCanvas.value, area.value.id, {
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
  if (!area.value?.id) return

  try {
    const url = `/api/areas/qr-label/${encodeURIComponent(area.value.id)}${
      area.value.name ? `?label=${encodeURIComponent(area.value.name)}` : ''
    }`

    const response = await fetch(url)
    const blob = await response.blob()
    const blobUrl = URL.createObjectURL(blob)

    const link = document.createElement('a')
    link.href = blobUrl
    link.download = `qr-label-${area.value.id}.png`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    URL.revokeObjectURL(blobUrl)
  } catch (err) {
    console.error('Failed to download label:', err)
  }
}

const handleDelete = async () => {
  if (confirm('Are you sure you want to delete this area?')) {
    const result = await deleteArea(id)
    if (result.success) {
      navigateTo('/areas')
    }
  }
}

onMounted(() => {
  nextTick(() => renderQrCode())
})

watch(() => area.value, () => {
  nextTick(() => renderQrCode())
}, { immediate: true })
</script>
