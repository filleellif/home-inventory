<template>
  <aside class="w-64 bg-white border-r border-gray-200 h-screen sticky top-0">
    <nav class="mt-5 px-2">
      <NuxtLink
        to="/"
        class="group flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-50"
        :class="{ 'bg-gray-100 text-primary-600': isActive('/') }"
      >
        <svg class="mr-3 h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
        </svg>
        Items
      </NuxtLink>

      <NuxtLink
        to="/areas"
        class="group flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-50 mt-1"
        :class="{ 'bg-gray-100 text-primary-600': isActive('/areas') }"
      >
        <svg class="mr-3 h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
        </svg>
        Areas
      </NuxtLink>

      <NuxtLink
        to="/categories"
        class="group flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-50 mt-1"
        :class="{ 'bg-gray-100 text-primary-600': isActive('/categories') }"
      >
        <svg class="mr-3 h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
        </svg>
        Categories
      </NuxtLink>
    </nav>
  </aside>
</template>

<script setup lang="ts">
const route = useRoute()

const isActive = (path: string) => {
  if (path === '/') {
    return route.path === '/'
  }
  return route.path.startsWith(path)
}
</script>
