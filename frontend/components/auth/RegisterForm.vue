<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
    <BaseInput
      v-model="form.name"
      label="Name"
      required
      :error="errors.name"
    />

    <BaseInput
      v-model="form.email"
      label="Email"
      type="email"
      required
      :error="errors.email"
    />

    <BaseInput
      v-model="form.password"
      label="Password"
      type="password"
      required
      :error="errors.password"
    />

    <BaseInput
      v-model="form.confirmPassword"
      label="Confirm Password"
      type="password"
      required
      :error="errors.confirmPassword"
    />

    <BaseButton type="submit" :loading="loading" class="w-full">
      Register
    </BaseButton>

    <p class="text-center text-sm text-gray-600">
      Already have an account?
      <NuxtLink to="/auth/login" class="text-primary-600 hover:text-primary-700 font-medium">
        Sign in here
      </NuxtLink>
    </p>
  </form>
</template>

<script setup lang="ts">
import type { RegisterCredentials } from '~/types/auth'

const authStore = useAuthStore()
const loading = ref(false)

const form = ref<RegisterCredentials>({
  name: '',
  email: '',
  password: '',
  confirmPassword: ''
})

const errors = ref<Record<string, string>>({})

const validate = () => {
  errors.value = {}

  if (!form.value.name) {
    errors.value.name = 'Name is required'
  }

  if (!form.value.email) {
    errors.value.email = 'Email is required'
  }

  if (!form.value.password) {
    errors.value.password = 'Password is required'
  } else if (form.value.password.length < 6) {
    errors.value.password = 'Password must be at least 6 characters'
  }

  if (form.value.password !== form.value.confirmPassword) {
    errors.value.confirmPassword = 'Passwords do not match'
  }

  return Object.keys(errors.value).length === 0
}

const handleSubmit = async () => {
  if (validate()) {
    loading.value = true
    try {
      await authStore.register(form.value)
    } finally {
      loading.value = false
    }
  }
}
</script>
