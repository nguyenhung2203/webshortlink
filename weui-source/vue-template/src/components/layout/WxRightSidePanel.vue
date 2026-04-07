<script setup lang="ts">
import { X } from 'lucide-vue-next'
import { useUiStore } from '@/stores/ui'
import { useThemeStore } from '@/stores/theme'
import WxThemeToggle from '@/components/layout/WxThemeToggle.vue'

const ui = useUiStore()
const theme = useThemeStore()

const sidebarColors: { key: 'dark' | 'light' | 'primary'; label: string; colorClass: string }[] = [
  { key: 'dark', label: 'Dark', colorClass: 'bg-gray-800' },
  { key: 'light', label: 'Light', colorClass: 'bg-gray-100 border border-outline' },
  { key: 'primary', label: 'Primary', colorClass: 'bg-primary' },
]
</script>

<template>
  <!-- Overlay -->
  <Transition name="fade">
    <div
      v-if="ui.rightPanelOpen"
      class="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm"
      @click="ui.closeRightPanel"
    />
  </Transition>

  <!-- Panel -->
  <Transition name="slide">
    <aside
      v-if="ui.rightPanelOpen"
      class="fixed right-0 top-0 z-50 flex h-full w-72 flex-col border-l border-outline bg-surface-container shadow-2xl"
    >
      <!-- Header -->
      <div class="flex items-center justify-between border-b border-outline px-4 py-3">
        <h2 class="text-sm font-semibold text-on-surface">Settings</h2>
        <button
          class="flex h-8 w-8 items-center justify-center rounded-lg text-on-surface-variant transition-colors hover:bg-surface-container-high hover:text-on-surface"
          @click="ui.closeRightPanel"
        >
          <X :size="18" />
        </button>
      </div>

      <!-- Content -->
      <div class="flex-1 overflow-y-auto p-4">
        <!-- Theme -->
        <div class="mb-6">
          <h3 class="mb-3 text-xs font-semibold uppercase tracking-wider text-on-surface-variant">
            Theme
          </h3>
          <div class="flex items-center justify-between rounded-lg bg-surface-container-high px-3 py-2.5">
            <span class="text-sm text-on-surface">Dark Mode</span>
            <button
              class="relative h-6 w-11 rounded-full transition-colors"
              :class="theme.isDark ? 'bg-primary' : 'bg-on-surface-variant/30'"
              @click="theme.toggle"
            >
              <span
                class="absolute top-0.5 h-5 w-5 rounded-full bg-white shadow transition-transform"
                :class="theme.isDark ? 'translate-x-5' : 'translate-x-0.5'"
              />
            </button>
          </div>
        </div>

        <!-- Sidebar Color -->
        <div class="mb-6">
          <h3 class="mb-3 text-xs font-semibold uppercase tracking-wider text-on-surface-variant">
            Sidebar Color
          </h3>
          <div class="flex gap-3">
            <button
              v-for="color in sidebarColors"
              :key="color.key"
              class="flex h-10 w-10 items-center justify-center rounded-lg transition-transform hover:scale-110"
              :class="[
                color.colorClass,
                ui.sidebarColor === color.key ? 'ring-2 ring-primary ring-offset-2 ring-offset-surface-container' : '',
              ]"
              :title="color.label"
              @click="ui.setSidebarColor(color.key)"
            />
          </div>
        </div>

        <!-- Navbar Fixed -->
        <div class="mb-6">
          <h3 class="mb-3 text-xs font-semibold uppercase tracking-wider text-on-surface-variant">
            Layout
          </h3>
          <div class="flex flex-col gap-2">
            <div class="flex items-center justify-between rounded-lg bg-surface-container-high px-3 py-2.5">
              <span class="text-sm text-on-surface">Fixed Navbar</span>
              <button
                class="relative h-6 w-11 rounded-full transition-colors"
                :class="ui.navbarFixed ? 'bg-primary' : 'bg-on-surface-variant/30'"
                @click="ui.toggleNavbarFixed"
              >
                <span
                  class="absolute top-0.5 h-5 w-5 rounded-full bg-white shadow transition-transform"
                  :class="ui.navbarFixed ? 'translate-x-5' : 'translate-x-0.5'"
                />
              </button>
            </div>
            <div class="flex items-center justify-between rounded-lg bg-surface-container-high px-3 py-2.5">
              <span class="text-sm text-on-surface">Fixed Footer</span>
              <button
                class="relative h-6 w-11 rounded-full transition-colors"
                :class="ui.footerFixed ? 'bg-primary' : 'bg-on-surface-variant/30'"
                @click="ui.toggleFooterFixed"
              >
                <span
                  class="absolute top-0.5 h-5 w-5 rounded-full bg-white shadow transition-transform"
                  :class="ui.footerFixed ? 'translate-x-5' : 'translate-x-0.5'"
                />
              </button>
            </div>
          </div>
        </div>
      </div>
    </aside>
  </Transition>
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

.slide-enter-active,
.slide-leave-active {
  transition: transform 0.3s ease;
}
.slide-enter-from,
.slide-leave-to {
  transform: translateX(100%);
}
</style>
