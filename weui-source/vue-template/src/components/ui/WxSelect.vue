<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'

type Option = { label: string; value: string | number; disabled?: boolean }
type Size = 'sm' | 'md' | 'lg'

const props = withDefaults(defineProps<{
  modelValue?: string | number | null
  options?: Option[]
  placeholder?: string
  disabled?: boolean
  size?: Size
  error?: boolean
}>(), {
  modelValue: null,
  options: () => [],
  placeholder: 'Select...',
  disabled: false,
  size: 'md',
  error: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string | number]
}>()

const open = ref(false)
const wrapperRef = ref<HTMLElement | null>(null)

const selectedLabel = computed(() => {
  const opt = props.options.find((o) => o.value === props.modelValue)
  return opt ? opt.label : ''
})

const sizeClasses: Record<Size, string> = {
  sm: 'h-8 text-xs px-2.5',
  md: 'h-10 text-sm px-3',
  lg: 'h-12 text-lg px-4',
}

function select(opt: Option) {
  if (opt.disabled) return
  emit('update:modelValue', opt.value)
  open.value = false
}

function onClickOutside(e: MouseEvent) {
  if (wrapperRef.value && !wrapperRef.value.contains(e.target as Node)) {
    open.value = false
  }
}

onMounted(() => document.addEventListener('click', onClickOutside))
onBeforeUnmount(() => document.removeEventListener('click', onClickOutside))
</script>

<template>
  <div ref="wrapperRef" class="relative inline-block w-full">
    <button
      type="button"
      class="flex items-center justify-between w-full rounded-lg border bg-surface-container-lowest transition-colors duration-200 cursor-pointer"
      :class="[
        sizeClasses[size],
        error ? 'border-danger' : open ? 'border-primary' : 'border-outline-variant',
        disabled && 'opacity-50 pointer-events-none',
      ]"
      @click="open = !open"
    >
      <span :class="selectedLabel ? 'text-on-surface' : 'text-on-surface-variant/50'">
        {{ selectedLabel || placeholder }}
      </span>
      <svg class="w-4 h-4 text-on-surface-variant shrink-0 transition-transform duration-200" :class="open && 'rotate-180'" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <path d="m6 9 6 6 6-6" />
      </svg>
    </button>

    <div
      v-if="open"
      class="absolute z-50 mt-1 w-full rounded-lg border border-outline-variant bg-surface-container-lowest shadow-lg py-1 max-h-60 overflow-y-auto"
    >
      <div
        v-for="opt in options"
        :key="opt.value"
        class="px-3 py-2 text-sm cursor-pointer transition-colors"
        :class="[
          opt.disabled ? 'opacity-40 cursor-not-allowed' : 'hover:bg-surface-container',
          opt.value === modelValue ? 'text-primary font-medium' : 'text-on-surface',
        ]"
        @click="select(opt)"
      >
        {{ opt.label }}
      </div>
    </div>
  </div>
</template>
