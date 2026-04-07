<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { Menu, ChevronRight, Settings as SettingsIcon } from 'lucide-vue-next'
import { useSidebarStore } from '@/stores/sidebar'
import { useUiStore } from '@/stores/ui'
import WxSideNav from '@/components/layout/WxSideNav.vue'
import WxThemeToggle from '@/components/layout/WxThemeToggle.vue'
import WxNotificationDropdown from '@/components/layout/WxNotificationDropdown.vue'
import WxMessageDropdown from '@/components/layout/WxMessageDropdown.vue'
import WxUserMenu from '@/components/layout/WxUserMenu.vue'
import WxFooter from '@/components/layout/WxFooter.vue'
import WxRightSidePanel from '@/components/layout/WxRightSidePanel.vue'

const sidebar = useSidebarStore()
const ui = useUiStore()
const route = useRoute()
const isMobile = ref(false)

function checkMobile() {
  isMobile.value = window.innerWidth < 768
  if (isMobile.value) {
    sidebar.isOpen = false
  }
}

onMounted(() => {
  checkMobile()
  window.addEventListener('resize', checkMobile)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
})

const sidebarWidth = computed(() => {
  if (isMobile.value) return '256px'
  return sidebar.isCollapsed ? '64px' : '256px'
})

const mainOffset = computed(() => {
  if (isMobile.value) return '0px'
  return sidebarWidth.value
})

const breadcrumbTitle = computed(() => {
  return (route.meta.title as string) || 'Dashboard'
})

const breadcrumbGroup = computed(() => {
  return (route.meta.group as string) || null
})

function handleToggle() {
  if (isMobile.value) {
    sidebar.toggle()
  } else {
    sidebar.collapse()
  }
}

function handleOverlayClick() {
  if (isMobile.value) {
    sidebar.isOpen = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-surface text-on-surface">
    <!-- Mobile overlay -->
    <Transition name="fade">
      <div
        v-if="isMobile && sidebar.isOpen"
        class="fixed inset-0 z-40 bg-black/50 backdrop-blur-sm"
        @click="handleOverlayClick"
      />
    </Transition>

    <!-- Sidebar -->
    <aside
      :class="[
        'fixed top-0 left-0 z-50 h-screen border-r border-outline transition-all duration-300 ease-in-out',
        isMobile
          ? sidebar.isOpen
            ? 'translate-x-0'
            : '-translate-x-full'
          : 'translate-x-0',
      ]"
      :style="{ width: sidebarWidth }"
    >
      <WxSideNav :collapsed="!isMobile && sidebar.isCollapsed" />
    </aside>

    <!-- Main wrapper -->
    <div
      class="flex min-h-screen flex-col transition-all duration-300 ease-in-out"
      :style="{ marginLeft: mainOffset }"
    >
      <!-- Top bar -->
      <header
        class="header-gradient sticky top-0 z-40 flex h-14 items-center justify-between px-4 shadow-md"
      >
        <div class="flex items-center gap-3">
          <button
            class="flex h-9 w-9 items-center justify-center rounded-lg text-white/90 transition-colors hover:bg-white/15 hover:text-white"
            @click="handleToggle"
          >
            <Menu :size="20" />
          </button>

          <!-- Breadcrumb -->
          <nav class="flex items-center gap-1.5 text-sm text-white/70">
            <span>Home</span>
            <ChevronRight :size="14" class="text-white/40" />
            <template v-if="breadcrumbGroup">
              <span>{{ breadcrumbGroup }}</span>
              <ChevronRight :size="14" class="text-white/40" />
            </template>
            <span class="font-medium text-white">{{ breadcrumbTitle }}</span>
          </nav>
        </div>

        <div class="flex items-center gap-2">
          <WxThemeToggle />
          <WxMessageDropdown />
          <WxNotificationDropdown />

          <!-- Settings gear button -->
          <button
            class="flex h-9 w-9 items-center justify-center rounded-lg text-white/90 transition-colors hover:bg-white/15 hover:text-white"
            title="Settings"
            @click="ui.openRightPanel"
          >
            <SettingsIcon :size="18" />
          </button>

          <WxUserMenu />
        </div>
      </header>

      <!-- Main content -->
      <main class="flex-1 overflow-y-auto p-6">
        <RouterView />
      </main>

      <!-- Footer -->
      <WxFooter />
    </div>

    <!-- Right side panel -->
    <WxRightSidePanel />
  </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
