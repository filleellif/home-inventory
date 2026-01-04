<template>
  <header class="bg-white shadow">
    <div class="mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex h-16 justify-between items-center">
        <div class="flex items-center">
          <button
            @click="$emit('toggle-sidebar')"
            class="lg:hidden mr-4 text-gray-500 hover:text-gray-700"
          >
            <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>

          <NuxtLink to="/" class="flex items-center">
            <span class="text-xl font-bold text-primary-600">Home Inventory</span>
          </NuxtLink>
        </div>

        <nav class="hidden md:flex items-center space-x-4">
          <NuxtLink to="/" class="text-gray-700 hover:text-primary-600 px-3 py-2 rounded-md text-sm font-medium">
            Items
          </NuxtLink>
          <NuxtLink to="/areas" class="text-gray-700 hover:text-primary-600 px-3 py-2 rounded-md text-sm font-medium">
            Areas
          </NuxtLink>
          <NuxtLink to="/categories" class="text-gray-700 hover:text-primary-600 px-3 py-2 rounded-md text-sm font-medium">
            Categories
          </NuxtLink>
        </nav>

        <div class="flex items-center">
          <button
            class="text-gray-700 hover:text-primary-600 px-3 py-2 rounded-md text-sm font-medium"
          >
            User Menu
          </button>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
defineEmits<{ 'toggle-sidebar': [] }>()
</script>
