<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(defineProps<{
  modelValue?: number
  min?: number
  max?: number
  step?: number
  disabled?: boolean
}>(), {
  modelValue: 0,
  min: -Infinity,
  max: Infinity,
  step: 1,
  disabled: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: number]
}>()

const canDecrement = computed(() => props.modelValue - props.step >= props.min)
const canIncrement = computed(() => props.modelValue + props.step <= props.max)

function clamp(val: number) {
  return Math.min(props.max, Math.max(props.min, val))
}

function decrement() {
  if (canDecrement.value) emit('update:modelValue', clamp(props.modelValue - props.step))
}

function increment() {
  if (canIncrement.value) emit('update:modelValue', clamp(props.modelValue + props.step))
}

function onInput(e: Event) {
  const val = parseFloat((e.target as HTMLInputElement).value)
  if (!isNaN(val)) emit('update:modelValue', clamp(val))
}
</script>

<template>
  <div
    class="inline-flex items-center rounded-lg border border-outline-variant bg-surface-container-lowest h-10 transition-colors duration-200 focus-within:border-primary"
    :class="disabled && 'opacity-50 pointer-events-none'"
  >
    <button
      type="button"
      class="flex items-center justify-center w-9 h-full text-on-surface-variant hover:text-on-surface hover:bg-surface-container rounded-l-lg transition-colors cursor-pointer"
      :disabled="!canDecrement || disabled"
      :class="!canDecrement && 'opacity-30 cursor-not-allowed'"
      @click="decrement"
    >
      <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><path d="M5 12h14" /></svg>
    </button>
    <input
      type="number"
      :value="modelValue"
      :min="min"
      :max="max"
      :step="step"
      :disabled="disabled"
      class="w-16 text-center bg-transparent outline-none text-on-surface text-sm [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
      @input="onInput"
    />
    <button
      type="button"
      class="flex items-center justify-center w-9 h-full text-on-surface-variant hover:text-on-surface hover:bg-surface-container rounded-r-lg transition-colors cursor-pointer"
      :disabled="!canIncrement || disabled"
      :class="!canIncrement && 'opacity-30 cursor-not-allowed'"
      @click="increment"
    >
      <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><path d="M5 12h14" /><path d="M12 5v14" /></svg>
    </button>
  </div>
</template>
