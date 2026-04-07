<script setup lang="ts">
import { computed } from 'vue'

type Variant = 'primary' | 'cta' | 'secondary' | 'ghost' | 'danger' | 'success' | 'text'
type Size = 'sm' | 'md' | 'lg'

const props = withDefaults(defineProps<{
  variant?: Variant
  size?: Size
  loading?: boolean
  disabled?: boolean
  to?: string | object
  href?: string
  type?: 'button' | 'submit' | 'reset'
}>(), {
  variant: 'primary',
  size: 'md',
  loading: false,
  disabled: false,
  type: 'button',
})

const sizeClasses: Record<Size, string> = {
  sm: 'px-3 py-1 text-[10px] gap-1.5',
  md: 'px-6 py-2 text-sm gap-2',
  lg: 'px-10 py-3 text-lg gap-2.5',
}

const variantClasses: Record<Variant, string> = {
  primary: 'bg-gradient-to-r from-cyan-400 to-blue-600 text-white font-bold shadow-lg shadow-blue-500/25 hover:-translate-y-0.5 hover:shadow-blue-500/40 active:translate-y-0.5 active:scale-[0.98]',
  cta: 'bg-gradient-to-r from-cyan-400 to-blue-700 text-white font-bold shadow-xl shadow-blue-600/30 hover:-translate-y-0.5 hover:shadow-blue-600/50 active:translate-y-0.5 active:scale-[0.98]',
  secondary: 'bg-white border border-slate-100 text-slate-600 font-bold shadow-sm hover:bg-slate-50 hover:-translate-y-0.5 active:translate-y-0.5 active:scale-[0.98]',
  ghost: 'bg-transparent text-slate-500 font-bold hover:text-slate-700',
  danger: 'bg-[#F43F5E] text-white font-bold shadow-lg shadow-rose-500/25 hover:-translate-y-0.5 active:translate-y-0.5 active:scale-[0.98]',
  success: 'bg-[#10B981] text-white font-bold shadow-lg shadow-emerald-500/25 hover:-translate-y-0.5 active:translate-y-0.5 active:scale-[0.98]',
  text: 'bg-transparent text-slate-500 font-bold hover:text-primary',
}

const classes = computed(() => [
  'inline-flex items-center justify-center rounded-md font-medium transition-all duration-200 cursor-pointer select-none border-0',
  sizeClasses[props.size],
  variantClasses[props.variant],
  props.disabled && !props.loading && 'opacity-50 cursor-not-allowed pointer-events-none',
  props.loading && 'opacity-50 pointer-events-none',
])

const componentType = computed(() => {
  if (props.to) return 'router-link'
  if (props.href) return 'a'
  return 'button'
})
</script>

<template>
  <component
    :is="componentType"
    :to="to"
    :href="href"
    :type="componentType === 'button' ? type : undefined"
    :class="classes" 
    :disabled="disabled || loading"
  >
    <svg
      v-if="loading"
      class="animate-spin shrink-0"
      :class="size === 'sm' ? 'w-3 h-3' : size === 'lg' ? 'w-5 h-5' : 'w-4 h-4'"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      stroke-width="2"
    >
      <path d="M21 12a9 9 0 1 1-6.219-8.56" stroke-linecap="round" />
    </svg>
    <slot v-if="$slots.icon && !loading" name="icon" />
    <slot />
  </component>
</template>
