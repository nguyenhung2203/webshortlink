<script setup lang="ts">
type Padding = 'none' | 'sm' | 'md' | 'lg'

withDefaults(defineProps<{
  padding?: Padding
}>(), {
  padding: 'md',
})

const paddingClasses: Record<Padding, string> = {
  none: '',
  sm: 'p-3',
  md: 'p-6',
  lg: 'p-8',
}
</script>

<template>
  <div class="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm">
    <div
      v-if="$slots.header"
      class="border-b border-outline-variant px-5 py-3"
    >
      <slot name="header" />
    </div>

    <div :class="paddingClasses[padding]">
      <slot />
    </div>

    <div
      v-if="$slots.footer"
      class="border-t border-outline-variant px-5 py-3"
    >
      <slot name="footer" />
    </div>
  </div>
</template>
