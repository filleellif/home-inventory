<template>
  <Transition
    enter-active-class="transition-opacity duration-200"
    leave-active-class="transition-opacity duration-200"
    enter-from-class="opacity-0"
    leave-to-class="opacity-0"
  >
    <div
      v-if="isLoading"
      class="fixed top-0 left-0 right-0 z-50"
    >
      <div class="h-1 bg-blue-600 loading-bar" />
    </div>
  </Transition>
</template>

<script setup lang="ts">
const { isLoading } = useLoading()
</script>

<style scoped>
.loading-bar {
  animation: loading 1.5s ease-in-out infinite;
  transform-origin: left;
}

@keyframes loading {
  0% {
    transform: scaleX(0);
  }
  50% {
    transform: scaleX(0.6);
  }
  100% {
    transform: scaleX(1);
  }
}
</style>
