<script setup lang="ts">
import { ref } from 'vue'

type Size = 'sm' | 'md' | 'lg'

const props = withDefaults(defineProps<{
  modelValue?: string
  placeholder?: string
  disabled?: boolean
  size?: Size
  error?: boolean
  errorMessage?: string
}>(), {
  modelValue: '',
  placeholder: 'Enter password',
  disabled: false,
  size: 'md',
  error: false,
  errorMessage: '',
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
  focus: [event: FocusEvent]
  blur: [event: FocusEvent]
}>()

const visible = ref(false)

const sizeClasses: Record<Size, string> = {
  sm: 'h-8 text-xs px-2.5',
  md: 'h-10 text-sm px-3',
  lg: 'h-12 text-lg px-4',
}
</script>

<template>
  <div class="flex flex-col gap-1">
    <div
      class="flex items-center rounded-lg border transition-colors duration-200 bg-surface-container-lowest"
      :class="[
        sizeClasses[size],
        error ? 'border-danger' : 'border-outline-variant focus-within:border-primary',
        disabled && 'opacity-50 pointer-events-none',
      ]"
    >
      <slot name="prefix" />
      <input
        :type="visible ? 'text' : 'password'"
        :value="modelValue"
        :placeholder="placeholder"
        :disabled="disabled"
        class="flex-1 bg-transparent outline-none text-on-surface placeholder:text-on-surface-variant/50 w-full"
        @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
        @focus="emit('focus', $event)"
        @blur="emit('blur', $event)"
      />
      <button
        type="button"
        tabindex="-1"
        class="shrink-0 text-on-surface-variant hover:text-on-surface ml-2 cursor-pointer"
        @click="visible = !visible"
      >
        <!-- Eye icon (visible) -->
        <svg v-if="!visible" class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M2.062 12.348a1 1 0 0 1 0-.696 10.75 10.75 0 0 1 19.876 0 1 1 0 0 1 0 .696 10.75 10.75 0 0 1-19.876 0" />
          <circle cx="12" cy="12" r="3" />
        </svg>
        <!-- EyeOff icon (hidden) -->
        <svg v-else class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <path d="M10.733 5.076a10.744 10.744 0 0 1 11.205 6.575 1 1 0 0 1 0 .696 10.747 10.747 0 0 1-1.444 2.49" />
          <path d="M14.084 14.158a3 3 0 0 1-4.242-4.242" />
          <path d="M17.479 17.499a10.75 10.75 0 0 1-15.417-5.151 1 1 0 0 1 0-.696 10.75 10.75 0 0 1 4.446-5.143" />
          <path d="m2 2 20 20" />
        </svg>
      </button>
    </div>
    <p v-if="error && errorMessage" class="text-danger text-xs">{{ errorMessage }}</p>
  </div>
</template>
