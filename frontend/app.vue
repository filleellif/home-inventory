<template>
  <div>
    <!-- LoadingOverlay must be outside ClientOnly to block before hydration -->
    <LoadingOverlay />
    <ClientOnly>
      <LoadingIndicator />
    </ClientOnly>
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>
