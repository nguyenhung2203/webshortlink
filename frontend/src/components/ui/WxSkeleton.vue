<script setup lang="ts">
type Variant = 'text' | 'circular' | 'rectangular'

const props = withDefaults(defineProps<{
  variant?: Variant
  width?: string
  height?: string
  lines?: number
}>(), {
  variant: 'text',
  lines: 1,
})
</script>

<template>
  <div v-if="variant === 'text'" class="flex flex-col gap-2">
    <div
      v-for="i in lines"
      :key="i"
      class="bg-surface-container-high animate-pulse rounded"
      :style="{
        width: i === lines && lines > 1 ? '75%' : (width || '100%'),
        height: height || '0.875rem',
      }"
    />
  </div>

  <div
    v-else-if="variant === 'circular'"
    class="bg-surface-container-high animate-pulse rounded-full"
    :style="{
      width: width || '2.5rem',
      height: height || width || '2.5rem',
    }"
  />

  <div
    v-else
    class="bg-surface-container-high animate-pulse rounded-lg"
    :style="{
      width: width || '100%',
      height: height || '8rem',
    }"
  />
</template>
