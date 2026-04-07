<script setup lang="ts">
import { ref } from 'vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { Search, ArrowLeft } from 'lucide-vue-next'

const searchQuery = ref('')

function handleSearch() {
  if (searchQuery.value.trim()) {
    // Navigate to search
  }
}
</script>

<template>
  <div class="min-h-screen flex flex-col items-center justify-center bg-surface p-4">
    <div class="w-full max-w-lg text-center">
      <!-- Large 404 -->
      <p class="text-[10rem] font-black leading-none tracking-tighter bg-gradient-to-r from-cyan-400 to-blue-600 bg-clip-text text-transparent select-none">
        404
      </p>

      <h1 class="text-3xl font-bold text-on-surface mt-2">Oops! Page Not Found</h1>
      <p class="text-on-surface-variant mt-3 max-w-md mx-auto">
        The page you are looking for might have been removed or is temporarily unavailable.
      </p>

      <!-- Search -->
      <div class="mt-8 flex gap-2 max-w-sm mx-auto">
        <div class="flex-1">
          <WxInput
            v-model="searchQuery"
            type="search"
            placeholder="Search pages..."
            @keyup.enter="handleSearch"
          >
            <template #prefix>
              <Search :size="16" />
            </template>
          </WxInput>
        </div>
        <WxButton variant="primary" @click="handleSearch">Search</WxButton>
      </div>

      <!-- Back to dashboard -->
      <div class="mt-6">
        <WxButton variant="cta" size="lg" @click="$router.push('/')">
          <template #icon>
            <ArrowLeft :size="18" />
          </template>
          Back to Dashboard
        </WxButton>
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
