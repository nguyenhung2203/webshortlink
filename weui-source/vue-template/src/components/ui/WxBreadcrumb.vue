<script setup lang="ts">
import { ChevronRight } from 'lucide-vue-next'

interface BreadcrumbItem {
  label: string
  to?: string
}

defineProps<{
  items: BreadcrumbItem[]
}>()
</script>

<template>
  <nav class="flex items-center gap-1.5 text-sm">
    <template v-for="(item, index) in items" :key="index">
      <ChevronRight
        v-if="index > 0"
        :size="14"
        class="text-on-surface-variant/50 shrink-0"
      />
      <router-link
        v-if="item.to && index < items.length - 1"
        :to="item.to"
        class="text-on-surface-variant hover:text-on-surface transition-colors"
      >
        {{ item.label }}
      </router-link>
      <span
        v-else
        class="font-semibold"
        :class="index === items.length - 1 ? 'text-on-surface' : 'text-on-surface-variant'"
      >
        {{ item.label }}
      </span>
    </template>
  </nav>
</template>
