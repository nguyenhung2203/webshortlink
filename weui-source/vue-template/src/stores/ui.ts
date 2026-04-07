import { ref } from 'vue'
import { defineStore } from 'pinia'

export const useUiStore = defineStore('ui', () => {
  const modalOpen = ref(false)
  const drawerOpen = ref(false)
  const rightPanelOpen = ref(false)
  const sidebarColor = ref<'dark' | 'light' | 'primary'>('dark')
  const navbarFixed = ref(true)
  const footerFixed = ref(false)

  function openModal() {
    modalOpen.value = true
  }

  function closeModal() {
    modalOpen.value = false
  }

  function openDrawer() {
    drawerOpen.value = true
  }

  function closeDrawer() {
    drawerOpen.value = false
  }

  function toggleRightPanel() {
    rightPanelOpen.value = !rightPanelOpen.value
  }

  function openRightPanel() {
    rightPanelOpen.value = true
  }

  function closeRightPanel() {
    rightPanelOpen.value = false
  }

  function setSidebarColor(color: 'dark' | 'light' | 'primary') {
    sidebarColor.value = color
  }

  function toggleNavbarFixed() {
    navbarFixed.value = !navbarFixed.value
  }

  function toggleFooterFixed() {
    footerFixed.value = !footerFixed.value
  }

  return {
    modalOpen,
    drawerOpen,
    rightPanelOpen,
    sidebarColor,
    navbarFixed,
    footerFixed,
    openModal,
    closeModal,
    openDrawer,
    closeDrawer,
    toggleRightPanel,
    openRightPanel,
    closeRightPanel,
    setSidebarColor,
    toggleNavbarFixed,
    toggleFooterFixed,
  }
})
