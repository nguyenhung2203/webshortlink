<script setup lang="ts">
import { computed } from 'vue'

type Size = 'xs' | 'sm' | 'md' | 'lg' | 'xl'
type Status = 'online' | 'offline' | 'away' | null

const props = withDefaults(defineProps<{
  src?: string
  name: string
  size?: Size
  status?: Status
}>(), {
  size: 'md',
  status: null,
})

const sizeClasses: Record<Size, string> = {
  xs: 'w-6 h-6 text-[10px]',
  sm: 'w-8 h-8 text-xs',
  md: 'w-10 h-10 text-xs',
  lg: 'w-14 h-14 text-sm',
  xl: 'w-20 h-20 text-xl',
}

const statusDotSize: Record<Size, string> = {
  xs: 'w-3 h-3 border-2 border-surface-container-lowest',
  sm: 'w-3 h-3 border-2 border-surface-container-lowest',
  md: 'w-3 h-3 border-2 border-surface-container-lowest',
  lg: 'w-3 h-3 border-2 border-surface-container-lowest',
  xl: 'w-3 h-3 border-2 border-surface-container-lowest',
}

const statusColors: Record<string, string> = {
  online: 'bg-teal-400',
  offline: 'bg-rose-400',
  away: 'bg-sky-400',
}

const initials = computed(() => {
  return props.name
    .split(' ')
    .map(w => w[0])
    .slice(0, 2)
    .join('')
    .toUpperCase()
})

const bgColor = computed(() => {
  const colors = [
    'bg-cyan-500', 'bg-blue-500', 'bg-violet-500', 'bg-rose-500',
    'bg-amber-500', 'bg-emerald-500', 'bg-indigo-500', 'bg-pink-500',
  ]
  let hash = 0
  for (const char of props.name) hash = char.charCodeAt(0) + ((hash << 5) - hash)
  return colors[Math.abs(hash) % colors.length]
})
</script>

<template>
  <div class="relative inline-flex shrink-0">
    <img
      v-if="src"
      :src="src"
      :alt="name"
      class="rounded-full object-cover"
      :class="sizeClasses[size]"
    />
    <div
      v-else
      class="rounded-full flex items-center justify-center font-bold text-white"
      :class="[sizeClasses[size], bgColor]"
    >
      {{ initials }}
    </div>
    <span
      v-if="status"
      class="absolute bottom-0 right-0 rounded-full border-surface-container-lowest"
      :class="[statusDotSize[size], statusColors[status]]"
    />
  </div>
</template>
