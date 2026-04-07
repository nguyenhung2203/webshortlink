<script setup lang="ts">
import { useThemeStore } from '@/stores/theme'
import WxToast from '@/components/ui/WxToast.vue'

// Initialize theme store so dark class is applied on app load
const theme = useThemeStore()
</script>

<template>
  <RouterView />
  <WxToast />
</template>
