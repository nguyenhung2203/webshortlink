import { ref, watch } from 'vue'
import { defineStore } from 'pinia'

export const useThemeStore = defineStore('theme', () => {
  const stored = localStorage.getItem('theme-dark')
  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
  const isDark = ref(stored !== null ? stored === 'true' : prefersDark)

  function applyTheme(dark: boolean) {
    document.documentElement.classList.toggle('dark', dark)
    localStorage.setItem('theme-dark', String(dark))
  }

  // Apply on init
  applyTheme(isDark.value)

  watch(isDark, (val) => applyTheme(val))

  function toggle() {
    isDark.value = !isDark.value
  }

  return { isDark, toggle }
})
