<template>
  <div class="min-h-screen bg-gray-50">
    <AppHeader @toggle-sidebar="sidebarOpen = !sidebarOpen" />

    <div class="flex">
      <!-- Mobile sidebar -->
      <Transition
        enter-active-class="transition-transform duration-300"
        enter-from-class="-translate-x-full"
        enter-to-class="translate-x-0"
        leave-active-class="transition-transform duration-300"
        leave-from-class="translate-x-0"
        leave-to-class="-translate-x-full"
      >
        <div
          v-if="sidebarOpen"
          class="fixed inset-0 z-40 lg:hidden"
          @click="sidebarOpen = false"
        >
          <div class="absolute inset-0 bg-gray-600 opacity-75" />
          <div class="relative w-64" @click.stop>
            <AppSidebar />
          </div>
        </div>
      </Transition>

      <!-- Desktop sidebar -->
      <AppSidebar class="hidden lg:block" />

      <!-- Main content -->
      <main class="flex-1 p-6 lg:p-8">
        <slot />
      </main>
    </div>

    <!-- Toast notifications -->
    <div class="fixed bottom-4 right-4 space-y-2 z-50">
      <TransitionGroup
        enter-active-class="transition-all duration-300"
        enter-from-class="translate-x-full opacity-0"
        enter-to-class="translate-x-0 opacity-100"
        leave-active-class="transition-all duration-300"
        leave-from-class="translate-x-0 opacity-100"
        leave-to-class="translate-x-full opacity-0"
      >
        <div
          v-for="toast in toasts"
          :key="toast.id"
          class="px-4 py-3 rounded-lg shadow-lg max-w-sm"
          :class="{
            'bg-green-500 text-white': toast.type === 'success',
            'bg-red-500 text-white': toast.type === 'error',
            'bg-blue-500 text-white': toast.type === 'info'
          }"
        >
          {{ toast.message }}
        </div>
      </TransitionGroup>
    </div>
  </div>
</template>

<script setup lang="ts">
const { toasts } = useToast()
const sidebarOpen = ref(false)
</script>
