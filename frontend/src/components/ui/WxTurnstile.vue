<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'

declare global {
  interface Window {
    turnstile?: {
      render: (container: string | HTMLElement, options: Record<string, unknown>) => string
      remove: (widgetId: string) => void
      reset: (widgetId?: string) => void
    }
    __wxTurnstileReady?: Promise<void>
  }
}

const props = defineProps<{
  modelValue: string | null
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string | null]
}>()

const siteKey = import.meta.env.VITE_TURNSTILE_SITE_KEY?.trim() ?? ''
const isEnabled = computed(() => siteKey.length > 0)
const container = ref<HTMLElement | null>(null)
let widgetId: string | null = null

function ensureScript(): Promise<void> {
  if (typeof window === 'undefined') {
    return Promise.resolve()
  }

  if (window.turnstile) {
    return Promise.resolve()
  }

  if (!window.__wxTurnstileReady) {
    window.__wxTurnstileReady = new Promise<void>((resolve, reject) => {
      const existing = document.querySelector<HTMLScriptElement>('script[data-wx-turnstile="true"]')
      if (existing) {
        existing.addEventListener('load', () => resolve(), { once: true })
        existing.addEventListener('error', () => reject(new Error('Failed to load Turnstile.')), { once: true })
        return
      }

      const script = document.createElement('script')
      script.src = 'https://challenges.cloudflare.com/turnstile/v0/api.js?render=explicit'
      script.async = true
      script.defer = true
      script.dataset.wxTurnstile = 'true'
      script.onload = () => resolve()
      script.onerror = () => reject(new Error('Failed to load Turnstile.'))
      document.head.appendChild(script)
    })
  }

  return window.__wxTurnstileReady
}

async function renderWidget() {
  if (!isEnabled.value || !container.value) {
    return
  }

  await ensureScript()
  if (!window.turnstile || widgetId) {
    return
  }

  widgetId = window.turnstile.render(container.value, {
    sitekey: siteKey,
    theme: 'light',
    callback: (token: string) => emit('update:modelValue', token),
    'expired-callback': () => emit('update:modelValue', null),
    'timeout-callback': () => emit('update:modelValue', null),
    'error-callback': () => emit('update:modelValue', null),
  })
}

function resetWidget() {
  emit('update:modelValue', null)
  if (widgetId && window.turnstile) {
    window.turnstile.reset(widgetId)
  }
}

defineExpose({
  reset: resetWidget,
})

watch(() => props.modelValue, (value) => {
  if (!value && widgetId && window.turnstile) {
    window.turnstile.reset(widgetId)
  }
})

onMounted(() => {
  void renderWidget()
})

onBeforeUnmount(() => {
  if (widgetId && window.turnstile) {
    window.turnstile.remove(widgetId)
  }
})
</script>

<template>
  <div v-if="isEnabled" class="space-y-2">
    <div ref="container" class="min-h-[65px]" />
    <p class="text-xs text-on-surface-variant">
      Hoàn tất xác minh bảo mật trước khi gửi biểu mẫu.
    </p>
  </div>
</template>
