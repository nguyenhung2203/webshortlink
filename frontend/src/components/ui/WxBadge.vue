<script setup lang="ts">
import { computed, useSlots } from 'vue'

type Variant = 'info' | 'success' | 'warning' | 'error' | 'neutral'
type Size = 'sm' | 'md' | 'lg'

const props = withDefaults(defineProps<{
  variant?: Variant
  size?: Size
  dot?: boolean
  max?: number
}>(), {
  variant: 'info',
  size: 'md',
  dot: false,
})

const slots = useSlots()

const variantClasses: Record<Variant, string> = {
  info: 'bg-info text-white',
  success: 'bg-success text-white',
  warning: 'bg-warning text-white',
  error: 'bg-danger text-white',
  neutral: 'bg-surface-container-high text-on-surface',
}

const sizeClasses: Record<Size, string> = {
  sm: 'min-w-[1rem] h-4 text-[10px] px-1',
  md: 'min-w-[1.25rem] h-5 text-[10px] px-1.5',
  lg: 'min-w-[1.5rem] h-6 text-sm px-2',
}

const dotSizeClasses: Record<Size, string> = {
  sm: 'w-1.5 h-1.5',
  md: 'w-2.5 h-2.5',
  lg: 'w-2.5 h-2.5',
}

const displayValue = computed(() => {
  const slotContent = slots.default?.()
  if (!slotContent?.length) return ''
  const text = slotContent[0]?.children?.toString() ?? ''
  const num = Number(text)
  if (props.max && !isNaN(num) && num > props.max) return `${props.max}+`
  return text
})
</script>

<template>
  <span
    v-if="dot"
    class="inline-block rounded-full"
    :class="[variantClasses[variant], dotSizeClasses[size]]"
  />
  <span
    v-else
    class="inline-flex items-center justify-center rounded-full font-bold uppercase leading-none"
    :class="[variantClasses[variant], sizeClasses[size]]"
  >
    <template v-if="max">{{ displayValue }}</template>
    <slot v-else />
  </span>
</template>
