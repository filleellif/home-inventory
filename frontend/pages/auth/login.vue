<template>
  <div class="min-h-screen flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8 bg-gray-50">
    <div class="max-w-md w-full space-y-8">
      <div class="text-center">
        <h2 class="text-3xl font-extrabold text-gray-900">
          Sign in to your account
        </h2>
        <p class="mt-2 text-sm text-gray-600">
          Manage your home inventory
        </p>
      </div>

      <BaseCard>
        <LoginForm />
      </BaseCard>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'auth'
})
</script>
