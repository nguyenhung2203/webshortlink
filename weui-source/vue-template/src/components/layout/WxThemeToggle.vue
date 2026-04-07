<script setup lang="ts">
import { useThemeStore } from '@/stores/theme'
import { Sun, Moon } from 'lucide-vue-next'

const theme = useThemeStore()
</script>

<template>
  <button
    class="relative flex h-9 w-9 items-center justify-center rounded-full text-white/90 transition-colors hover:bg-white/15 hover:text-white"
    :title="theme.isDark ? 'Switch to light mode' : 'Switch to dark mode'"
    @click="theme.toggle"
  >
    <Transition name="theme-icon" mode="out-in">
      <Moon v-if="theme.isDark" :size="18" key="moon" />
      <Sun v-else :size="18" key="sun" />
    </Transition>
  </button>
</template>

<style scoped>
.theme-icon-enter-active,
.theme-icon-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}
.theme-icon-enter-from {
  opacity: 0;
  transform: rotate(-90deg) scale(0.6);
}
.theme-icon-leave-to {
  opacity: 0;
  transform: rotate(90deg) scale(0.6);
}
</style>
