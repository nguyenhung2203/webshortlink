<script setup lang="ts">
import { Zap } from 'lucide-vue-next'
import WxTopNav from '@/components/layout/WxTopNav.vue'
import WxThemeToggle from '@/components/layout/WxThemeToggle.vue'
import WxUserMenu from '@/components/layout/WxUserMenu.vue'
import WxFooter from '@/components/layout/WxFooter.vue'
</script>

<template>
  <div class="flex min-h-screen flex-col bg-surface text-on-surface">
    <!-- Header -->
    <header class="header-gradient sticky top-0 z-30 shadow-md">
      <!-- Top bar -->
      <div class="mx-auto flex h-14 max-w-7xl items-center justify-between px-4">
        <!-- Logo -->
        <div class="flex items-center gap-3">
          <div class="flex h-8 w-8 items-center justify-center rounded-lg bg-white/15">
            <Zap :size="18" class="text-white" />
          </div>
          <span class="text-base font-bold tracking-wider text-white">WEMAKE</span>
        </div>

        <!-- Right side -->
        <div class="flex items-center gap-2">
          <WxThemeToggle />
          <WxUserMenu />
        </div>
      </div>

      <!-- Navigation bar -->
      <div class="border-t border-white/10">
        <div class="mx-auto max-w-7xl overflow-x-auto px-4 py-1">
          <WxTopNav />
        </div>
      </div>
    </header>

    <!-- Main content -->
    <main class="mx-auto w-full max-w-7xl flex-1 p-6">
      <RouterView />
    </main>

    <!-- Footer -->
    <WxFooter />
  </div>
</template>
