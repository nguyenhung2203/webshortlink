<script setup lang="ts">
import { TrendingUp, TrendingDown } from 'lucide-vue-next'
import { computed } from 'vue'

const props = defineProps<{
  title: string
  value: string | number
  trend?: number
  trendUp?: boolean
}>()

const trendColor = computed(() => props.trendUp ? 'text-success' : 'text-danger')
const TrendIcon = computed(() => props.trendUp ? TrendingUp : TrendingDown)
</script>

<template>
  <div class="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm p-5">
    <div class="flex items-start justify-between">
      <div class="flex-1 min-w-0">
        <p class="text-sm text-on-surface-variant font-medium mb-1">{{ title }}</p>
        <p class="text-2xl font-bold text-on-surface tracking-tight">{{ value }}</p>
        <div v-if="trend != null" class="flex items-center gap-1 mt-2" :class="trendColor">
          <component :is="TrendIcon" :size="14" />
          <span class="text-xs font-semibold">{{ Math.abs(trend) }}%</span>
        </div>
      </div>
      <div v-if="$slots.icon" class="shrink-0 ml-4 p-2.5 rounded-lg bg-primary/10 text-primary">
        <slot name="icon" />
      </div>
    </div>
  </div>
</template>
