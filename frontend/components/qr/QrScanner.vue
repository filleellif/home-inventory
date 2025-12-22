<script setup lang="ts">
import { ref, watch, onMounted, onBeforeUnmount } from 'vue'
import { Html5Qrcode } from 'html5-qrcode'
import {
  TransitionRoot,
  TransitionChild,
  Dialog,
  DialogPanel,
  DialogTitle,
} from '@headlessui/vue'
import { XMarkIcon, CameraIcon } from '@heroicons/vue/24/outline'

const props = defineProps<{
  open: boolean
}>()

const emit = defineEmits<{
  close: []
  scan: [qrCode: string]
}>()

const html5QrCode = ref<Html5Qrcode | null>(null)
const isScanning = ref(false)
const manualCode = ref('')
const error = ref('')
const cameraPermissionDenied = ref(false)

const startScanner = async () => {
  try {
    error.value = ''
    cameraPermissionDenied.value = false

    if (!html5QrCode.value) {
      html5QrCode.value = new Html5Qrcode('qr-reader')
    }

    await html5QrCode.value.start(
      { facingMode: 'environment' },
      {
        fps: 10,
        qrbox: { width: 250, height: 250 },
      },
      (decodedText) => {
        handleScan(decodedText)
      },
      () => {
        // Ignore scanning errors (happens frequently during scanning)
      }
    )

    isScanning.value = true
  } catch (err: any) {
    console.error('Failed to start scanner:', err)
    if (err?.name === 'NotAllowedError' || err?.message?.includes('Permission')) {
      cameraPermissionDenied.value = true
      error.value = 'Camera permission denied. Please use manual entry or grant camera access.'
    } else {
      error.value = 'Failed to start camera. Please use manual entry.'
    }
  }
}

const stopScanner = async () => {
  if (html5QrCode.value && isScanning.value) {
    try {
      await html5QrCode.value.stop()
      isScanning.value = false
    } catch (err) {
      console.error('Failed to stop scanner:', err)
    }
  }
}

const handleScan = (qrCode: string) => {
  emit('scan', qrCode)
  stopScanner()
  emit('close')
}

const handleManualSubmit = () => {
  if (manualCode.value.trim()) {
    emit('scan', manualCode.value.trim())
    manualCode.value = ''
    emit('close')
  }
}

const handleClose = () => {
  stopScanner()
  manualCode.value = ''
  error.value = ''
  emit('close')
}

watch(() => props.open, (isOpen) => {
  if (isOpen) {
    setTimeout(() => {
      startScanner()
    }, 100)
  } else {
    stopScanner()
  }
})

onBeforeUnmount(() => {
  stopScanner()
})
</script>

<template>
  <TransitionRoot :show="open" as="template">
    <Dialog as="div" class="relative z-50" @close="handleClose">
      <TransitionChild
        as="template"
        enter="ease-out duration-300"
        enter-from="opacity-0"
        enter-to="opacity-100"
        leave="ease-in duration-200"
        leave-from="opacity-100"
        leave-to="opacity-0"
      >
        <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" />
      </TransitionChild>

      <div class="fixed inset-0 z-50 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
          <TransitionChild
            as="template"
            enter="ease-out duration-300"
            enter-from="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
            enter-to="opacity-100 translate-y-0 sm:scale-100"
            leave="ease-in duration-200"
            leave-from="opacity-100 translate-y-0 sm:scale-100"
            leave-to="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
          >
            <DialogPanel class="relative transform overflow-hidden rounded-lg bg-white px-4 pb-4 pt-5 text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg sm:p-6">
              <div class="absolute right-0 top-0 pr-4 pt-4">
                <button
                  type="button"
                  class="rounded-md bg-white text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
                  @click="handleClose"
                >
                  <span class="sr-only">Close</span>
                  <XMarkIcon class="h-6 w-6" aria-hidden="true" />
                </button>
              </div>

              <div class="sm:flex sm:items-start">
                <div class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-blue-100 sm:mx-0 sm:h-10 sm:w-10">
                  <CameraIcon class="h-6 w-6 text-blue-600" aria-hidden="true" />
                </div>
                <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left w-full">
                  <DialogTitle as="h3" class="text-base font-semibold leading-6 text-gray-900">
                    Scan QR Code
                  </DialogTitle>
                  <div class="mt-4">
                    <div
                      v-if="!cameraPermissionDenied"
                      id="qr-reader"
                      class="w-full rounded-lg overflow-hidden border border-gray-300"
                    />

                    <div v-if="error" class="mt-3 text-sm text-red-600">
                      {{ error }}
                    </div>

                    <div class="mt-4">
                      <label for="manual-code" class="block text-sm font-medium text-gray-700 mb-2">
                        Or enter code manually:
                      </label>
                      <div class="flex gap-2">
                        <input
                          id="manual-code"
                          v-model="manualCode"
                          type="text"
                          placeholder="Enter QR code"
                          class="flex-1 rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                          @keyup.enter="handleManualSubmit"
                        />
                        <button
                          type="button"
                          class="inline-flex justify-center rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-600 disabled:opacity-50 disabled:cursor-not-allowed"
                          :disabled="!manualCode.trim()"
                          @click="handleManualSubmit"
                        >
                          Submit
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>
