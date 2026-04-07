<script setup lang="ts">
import { X } from 'lucide-vue-next'

defineProps<{
  filters: string[]
}>()

const emit = defineEmits<{
  remove: [index: number]
  clear: []
}>()
</script>

<template>
  <div v-if="filters.length" class="flex items-center gap-2 flex-wrap">
    <span
      v-for="(filter, index) in filters"
      :key="index"
      class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-sm font-medium bg-primary/15 text-primary"
    >
      {{ filter }}
      <button
        class="inline-flex items-center justify-center w-4 h-4 rounded-full hover:bg-primary/20 transition-colors cursor-pointer"
        @click="emit('remove', index)"
      >
        <X :size="12" />
      </button>
    </span>
    <button
      class="text-sm text-on-surface-variant hover:text-on-surface transition-colors cursor-pointer font-medium"
      @click="emit('clear')"
    >
      Clear All
    </button>
  </div>
</template>
