<script setup lang="ts">
import { Info, CheckCircle2, AlertTriangle, XCircle, X } from 'lucide-vue-next'
import { computed } from 'vue'

type AlertType = 'info' | 'success' | 'warning' | 'error'

const props = withDefaults(defineProps<{
  type?: AlertType
  title?: string
  closable?: boolean
}>(), {
  type: 'info',
  closable: false,
})

const emit = defineEmits<{
  close: []
}>()

const typeConfig: Record<AlertType, { border: string; bg: string; text: string }> = {
  info: { border: 'border-l-info', bg: 'bg-info/10', text: 'text-info' },
  success: { border: 'border-l-success', bg: 'bg-success/10', text: 'text-success' },
  warning: { border: 'border-l-warning', bg: 'bg-warning/10', text: 'text-warning' },
  error: { border: 'border-l-danger', bg: 'bg-danger/10', text: 'text-danger' },
}

const icons = { info: Info, success: CheckCircle2, warning: AlertTriangle, error: XCircle }

const config = computed(() => typeConfig[props.type])
const IconComponent = computed(() => icons[props.type])
</script>

<template>
  <div
    class="flex gap-3 rounded-lg border-l-4 p-4"
    :class="[config.border, config.bg]"
  >
    <component :is="IconComponent" :size="20" class="shrink-0 mt-0.5" :class="config.text" />
    <div class="flex-1 min-w-0">
      <p v-if="title" class="font-semibold text-sm text-on-surface mb-0.5">{{ title }}</p>
      <div class="text-sm text-on-surface-variant">
        <slot />
      </div>
    </div>
    <button
      v-if="closable"
      class="shrink-0 text-on-surface-variant hover:text-on-surface transition-colors cursor-pointer"
      @click="emit('close')"
    >
      <X :size="16" />
    </button>
  </div>
</template>
