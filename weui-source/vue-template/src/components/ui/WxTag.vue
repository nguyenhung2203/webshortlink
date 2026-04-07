<script setup lang="ts">
import { X } from 'lucide-vue-next'

type Variant = 'default' | 'primary' | 'success' | 'warning' | 'error' | 'info'
type Size = 'sm' | 'md'

const props = withDefaults(defineProps<{
  variant?: Variant
  size?: Size
  removable?: boolean
}>(), {
  variant: 'default',
  size: 'md',
  removable: false,
})

const emit = defineEmits<{
  remove: []
}>()

const variantClasses: Record<Variant, string> = {
  default: 'bg-surface-container text-on-surface-variant border border-outline-variant',
  primary: 'bg-blue-500/10 text-primary border border-primary/20',
  success: 'bg-emerald-500/10 text-emerald-500 border border-emerald-500/20',
  warning: 'bg-amber-500/10 text-amber-500 border border-amber-500/20',
  error: 'bg-rose-500/10 text-rose-500 border border-rose-500/20',
  info: 'bg-sky-500/10 text-sky-500 border border-sky-500/20',
}

const sizeClasses: Record<Size, string> = {
  sm: 'px-2.5 py-0.5 text-[10px] font-bold gap-1',
  md: 'px-2.5 py-0.5 text-[10px] font-bold gap-1.5',
}
</script>

<template>
  <span
    class="inline-flex items-center rounded font-medium transition-colors duration-150"
    :class="[variantClasses[variant], sizeClasses[size]]"
  >
    <slot />
    <button
      v-if="removable"
      class="inline-flex items-center justify-center rounded-full hover:bg-black/10 transition-colors cursor-pointer"
      :class="size === 'sm' ? 'w-3.5 h-3.5' : 'w-4 h-4'"
      @click.stop="emit('remove')"
    >
      <X :size="size === 'sm' ? 10 : 12" />
    </button>
  </span>
</template>
