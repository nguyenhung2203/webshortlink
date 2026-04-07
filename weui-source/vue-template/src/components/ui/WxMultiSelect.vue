<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'

type Option = { label: string; value: string | number; disabled?: boolean }

const props = withDefaults(defineProps<{
  modelValue?: (string | number)[]
  options?: Option[]
  placeholder?: string
  disabled?: boolean
  error?: boolean
}>(), {
  modelValue: () => [],
  options: () => [],
  placeholder: 'Select...',
  disabled: false,
  error: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: (string | number)[]]
}>()

const open = ref(false)
const wrapperRef = ref<HTMLElement | null>(null)

const selectedOptions = computed(() =>
  props.options.filter((o) => props.modelValue.includes(o.value))
)

function toggle(opt: Option) {
  if (opt.disabled) return
  const idx = props.modelValue.indexOf(opt.value)
  const next = [...props.modelValue]
  if (idx === -1) next.push(opt.value)
  else next.splice(idx, 1)
  emit('update:modelValue', next)
}

function removeTag(val: string | number) {
  emit('update:modelValue', props.modelValue.filter((v) => v !== val))
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
  <div ref="wrapperRef" class="relative w-full">
    <div
      class="flex flex-wrap items-center gap-1.5 min-h-[2.5rem] rounded-lg border px-3 py-1.5 bg-surface-container-lowest cursor-pointer transition-colors duration-200"
      :class="[
        error ? 'border-danger' : open ? 'border-primary' : 'border-outline-variant',
        disabled && 'opacity-50 pointer-events-none',
      ]"
      @click="open = !open"
    >
      <span
        v-for="sel in selectedOptions"
        :key="sel.value"
        class="inline-flex items-center gap-1 bg-surface-container rounded px-2 py-0.5 text-xs text-on-surface"
      >
        {{ sel.label }}
        <button type="button" class="hover:text-danger cursor-pointer" @click.stop="removeTag(sel.value)">
          <svg class="w-3 h-3" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><path d="M18 6 6 18" /><path d="m6 6 12 12" /></svg>
        </button>
      </span>
      <span v-if="!selectedOptions.length" class="text-sm text-on-surface-variant/50">{{ placeholder }}</span>
      <svg class="w-4 h-4 text-on-surface-variant shrink-0 ml-auto transition-transform duration-200" :class="open && 'rotate-180'" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><path d="m6 9 6 6 6-6" /></svg>
    </div>

    <div
      v-if="open"
      class="absolute z-50 mt-1 w-full rounded-lg border border-outline-variant bg-surface-container-lowest shadow-lg py-1 max-h-60 overflow-y-auto"
    >
      <div
        v-for="opt in options"
        :key="opt.value"
        class="flex items-center gap-2 px-3 py-2 text-sm cursor-pointer transition-colors"
        :class="opt.disabled ? 'opacity-40 cursor-not-allowed' : 'hover:bg-surface-container text-on-surface'"
        @click="toggle(opt)"
      >
        <span
          class="flex items-center justify-center w-4 h-4 rounded border shrink-0 transition-colors"
          :class="modelValue.includes(opt.value) ? 'bg-primary border-primary' : 'border-outline-variant'"
        >
          <svg v-if="modelValue.includes(opt.value)" class="w-3 h-3 text-white" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"><path d="M20 6 9 17l-5-5" /></svg>
        </span>
        {{ opt.label }}
      </div>
    </div>
  </div>
</template>
