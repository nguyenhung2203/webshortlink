<script setup lang="ts">
interface TimelineItem {
  title: string
  description?: string
  time?: string
  color?: string
}

defineProps<{
  items: TimelineItem[]
}>()

function dotColor(color?: string): string {
  const map: Record<string, string> = {
    primary: 'bg-primary',
    success: 'bg-success',
    warning: 'bg-warning',
    danger: 'bg-danger',
    error: 'bg-danger',
    info: 'bg-info',
  }
  return map[color ?? 'primary'] ?? 'bg-primary'
}
</script>

<template>
  <div class="relative pl-6">
    <!-- Vertical line -->
    <div class="absolute left-[0.4375rem] top-2 bottom-2 w-0.5 bg-surface-container-high" />

    <div
      v-for="(item, index) in items"
      :key="index"
      class="relative pb-6 last:pb-0"
    >
      <!-- Dot -->
      <div
        class="absolute -left-6 top-1.5 w-3.5 h-3.5 rounded-full border-2 border-surface-container-lowest"
        :class="dotColor(item.color)"
      />

      <div>
        <div class="flex items-center gap-2 flex-wrap">
          <p class="text-sm font-semibold text-on-surface">{{ item.title }}</p>
          <span v-if="item.time" class="text-xs text-on-surface-variant">{{ item.time }}</span>
        </div>
        <p v-if="item.description" class="text-sm text-on-surface-variant mt-0.5">
          {{ item.description }}
        </p>
      </div>
    </div>
  </div>
</template>
