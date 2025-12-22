<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import QRCode from 'qrcode'
import { ArrowPathIcon, ArrowDownTrayIcon, ClipboardIcon } from '@heroicons/vue/24/outline'

const props = defineProps<{
  level: 'Room' | 'Shelf' | 'Box'
  modelValue?: string
  name?: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
  'update:name': [value: string]
}>()

const canvas = ref<HTMLCanvasElement>()
const isGenerating = ref(false)
const copySuccess = ref(false)

const generateQrCode = async () => {
  try {
    isGenerating.value = true
    const { $api } = useNuxtApp()
    const response = await $api('/items/generate-qr', {
      method: 'POST',
    })
    emit('update:modelValue', response.qrCode)
  } catch (err) {
    console.error('Failed to generate QR code:', err)
  } finally {
    isGenerating.value = false
  }
}

const renderQrCode = async () => {
  if (!props.modelValue || !canvas.value) return

  try {
    await QRCode.toCanvas(canvas.value, props.modelValue, {
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

const downloadLabel = async () => {
  if (!props.modelValue) return

  try {
    const url = `/api/items/qr-label/${encodeURIComponent(props.modelValue)}${
      props.name ? `?label=${encodeURIComponent(props.name)}` : ''
    }`

    const response = await fetch(url)
    const blob = await response.blob()
    const blobUrl = URL.createObjectURL(blob)

    const link = document.createElement('a')
    link.href = blobUrl
    link.download = `qr-label-${props.modelValue}.png`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    URL.revokeObjectURL(blobUrl)
  } catch (err) {
    console.error('Failed to download label:', err)
  }
}

const copyToClipboard = async () => {
  if (!props.modelValue) return

  try {
    await navigator.clipboard.writeText(props.modelValue)
    copySuccess.value = true
    setTimeout(() => {
      copySuccess.value = false
    }, 2000)
  } catch (err) {
    console.error('Failed to copy to clipboard:', err)
  }
}

watch(() => props.modelValue, () => {
  renderQrCode()
})

onMounted(() => {
  renderQrCode()
})
</script>

<template>
  <div class="space-y-3">
    <div class="flex items-center justify-between">
      <label class="block text-sm font-medium text-gray-700">
        {{ level }} QR Code
      </label>
      <button
        type="button"
        class="inline-flex items-center gap-1 rounded-md bg-white px-2.5 py-1.5 text-xs font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
        :disabled="isGenerating"
        @click="generateQrCode"
      >
        <ArrowPathIcon class="h-4 w-4" :class="{ 'animate-spin': isGenerating }" />
        Generate New
      </button>
    </div>

    <div v-if="modelValue" class="space-y-3">
      <div class="flex items-center justify-center p-4 bg-gray-50 rounded-lg border border-gray-200">
        <canvas ref="canvas" />
      </div>

      <div class="flex items-center justify-between p-3 bg-gray-50 rounded-lg border border-gray-200">
        <code class="text-sm font-mono text-gray-900">{{ modelValue }}</code>
        <button
          type="button"
          class="inline-flex items-center gap-1 rounded-md bg-white px-2.5 py-1.5 text-xs font-semibold shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
          :class="copySuccess ? 'text-green-700 ring-green-300' : 'text-gray-900'"
          @click="copyToClipboard"
        >
          <ClipboardIcon class="h-4 w-4" />
          {{ copySuccess ? 'Copied!' : 'Copy' }}
        </button>
      </div>

      <div class="flex gap-2">
        <button
          type="button"
          class="flex-1 inline-flex items-center justify-center gap-1 rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-600"
          @click="downloadLabel"
        >
          <ArrowDownTrayIcon class="h-4 w-4" />
          Download Label
        </button>
      </div>
    </div>

    <div v-else class="text-center py-8 bg-gray-50 rounded-lg border border-gray-200 border-dashed">
      <p class="text-sm text-gray-500">
        Click "Generate New" to create a QR code for this {{ level.toLowerCase() }}
      </p>
    </div>
  </div>
</template>
