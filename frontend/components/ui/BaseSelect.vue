<template>
  <div class="w-full">
    <label v-if="label" :for="selectId" class="block text-sm font-medium text-gray-700 mb-1">
      {{ label }}
      <span v-if="required" class="text-red-500">*</span>
    </label>
    <select
      :id="selectId"
      :value="modelValue"
      :required="required"
      :disabled="disabled"
      :class="selectClasses"
      @change="$emit('update:modelValue', ($event.target as HTMLSelectElement).value)"
    >
      <option v-if="placeholder" value="">{{ placeholder }}</option>
      <slot />
    </select>
    <p v-if="error" class="mt-1 text-sm text-red-600">{{ error }}</p>
  </div>
</template>

<script setup lang="ts">
interface Props {
  modelValue?: string
  label?: string
  placeholder?: string
  required?: boolean
  disabled?: boolean
  error?: string
}

const props = withDefaults(defineProps<Props>(), {
  required: false,
  disabled: false
})

defineEmits<{ 'update:modelValue': [value: string] }>()

const selectId = `select-${Math.random().toString(36).substr(2, 9)}`

const selectClasses = computed(() => {
  const base = 'block w-full rounded-md shadow-sm focus:ring-primary-500 focus:border-primary-500 sm:text-sm'
  const error = props.error ? 'border-red-300' : 'border-gray-300'
  return `${base} ${error}`
})
</script>
