import { ref } from 'vue'
import { defineStore } from 'pinia'

export interface NavChild {
  key: string
  label: string
  to: string
}

export interface NavItem {
  key: string
  label: string
  icon: string
  to?: string
  children?: NavChild[]
  badge?: number | string
  separator?: boolean
}

export const useSidebarStore = defineStore('sidebar', () => {
  const isOpen = ref(true)
  const isCollapsed = ref(false)
  const activeItem = ref('dashboard')
  const expandedItems = ref<string[]>(['dashboard'])

  function toggle() {
    isOpen.value = !isOpen.value
  }

  function collapse() {
    isCollapsed.value = !isCollapsed.value
  }

  function setActive(key: string) {
    activeItem.value = key
  }

  function toggleExpand(key: string) {
    const index = expandedItems.value.indexOf(key)
    if (index === -1) {
      expandedItems.value.push(key)
    } else {
      expandedItems.value.splice(index, 1)
    }
  }

  function isExpanded(key: string): boolean {
    return expandedItems.value.includes(key)
  }

  function ensureExpanded(key: string) {
    if (!expandedItems.value.includes(key)) {
      expandedItems.value.push(key)
    }
  }

  return {
    isOpen,
    isCollapsed,
    activeItem,
    expandedItems,
    toggle,
    collapse,
    setActive,
    toggleExpand,
    isExpanded,
    ensureExpanded,
  }
})
