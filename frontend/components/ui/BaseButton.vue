<template>
  <button
    :type="type"
    :disabled="disabled || loading"
    :class="buttonClasses"
    @click="$emit('click', $event)"
  >
    <LoadingSpinner v-if="loading" class="w-4 h-4 mr-2" />
    <slot />
  </button>
</template>

<script setup lang="ts">
interface Props {
  variant?: 'primary' | 'secondary' | 'danger'
  type?: 'button' | 'submit'
  disabled?: boolean
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  type: 'button',
  disabled: false,
  loading: false
})

defineEmits<{ click: [event: MouseEvent] }>()

const buttonClasses = computed(() => {
  const base = 'inline-flex items-center justify-center px-4 py-2 rounded-md font-medium focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors'

  const variants = {
    primary: 'bg-primary-600 text-white hover:bg-primary-700 focus:ring-primary-500',
    secondary: 'bg-gray-200 text-gray-900 hover:bg-gray-300 focus:ring-gray-500',
    danger: 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-500'
  }

  return `${base} ${variants[props.variant]}`
})
</script>
