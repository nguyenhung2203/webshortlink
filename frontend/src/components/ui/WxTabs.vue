<script setup lang="ts">
type TabItem = { key: string | number; label: string; disabled?: boolean }

const props = withDefaults(defineProps<{
  modelValue?: string | number
  items?: TabItem[]
}>(), {
  modelValue: '',
  items: () => [],
})

const emit = defineEmits<{
  'update:modelValue': [value: string | number]
}>()

function selectTab(item: TabItem) {
  if (!item.disabled) emit('update:modelValue', item.key)
}
</script>

<template>
  <div>
    <div class="flex border-b border-outline-variant">
      <button
        v-for="item in items"
        :key="item.key"
        type="button"
        class="relative px-4 py-2.5 text-sm font-medium transition-colors duration-200 cursor-pointer"
        :class="[
          item.key === modelValue
            ? 'text-primary'
            : 'text-on-surface-variant hover:text-on-surface',
          item.disabled && 'opacity-40 cursor-not-allowed',
        ]"
        :disabled="item.disabled"
        @click="selectTab(item)"
      >
        {{ item.label }}
        <span
          v-if="item.key === modelValue"
          class="absolute bottom-0 left-0 right-0 h-0.5 bg-primary rounded-t"
        />
      </button>
    </div>
    <div class="pt-4">
      <slot :active-key="modelValue" />
    </div>
  </div>
</template>
