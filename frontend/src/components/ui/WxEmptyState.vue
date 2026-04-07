<script setup lang="ts">
import { InboxIcon } from 'lucide-vue-next'

defineProps<{
  title: string
  description?: string
}>()
</script>

<template>
  <div class="flex flex-col items-center justify-center py-12 px-6 text-center">
    <div class="mb-4 text-on-surface-variant/40">
      <slot name="icon">
        <InboxIcon :size="48" :stroke-width="1.5" />
      </slot>
    </div>
    <h3 class="text-lg font-semibold text-on-surface mb-1">{{ title }}</h3>
    <p v-if="description" class="text-sm text-on-surface-variant max-w-sm">{{ description }}</p>
    <div v-if="$slots.action" class="mt-5">
      <slot name="action" />
    </div>
  </div>
</template>
