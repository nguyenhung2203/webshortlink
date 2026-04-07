<script setup lang="ts">
import WxButton from '@/components/ui/WxButton.vue'
import { ArrowLeft, Bug } from 'lucide-vue-next'
import { ref } from 'vue'

const reportSent = ref(false)

function handleReport() {
  reportSent.value = true
  setTimeout(() => { reportSent.value = false }, 3000)
}
</script>

<template>
  <div class="min-h-screen flex flex-col items-center justify-center bg-surface p-4">
    <div class="w-full max-w-lg text-center">
      <!-- Large 500 -->
      <p class="text-[10rem] font-black leading-none tracking-tighter bg-gradient-to-r from-red-400 to-rose-600 bg-clip-text text-transparent select-none">
        500
      </p>

      <h1 class="text-3xl font-bold text-on-surface mt-2">Internal Server Error</h1>
      <p class="text-on-surface-variant mt-3 max-w-md mx-auto">
        Something went wrong on our end. We're working to fix it.
      </p>

      <!-- Actions -->
      <div class="mt-8 flex flex-col sm:flex-row items-center justify-center gap-3">
        <WxButton variant="cta" size="lg" @click="$router.push('/')">
          <template #icon>
            <ArrowLeft :size="18" />
          </template>
          Back to Dashboard
        </WxButton>
        <WxButton
          variant="secondary"
          size="lg"
          :disabled="reportSent"
          @click="handleReport"
        >
          <template #icon>
            <Bug :size="18" />
          </template>
          {{ reportSent ? 'Report Sent' : 'Report Issue' }}
        </WxButton>
      </div>

      <!-- Error details hint -->
      <div class="mt-10 p-4 rounded-lg bg-surface-container-lowest border border-outline-variant max-w-sm mx-auto">
        <p class="text-xs text-on-surface-variant font-mono">
          Error ID: ERR-{{ Math.random().toString(36).substring(2, 10).toUpperCase() }}
        </p>
        <p class="text-xs text-on-surface-variant/60 mt-1">
          If the issue persists, include this ID when contacting support.
        </p>
      </div>
    </div>

    <!-- Branding -->
    <div class="absolute bottom-6 text-center">
      <div class="flex items-center justify-center gap-2 text-on-surface-variant/50">
        <div class="w-6 h-6 rounded-md bg-gradient-to-br from-cyan-400 to-blue-600 flex items-center justify-center text-white text-xs font-bold">
          W
        </div>
        <span class="text-sm font-medium">Wemake</span>
      </div>
    </div>
  </div>
</template>
