<script setup lang="ts">
const props = withDefaults(defineProps<{
  modelValue?: string | number | boolean
  value: string | number | boolean
  disabled?: boolean
  label?: string
  name?: string
}>(), {
  modelValue: '',
  disabled: false,
  label: '',
  name: '',
})

const emit = defineEmits<{
  'update:modelValue': [value: string | number | boolean]
}>()

function select() {
  if (!props.disabled) emit('update:modelValue', props.value)
}
</script>

<template>
  <label
    class="inline-flex items-center gap-2 select-none"
    :class="disabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer'"
    @click="select"
  >
    <span
      class="flex items-center justify-center w-[18px] h-[18px] rounded-full border-2 transition-colors duration-200"
      :class="modelValue === value ? 'border-primary' : 'border-outline-variant'"
    >
      <span
        v-if="modelValue === value"
        class="w-2.5 h-2.5 rounded-full bg-primary"
      />
    </span>
    <input
      type="radio"
      :name="name"
      :value="value"
      :checked="modelValue === value"
      :disabled="disabled"
      class="sr-only"
    />
    <span v-if="label" class="text-sm text-on-surface">{{ label }}</span>
  </label>
</template>
