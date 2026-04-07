<script setup lang="ts">
import { X, Info, CheckCircle, AlertTriangle, AlertCircle } from 'lucide-vue-next'
import { useToast } from '@/composables/useToast'
import type { ToastItem } from '@/types'

const { toasts, remove } = useToast()

const typeConfig: Record<ToastItem['type'], { border: string; iconColor: string; icon: typeof Info }> = {
  info: { border: 'border-l-info', iconColor: 'text-info', icon: Info },
  success: { border: 'border-l-success', iconColor: 'text-success', icon: CheckCircle },
  warning: { border: 'border-l-warning', iconColor: 'text-warning', icon: AlertTriangle },
  error: { border: 'border-l-danger', iconColor: 'text-danger', icon: AlertCircle },
}
</script>

<template>
  <Teleport to="body">
    <div class="fixed top-4 right-4 z-[100] flex flex-col gap-2 pointer-events-none">
      <TransitionGroup
        name="toast"
        tag="div"
        class="flex flex-col gap-2"
      >
        <div
          v-for="toast in toasts"
          :key="toast.id"
          class="pointer-events-auto w-80 bg-surface-container rounded-lg shadow-xl border border-outline-variant border-l-4 p-4 flex items-start gap-3"
          :class="typeConfig[toast.type].border"
        >
          <component
            :is="typeConfig[toast.type].icon"
            class="w-5 h-5 shrink-0 mt-0.5"
            :class="typeConfig[toast.type].iconColor"
          />
          <div class="flex-1 min-w-0">
            <p class="text-sm font-semibold text-on-surface">{{ toast.title }}</p>
            <p class="text-xs text-on-surface-variant mt-0.5 leading-relaxed">{{ toast.message }}</p>
          </div>
          <button
            class="shrink-0 p-1 rounded text-on-surface-variant hover:text-on-surface hover:bg-surface-container-high transition-colors cursor-pointer"
            @click="remove(toast.id)"
          >
            <X class="w-3.5 h-3.5" />
          </button>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<style scoped>
.toast-enter-active {
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
.toast-leave-active {
  transition: all 0.2s ease-in;
}
.toast-enter-from {
  opacity: 0;
  transform: translateX(100%);
}
.toast-leave-to {
  opacity: 0;
  transform: translateX(100%);
}
.toast-move {
  transition: transform 0.3s ease;
}
</style>
