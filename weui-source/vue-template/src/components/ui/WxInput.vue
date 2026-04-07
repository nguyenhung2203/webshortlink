<script setup lang="ts">
type Size = 'sm' | 'md' | 'lg'

const props = withDefaults(defineProps<{
  modelValue?: string
  type?: 'text' | 'email' | 'search' | 'url'
  placeholder?: string
  disabled?: boolean
  size?: Size
  error?: boolean
  errorMessage?: string
  readonly?: boolean
}>(), {
  modelValue: '',
  type: 'text',
  placeholder: '',
  disabled: false,
  size: 'md',
  error: false,
  errorMessage: '',
  readonly: false,
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
  focus: [event: FocusEvent]
  blur: [event: FocusEvent]
}>()

const sizeClasses: Record<Size, string> = {
  sm: 'h-8 text-xs px-2.5',
  md: 'h-10 text-sm px-3',
  lg: 'h-12 text-lg px-4',
}
</script>

<template>
  <div class="flex flex-col gap-1">
    <div
      class="flex items-center rounded-lg border transition-colors duration-200 bg-surface-container-low"
      :class="[
        sizeClasses[size],
        error ? 'border-rose-500/30 bg-rose-500/10 focus-within:ring-rose-500/20 focus-within:border-rose-500' : 'border-outline-variant focus-within:border-primary focus-within:ring-2 focus-within:ring-primary/20',
        disabled && 'opacity-50 pointer-events-none',
      ]"
    >
      <span v-if="$slots.prefix" class="shrink-0 text-on-surface-variant mr-2">
        <slot name="prefix" />
      </span>
      <input
        :type="type"
        :value="modelValue"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        class="flex-1 bg-transparent outline-none text-on-surface placeholder:text-on-surface-variant placeholder:opacity-40 w-full"
        @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
        @focus="emit('focus', $event)"
        @blur="emit('blur', $event)"
      />
      <span v-if="$slots.suffix" class="shrink-0 text-on-surface-variant ml-2">
        <slot name="suffix" />
      </span>
    </div>
    <p v-if="error && errorMessage" class="text-[10px] text-rose-500 font-medium">{{ errorMessage }}</p>
  </div>
</template>
