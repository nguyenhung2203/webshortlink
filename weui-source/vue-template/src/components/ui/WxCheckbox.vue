<script setup lang="ts">
const props = withDefaults(defineProps<{
  modelValue?: boolean
  disabled?: boolean
  indeterminate?: boolean
  label?: string
}>(), {
  modelValue: false,
  disabled: false,
  indeterminate: false,
  label: '',
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

function toggle() {
  if (!props.disabled) emit('update:modelValue', !props.modelValue)
}
</script>

<template>
  <label
    class="inline-flex items-center gap-2 select-none"
    :class="disabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer'"
    @click.prevent="toggle"
  >
    <span
      class="flex items-center justify-center w-[18px] h-[18px] rounded border-2 transition-colors duration-200 shrink-0"
      :class="
        modelValue || indeterminate
          ? 'bg-primary border-primary'
          : 'border-outline-variant'
      "
    >
      <!-- Check icon -->
      <svg
        v-if="modelValue && !indeterminate"
        class="w-3 h-3 text-white"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="3"
        stroke-linecap="round"
        stroke-linejoin="round"
      >
        <path d="M20 6 9 17l-5-5" />
      </svg>
      <!-- Minus icon (indeterminate) -->
      <svg
        v-else-if="indeterminate"
        class="w-3 h-3 text-white"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="3"
        stroke-linecap="round"
      >
        <path d="M5 12h14" />
      </svg>
    </span>
    <input type="checkbox" :checked="modelValue" :disabled="disabled" class="sr-only" />
    <span v-if="label" class="text-sm text-on-surface">{{ label }}</span>
  </label>
</template>
