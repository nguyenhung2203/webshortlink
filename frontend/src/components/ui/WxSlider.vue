<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(defineProps<{
  modelValue?: number
  min?: number
  max?: number
  step?: number
  disabled?: boolean
  showValue?: boolean
}>(), {
  modelValue: 0,
  min: 0,
  max: 100,
  step: 1,
  disabled: false,
  showValue: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: number]
}>()

const percentage = computed(() =>
  ((props.modelValue - props.min) / (props.max - props.min)) * 100
)

function onInput(e: Event) {
  emit('update:modelValue', parseFloat((e.target as HTMLInputElement).value))
}
</script>

<template>
  <div class="flex items-center gap-3" :class="disabled && 'opacity-50'">
    <div class="relative flex-1 flex items-center">
      <input
        type="range"
        :value="modelValue"
        :min="min"
        :max="max"
        :step="step"
        :disabled="disabled"
        class="w-full h-2 appearance-none rounded-full bg-outline-variant cursor-pointer
          [&::-webkit-slider-thumb]:appearance-none
          [&::-webkit-slider-thumb]:w-5
          [&::-webkit-slider-thumb]:h-5
          [&::-webkit-slider-thumb]:rounded-full
          [&::-webkit-slider-thumb]:bg-primary
          [&::-webkit-slider-thumb]:shadow-md
          [&::-webkit-slider-thumb]:cursor-pointer
          [&::-webkit-slider-thumb]:border-2
          [&::-webkit-slider-thumb]:border-white
          [&::-webkit-slider-thumb]:transition-transform
          [&::-webkit-slider-thumb]:duration-150
          [&::-webkit-slider-thumb]:hover:scale-110
          [&::-moz-range-thumb]:w-5
          [&::-moz-range-thumb]:h-5
          [&::-moz-range-thumb]:rounded-full
          [&::-moz-range-thumb]:bg-primary
          [&::-moz-range-thumb]:border-2
          [&::-moz-range-thumb]:border-white
          [&::-moz-range-thumb]:shadow-md
          [&::-moz-range-thumb]:cursor-pointer
          [&::-moz-range-track]:bg-outline-variant
          [&::-moz-range-track]:rounded-full
          [&::-moz-range-track]:h-2"
        :style="{
          background: `linear-gradient(to right, var(--color-primary, #00f2ff) ${percentage}%, var(--color-outline-variant, #444) ${percentage}%)`,
        }"
        @input="onInput"
      />
    </div>
    <span v-if="showValue" class="text-sm text-on-surface tabular-nums min-w-[2.5rem] text-right">
      {{ modelValue }}
    </span>
  </div>
</template>
