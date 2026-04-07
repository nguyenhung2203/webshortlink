<script setup lang="ts">
import { Zap } from 'lucide-vue-next'
</script>

<template>
  <div
    class="flex min-h-screen flex-col items-center justify-center bg-gradient-to-br from-surface to-surface-container-low px-4"
  >
    <!-- Card -->
    <div
      class="w-full max-w-md rounded-2xl border border-outline bg-surface-container p-8 shadow-xl"
    >
      <!-- Logo -->
      <div class="mb-8 flex flex-col items-center gap-3">
        <div
          class="flex h-12 w-12 items-center justify-center rounded-xl bg-primary/10"
        >
          <Zap :size="24" class="text-primary" />
        </div>
        <h1 class="text-xl font-bold tracking-tight text-on-surface">
          WEMAKE
        </h1>
      </div>

      <!-- Router content -->
      <RouterView />
    </div>

    <!-- Footer -->
    <p class="mt-8 text-sm text-on-surface-variant">
      Wemake System
    </p>
  </div>
</template>
