<template>
  <form @submit.prevent="handleSubmit" class="space-y-6">
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

    <BaseButton type="submit" :loading="loading" class="w-full">
      Sign In
    </BaseButton>

    <p class="text-center text-sm text-gray-600">
      Don't have an account?
      <NuxtLink to="/auth/register" class="text-primary-600 hover:text-primary-700 font-medium">
        Register here
      </NuxtLink>
    </p>
  </form>
</template>

<script setup lang="ts">
import type { LoginCredentials } from '~/types/auth'

const authStore = useAuthStore()
const loading = ref(false)

const form = ref<LoginCredentials>({
  email: '',
  password: ''
})

const errors = ref<Record<string, string>>({})

const validate = () => {
  errors.value = {}

  if (!form.value.email) {
    errors.value.email = 'Email is required'
  }

  if (!form.value.password) {
    errors.value.password = 'Password is required'
  }

  return Object.keys(errors.value).length === 0
}

const handleSubmit = async () => {
  if (validate()) {
    loading.value = true
    try {
      await authStore.login(form.value)
    } finally {
      loading.value = false
    }
  }
}
</script>
