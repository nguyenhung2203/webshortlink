<script setup lang="ts">
import { Calendar } from 'lucide-vue-next'

withDefaults(defineProps<{
  modelValue?: string
  placeholder?: string
  disabled?: boolean
}>(), {
  modelValue: '',
  placeholder: 'Select a date',
  disabled: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()
</script>

<template>
  <div
    class="flex items-center h-10 rounded-lg border transition-colors duration-200 bg-surface-container-lowest px-3"
    :class="[
      'border-outline-variant focus-within:border-primary',
      disabled && 'opacity-50 pointer-events-none',
    ]"
  >
    <Calendar class="w-4 h-4 text-on-surface-variant shrink-0 mr-2" />
    <input
      type="date"
      :value="modelValue"
      :placeholder="placeholder"
      :disabled="disabled"
      class="flex-1 bg-transparent outline-none text-sm text-on-surface placeholder:text-on-surface-variant/50 w-full appearance-none"
      @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
    />
  </div>
</template>

<style scoped>
/* Style the native date input to blend with the design system */
input[type="date"]::-webkit-calendar-picker-indicator {
  filter: invert(0.5);
  cursor: pointer;
  opacity: 0.6;
}
input[type="date"]::-webkit-calendar-picker-indicator:hover {
  opacity: 1;
}

.dark input[type="date"]::-webkit-calendar-picker-indicator {
  filter: invert(1) brightness(0.7);
}
</style>
