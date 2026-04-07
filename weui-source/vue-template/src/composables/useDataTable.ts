import { ref, computed, type Ref } from 'vue'
import type { TableColumn } from '@/types'

interface SortConfig {
  key: string
  direction: 'asc' | 'desc'
}

export function useDataTable<T extends Record<string, unknown>>(
  data: Ref<T[]>,
  _columns: Ref<TableColumn[]>,
) {
  const sortConfig = ref<SortConfig>({ key: '', direction: 'asc' })
  const currentFilter = ref<{ key: string; value: unknown }>({ key: '', value: '' })

  const filteredData = computed(() => {
    if (!currentFilter.value.key || currentFilter.value.value === '') {
      return data.value
    }
    return data.value.filter(
      (row) => String(row[currentFilter.value.key]) === String(currentFilter.value.value),
    )
  })

  const sortedData = computed(() => {
    const { key, direction } = sortConfig.value
    if (!key) return filteredData.value

    return [...filteredData.value].sort((a, b) => {
      const aVal = a[key]
      const bVal = b[key]
      if (aVal == null || bVal == null) return 0

      const comparison = String(aVal).localeCompare(String(bVal), undefined, { numeric: true })
      return direction === 'asc' ? comparison : -comparison
    })
  })

  function sortBy(key: string) {
    if (sortConfig.value.key === key) {
      sortConfig.value.direction = sortConfig.value.direction === 'asc' ? 'desc' : 'asc'
    } else {
      sortConfig.value = { key, direction: 'asc' }
    }
  }

  function filterBy(key: string, value: unknown) {
    currentFilter.value = { key, value }
  }

  return { sortedData, sortBy, sortConfig, filterBy, currentFilter }
}
