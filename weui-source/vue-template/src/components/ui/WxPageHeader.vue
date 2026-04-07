<script setup lang="ts">
defineProps<{
  title: string
  description?: string
}>()
</script>

<template>
  <div class="space-y-3">
    <div v-if="$slots.breadcrumb">
      <slot name="breadcrumb" />
    </div>
    <div class="flex items-start justify-between gap-4 flex-wrap">
      <div>
        <h1 class="text-2xl font-bold text-on-surface tracking-tight">{{ title }}</h1>
        <p v-if="description" class="mt-1 text-sm text-on-surface-variant">{{ description }}</p>
      </div>
      <div v-if="$slots.actions" class="flex items-center gap-2 shrink-0">
        <slot name="actions" />
      </div>
    </div>
  </div>
</template>
