<script setup lang="ts">
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import { Box, Maximize2, AlignCenter } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Boxed Layout"
      description="Content is constrained to a maximum width for improved readability on large screens."
    />

    <div class="max-w-5xl mx-auto space-y-6">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <WxCard>
          <div class="flex flex-col items-center text-center py-4">
            <div class="w-12 h-12 rounded-xl bg-violet-500/10 text-violet-500 flex items-center justify-center mb-3">
              <Box :size="24" />
            </div>
            <h3 class="text-sm font-semibold text-on-surface mb-1">Constrained Width</h3>
            <p class="text-xs text-on-surface-variant">
              Content is wrapped in a max-w-5xl container, keeping text and elements within a comfortable reading width.
            </p>
          </div>
        </WxCard>

        <WxCard>
          <div class="flex flex-col items-center text-center py-4">
            <div class="w-12 h-12 rounded-xl bg-rose-500/10 text-rose-500 flex items-center justify-center mb-3">
              <AlignCenter :size="24" />
            </div>
            <h3 class="text-sm font-semibold text-on-surface mb-1">Centered Content</h3>
            <p class="text-xs text-on-surface-variant">
              The boxed container is centered horizontally, creating balanced whitespace on both sides.
            </p>
          </div>
        </WxCard>

        <WxCard>
          <div class="flex flex-col items-center text-center py-4">
            <div class="w-12 h-12 rounded-xl bg-teal-500/10 text-teal-500 flex items-center justify-center mb-3">
              <Maximize2 :size="24" />
            </div>
            <h3 class="text-sm font-semibold text-on-surface mb-1">Better Readability</h3>
            <p class="text-xs text-on-surface-variant">
              Shorter line lengths improve reading speed and comprehension, especially for text-heavy pages.
            </p>
          </div>
        </WxCard>
      </div>

      <WxCard>
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">About Boxed Layout</h3>
        </template>
        <div class="text-sm text-on-surface-variant space-y-3">
          <p>
            The boxed layout constrains the main content area to a maximum width of 64rem (1024px). This is
            particularly useful for ultra-wide monitors where full-width layouts can make content difficult to scan.
          </p>
          <p>
            Notice how this page feels more focused compared to a full-width layout. The contained width helps
            guide the eye and keeps related information closer together, reducing cognitive load.
          </p>
          <p>
            This layout works best for content-heavy pages such as documentation, settings panels, and form-based
            workflows where users benefit from a focused reading experience.
          </p>
        </div>
      </WxCard>

      <WxCard>
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">Sample Content</h3>
        </template>
        <div class="grid grid-cols-2 gap-4">
          <div v-for="i in 4" :key="i" class="h-20 rounded-lg bg-surface-container-high flex items-center justify-center">
            <span class="text-sm text-on-surface-variant">Block {{ i }}</span>
          </div>
        </div>
      </WxCard>
    </div>
  </div>
</template>
