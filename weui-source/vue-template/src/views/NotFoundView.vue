<script setup lang="ts">
import WxButton from '@/components/ui/WxButton.vue'
</script>

<template>
  <div class="min-h-[60vh] flex flex-col items-center justify-center text-center px-4">
    <p class="text-[8rem] font-black leading-none tracking-tighter bg-gradient-to-r from-cyan-400 to-blue-600 bg-clip-text text-transparent select-none">
      404
    </p>
    <h1 class="text-2xl font-bold text-on-surface mt-2">Page not found</h1>
    <p class="text-sm text-on-surface-variant mt-2 max-w-md">
      The page you are looking for does not exist or has been moved. Please check the URL or navigate back to the dashboard.
    </p>
    <div class="mt-8 flex gap-3">
      <WxButton variant="primary" @click="$router.push('/')">Go to Dashboard</WxButton>
      <WxButton variant="secondary" @click="$router.back()">Go Back</WxButton>
    </div>
  </div>
</template>
