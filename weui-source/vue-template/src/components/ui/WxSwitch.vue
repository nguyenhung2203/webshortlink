<script setup lang="ts">
type Size = 'sm' | 'md' | 'lg'

const props = withDefaults(defineProps<{
  modelValue?: boolean
  disabled?: boolean
  size?: Size
  label?: string
}>(), {
  modelValue: false,
  disabled: false,
  size: 'md',
  label: '',
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const trackSize: Record<Size, string> = {
  sm: 'w-8 h-4',
  md: 'w-10 h-5',
  lg: 'w-12 h-6',
}

const thumbSize: Record<Size, string> = {
  sm: 'w-2.5 h-2.5',
  md: 'w-3 h-3',
  lg: 'w-4 h-4',
}

const thumbTranslate: Record<Size, string> = {
  sm: 'translate-x-[14px]',
  md: 'translate-x-5',
  lg: 'translate-x-6',
}
</script>

<template>
  <label
    class="inline-flex items-center gap-2 select-none"
    :class="[disabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer']"
  >
    <button
      type="button"
      role="switch"
      :aria-checked="modelValue"
      :disabled="disabled"
      class="relative inline-flex items-center rounded-full p-0.5 transition-colors duration-200"
      :class="[trackSize[size], modelValue ? 'bg-gradient-to-r from-cyan-400 to-blue-600 shadow-md shadow-primary/20' : 'bg-surface-container']"
      @click="emit('update:modelValue', !modelValue)"
    >
      <span
        class="block rounded-full bg-white shadow-sm top-1 transition-transform duration-200"
        :class="[thumbSize[size], modelValue ? thumbTranslate[size] : 'translate-x-0']"
      />
    </button>
    <span v-if="label" class="text-xs text-on-surface-variant opacity-80 font-medium">{{ label }}</span>
  </label>
</template>
