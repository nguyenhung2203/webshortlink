<script setup lang="ts">
import { ref } from 'vue'
import { User, Settings, LogOut } from 'lucide-vue-next'

const isOpen = ref(false)

function toggle() {
  isOpen.value = !isOpen.value
}

function close() {
  isOpen.value = false
}
</script>

<template>
  <div class="relative">
    <button
      class="ml-1 flex items-center gap-2 rounded-lg px-2 py-1 transition-colors hover:bg-white/15"
      @click="toggle"
    >
      <div
        class="flex h-8 w-8 items-center justify-center rounded-full bg-white/20 text-sm font-semibold text-white"
      >
        W
      </div>
      <span class="hidden text-sm font-medium text-white sm:block">Admin</span>
    </button>

    <!-- Dropdown overlay -->
    <div v-if="isOpen" class="fixed inset-0 z-30" @click="close" />

    <!-- Dropdown -->
    <Transition name="dropdown">
      <div
        v-if="isOpen"
        class="absolute right-0 top-full z-50 mt-2 w-48 overflow-hidden rounded-xl border border-outline bg-surface-container shadow-xl"
      >
        <!-- User info -->
        <div class="border-b border-outline px-4 py-3">
          <p class="text-sm font-semibold text-on-surface">Admin User</p>
          <p class="text-xs text-on-surface-variant">admin@wemake.com</p>
        </div>

        <!-- Menu items -->
        <div class="py-1">
          <RouterLink
            to="/pages/profile"
            class="flex items-center gap-3 px-4 py-2 text-sm text-on-surface-variant transition-colors hover:bg-surface-container-high hover:text-on-surface"
            @click="close"
          >
            <User :size="16" />
            Profile
          </RouterLink>
          <RouterLink
            to="/pages/faq"
            class="flex items-center gap-3 px-4 py-2 text-sm text-on-surface-variant transition-colors hover:bg-surface-container-high hover:text-on-surface"
            @click="close"
          >
            <Settings :size="16" />
            Settings
          </RouterLink>
        </div>

        <!-- Divider -->
        <div class="border-t border-outline" />

        <!-- Logout -->
        <div class="py-1">
          <RouterLink
            to="/auth/login"
            class="flex items-center gap-3 px-4 py-2 text-sm text-danger transition-colors hover:bg-danger/10"
            @click="close"
          >
            <LogOut :size="16" />
            Logout
          </RouterLink>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.dropdown-enter-active,
.dropdown-leave-active {
  transition: opacity 0.15s ease, transform 0.15s ease;
}
.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
