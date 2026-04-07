<script setup lang="ts">
const props = withDefaults(defineProps<{
  modelValue?: string
  placeholder?: string
  rows?: number
  disabled?: boolean
  error?: boolean
  errorMessage?: string
}>(), {
  modelValue: '',
  placeholder: '',
  rows: 4,
  disabled: false,
  error: false,
  errorMessage: '',
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()
</script>

<template>
  <div class="flex flex-col gap-1">
    <textarea
      :value="modelValue"
      :placeholder="placeholder"
      :rows="rows"
      :disabled="disabled"
      class="w-full rounded-lg border px-3 py-2 text-sm bg-surface-container-lowest text-on-surface placeholder:text-on-surface-variant/50 outline-none transition-colors duration-200 resize-y"
      :class="[
        error ? 'border-danger' : 'border-outline-variant focus:border-primary',
        disabled && 'opacity-50 pointer-events-none',
      ]"
      @input="emit('update:modelValue', ($event.target as HTMLTextAreaElement).value)"
    />
    <p v-if="error && errorMessage" class="text-danger text-xs">{{ errorMessage }}</p>
  </div>
</template>
