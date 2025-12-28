<template>
  <div
    v-if="show"
    class="fixed inset-0 z-50 overflow-y-auto"
    aria-labelledby="modal-title"
    role="dialog"
    aria-modal="true"
  >
    <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
      <!-- Background overlay -->
      <div
        class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity"
        aria-hidden="true"
        @click="$emit('close')"
      ></div>

      <!-- Modal panel -->
      <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
        <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
          <div class="sm:flex sm:items-start">
            <div class="mt-3 text-center sm:mt-0 sm:text-left w-full">
              <h3 class="text-lg leading-6 font-medium text-gray-900" id="modal-title">
                Move {{ area?.name }}
              </h3>
              <div class="mt-4">
                <p class="text-sm text-gray-500 mb-4">
                  Select a new parent area or choose "None" to make it a root area.
                </p>
                <BaseSelect
                  v-model="selectedParentId"
                  label="New Parent Area"
                >
                  <option value="">None (Root Area)</option>
                  <option
                    v-for="availableArea in availableAreas"
                    :key="availableArea.id"
                    :value="availableArea.id"
                  >
                    {{ availableArea.fullPath }}
                  </option>
                </BaseSelect>
              </div>
            </div>
          </div>
        </div>
        <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse gap-2">
          <BaseButton @click="handleMove" :disabled="!hasChanged">
            Move
          </BaseButton>
          <BaseButton variant="secondary" @click="$emit('close')">
            Cancel
          </BaseButton>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { AreaDto, AreaTreeNode } from '~/types/area'

interface Props {
  show: boolean
  area: AreaTreeNode | null
  allAreas: AreaDto[]
}

const props = defineProps<Props>()
const emit = defineEmits<{
  close: []
  move: [areaId: string, newParentId: string | null]
}>()

const selectedParentId = ref<string>('')

// Initialize selected parent when area changes
watch(() => props.area, (newArea) => {
  if (newArea) {
    selectedParentId.value = newArea.parentAreaId || ''
  }
}, { immediate: true })

// Check if selection has changed
const hasChanged = computed(() => {
  if (!props.area) return false
  const currentParent = props.area.parentAreaId || ''
  return selectedParentId.value !== currentParent
})

// Get all areas that can be selected as parents (excluding the area itself and its descendants)
const availableAreas = computed(() => {
  if (!props.area) return []

  const excludedIds = new Set<string>()

  // Add the area itself
  excludedIds.add(props.area.id)

  // Add all descendants recursively
  const addDescendants = (node: AreaTreeNode) => {
    node.children.forEach(child => {
      excludedIds.add(child.id)
      addDescendants(child)
    })
  }
  addDescendants(props.area)

  // Filter out excluded areas
  return props.allAreas.filter(a => !excludedIds.has(a.id))
})

const handleMove = () => {
  if (!props.area) return

  const newParentId = selectedParentId.value || null
  emit('move', props.area.id, newParentId)
}
</script>
