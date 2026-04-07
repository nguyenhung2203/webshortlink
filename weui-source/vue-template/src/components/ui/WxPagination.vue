<script setup lang="ts">
import { computed } from 'vue'
import { ChevronLeft, ChevronRight } from 'lucide-vue-next'

const props = withDefaults(defineProps<{
  currentPage?: number
  totalPages: number
  pageSize?: number
  totalItems?: number
}>(), {
  currentPage: 1,
  pageSize: 10,
})

const emit = defineEmits<{
  'update:currentPage': [page: number]
  'update:pageSize': [size: number]
}>()

const pageSizes = [10, 20, 50, 100]

const visiblePages = computed(() => {
  const pages: (number | '...')[] = []
  const total = props.totalPages
  const current = props.currentPage

  if (total <= 7) {
    for (let i = 1; i <= total; i++) pages.push(i)
  } else {
    pages.push(1)
    if (current > 3) pages.push('...')
    const start = Math.max(2, current - 1)
    const end = Math.min(total - 1, current + 1)
    for (let i = start; i <= end; i++) pages.push(i)
    if (current < total - 2) pages.push('...')
    pages.push(total)
  }

  return pages
})

const itemRange = computed(() => {
  if (!props.totalItems) return ''
  const start = (props.currentPage - 1) * props.pageSize + 1
  const end = Math.min(props.currentPage * props.pageSize, props.totalItems)
  return `${start}-${end} of ${props.totalItems}`
})
</script>

<template>
  <div class="flex items-center justify-between gap-4 flex-wrap">
    <div class="flex items-center gap-2 text-sm text-on-surface-variant">
      <span v-if="itemRange">{{ itemRange }}</span>
      <span v-if="pageSize" class="flex items-center gap-1.5">
        <span class="hidden sm:inline">Rows per page:</span>
        <select
          :value="pageSize"
          class="bg-surface-container rounded px-1.5 py-0.5 text-sm text-on-surface border border-outline-variant cursor-pointer"
          @change="emit('update:pageSize', Number(($event.target as HTMLSelectElement).value))"
        >
          <option v-for="s in pageSizes" :key="s" :value="s">{{ s }}</option>
        </select>
      </span>
    </div>

    <div class="flex items-center gap-1">
      <button
        class="p-1.5 rounded-lg text-on-surface-variant hover:bg-surface-container-high disabled:opacity-30 disabled:pointer-events-none transition-colors cursor-pointer"
        :disabled="currentPage <= 1"
        @click="emit('update:currentPage', currentPage - 1)"
      >
        <ChevronLeft :size="18" />
      </button>

      <template v-for="(page, i) in visiblePages" :key="i">
        <span v-if="page === '...'" class="px-1.5 text-sm text-on-surface-variant">...</span>
        <button
          v-else
          class="min-w-[2rem] h-8 rounded-lg text-sm font-medium transition-colors cursor-pointer"
          :class="page === currentPage
            ? 'bg-primary text-white'
            : 'text-on-surface-variant hover:bg-surface-container-high'"
          @click="emit('update:currentPage', page)"
        >
          {{ page }}
        </button>
      </template>

      <button
        class="p-1.5 rounded-lg text-on-surface-variant hover:bg-surface-container-high disabled:opacity-30 disabled:pointer-events-none transition-colors cursor-pointer"
        :disabled="currentPage >= totalPages"
        @click="emit('update:currentPage', currentPage + 1)"
      >
        <ChevronRight :size="18" />
      </button>
    </div>
  </div>
</template>
