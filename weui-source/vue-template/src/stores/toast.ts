import { ref } from 'vue'
import { defineStore } from 'pinia'
import type { ToastItem } from '@/types'

const MAX_TOASTS = 3

export const useToastStore = defineStore('toast', () => {
  const toasts = ref<ToastItem[]>([])

  function remove(id: number) {
    toasts.value = toasts.value.filter((t) => t.id !== id)
  }

  function add(type: ToastItem['type'], title: string, message: string) {
    const id = Date.now()
    const toast: ToastItem = { id, type, title, message }

    toasts.value.push(toast)

    // Enforce max visible toasts
    if (toasts.value.length > MAX_TOASTS) {
      toasts.value = toasts.value.slice(-MAX_TOASTS)
    }

    // Auto-remove after 5 seconds
    setTimeout(() => remove(id), 5000)
  }

  return { toasts, add, remove }
})
