<script setup lang="ts">
import { computed } from 'vue'

type Variant = 'primary' | 'success' | 'warning' | 'danger'
type Size = 'sm' | 'md' | 'lg'

const props = withDefaults(defineProps<{
  value?: number
  variant?: Variant
  size?: Size
  showValue?: boolean
}>(), {
  value: 0,
  variant: 'primary',
  size: 'md',
  showValue: false,
})

const sizeClasses: Record<Size, string> = {
  sm: 'h-1',
  md: 'h-2',
  lg: 'h-3',
}

const variantClasses: Record<Variant, string> = {
  primary: 'bg-primary',
  success: 'bg-success',
  warning: 'bg-warning',
  danger: 'bg-danger',
}

const clampedValue = computed(() => Math.min(100, Math.max(0, props.value)))
</script>

<template>
  <div class="flex flex-col gap-1">
    <div class="flex items-center justify-between" v-if="showValue">
      <slot />
      <span class="text-xs text-on-surface-variant font-medium">{{ clampedValue }}%</span>
    </div>
    <div
      class="w-full rounded-full overflow-hidden bg-surface-container"
      :class="sizeClasses[size]"
    >
      <div
        class="h-full transition-all duration-500 ease-out"
        :class="variantClasses[variant]"
        :style="{ width: `${clampedValue}%` }"
      />
    </div>
  </div>
</template>
