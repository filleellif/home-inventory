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
      rows="3"
    />

    <BaseSelect
      v-model="form.parentCategoryId"
      label="Parent Category"
      placeholder="None (Top level)"
    >
      <option v-for="category in sortedCategories" :key="category.id" :value="category.id">
        {{ category.displayName }}
      </option>
    </BaseSelect>

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
import type { CategoryDto, CreateCategoryDto } from '~/types/category'

interface Props {
  initialData?: CategoryDto
  categories?: CategoryDto[]
  loading?: boolean
}

const props = defineProps<Props>()
const emit = defineEmits<{
  submit: [data: CreateCategoryDto]
  cancel: []
}>()

const form = ref<CreateCategoryDto>({
  name: props.initialData?.name || '',
  description: props.initialData?.description || '',
  parentCategoryId: props.initialData?.parentCategoryId
})

const errors = ref<Record<string, string>>({})

// Build hierarchical sorted list for dropdown
const { buildCategoryTree } = useCategories()

const sortedCategories = computed(() => {
  if (!props.categories) return []

  // Filter out current category to prevent self-reference
  const availableCategories = props.categories.filter(
    cat => cat.id !== props.initialData?.id
  )

  const tree = buildCategoryTree(availableCategories)
  const flattened: Array<CategoryDto & { displayName: string }> = []

  const flatten = (nodes: any[], depth = 0) => {
    nodes.forEach(node => {
      // Add indentation prefix based on depth
      const prefix = '  '.repeat(depth)
      flattened.push({
        ...node,
        displayName: `${prefix}${node.name}`
      })
      if (node.children?.length > 0) {
        flatten(node.children, depth + 1)
      }
    })
  }

  flatten(tree)
  return flattened
})

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
</script>
