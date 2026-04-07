<script setup lang="ts">
import { computed } from 'vue'
import { ArrowUpDown, ArrowUp, ArrowDown } from 'lucide-vue-next'
import type { TableColumn } from '@/types'

const props = withDefaults(defineProps<{
  columns: TableColumn[]
  data: Record<string, unknown>[]
  loading?: boolean
  emptyText?: string
}>(), {
  loading: false,
  emptyText: 'No data available',
})

const emit = defineEmits<{
  sort: [key: string, direction: 'asc' | 'desc']
}>()

const sortKey = defineModel<string>('sortKey', { default: '' })
const sortDirection = defineModel<'asc' | 'desc'>('sortDirection', { default: 'asc' })

function handleSort(column: TableColumn) {
  if (!column.sortable) return

  if (sortKey.value === column.key) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortKey.value = column.key
    sortDirection.value = 'asc'
  }

  emit('sort', sortKey.value, sortDirection.value)
}

const skeletonRows = computed(() => Array.from({ length: 5 }))
</script>

<template>
  <div class="bg-surface-container-lowest rounded-xl border border-outline-variant overflow-hidden">
    <div class="overflow-x-auto">
      <table class="w-full">
        <thead>
          <tr class="bg-surface-container">
            <th
              v-for="col in columns"
              :key="col.key"
              :style="col.width ? { width: col.width } : undefined"
              class="px-4 py-3 text-left text-[10px] font-bold uppercase tracking-wider text-on-surface-variant select-none"
              :class="col.sortable && 'cursor-pointer hover:text-on-surface transition-colors'"
              @click="handleSort(col)"
            >
              <span class="inline-flex items-center gap-1.5">
                {{ col.label }}
                <template v-if="col.sortable">
                  <ArrowUp v-if="sortKey === col.key && sortDirection === 'asc'" class="w-3.5 h-3.5 text-primary" />
                  <ArrowDown v-else-if="sortKey === col.key && sortDirection === 'desc'" class="w-3.5 h-3.5 text-primary" />
                  <ArrowUpDown v-else class="w-3.5 h-3.5 opacity-40" />
                </template>
              </span>
            </th>
          </tr>
        </thead>

        <!-- Loading state -->
        <tbody v-if="loading">
          <tr
            v-for="(_, rowIdx) in skeletonRows"
            :key="rowIdx"
            class="border-t border-outline-variant"
            :class="rowIdx % 2 === 1 && 'bg-surface-container-low/30'"
          >
            <td v-for="col in columns" :key="col.key" class="px-4 py-3">
              <div class="h-4 rounded bg-surface-container-high/60 animate-pulse" :style="{ width: `${60 + Math.random() * 30}%` }" />
            </td>
          </tr>
        </tbody>

        <!-- Data rows -->
        <tbody v-else-if="data.length > 0">
          <tr
            v-for="(row, rowIdx) in data"
            :key="rowIdx"
            class="border-t border-outline-variant transition-colors hover:bg-surface-container-low/50"
            :class="rowIdx % 2 === 1 && 'bg-surface-container-low/30'"
          >
            <td
              v-for="col in columns"
              :key="col.key"
              class="px-4 py-3 text-sm text-on-surface"
            >
              <slot :name="`cell-${col.key}`" :row="row" :value="row[col.key]">
                {{ row[col.key] }}
              </slot>
            </td>
          </tr>
        </tbody>

        <!-- Empty state -->
        <tbody v-else>
          <tr>
            <td :colspan="columns.length" class="px-4 py-12">
              <slot name="empty">
                <div class="flex flex-col items-center justify-center text-on-surface-variant">
                  <svg class="w-12 h-12 mb-3 opacity-30" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                    <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" stroke-linecap="round" stroke-linejoin="round" />
                  </svg>
                  <p class="text-sm">{{ emptyText }}</p>
                </div>
              </slot>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
