<script setup lang="ts">
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import { PanelLeft, Pin, ScrollText } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Fixed Sidebar Layout"
      description="The sidebar stays fixed in position while the main content area scrolls independently."
    />

    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <WxCard>
        <div class="flex items-start gap-4">
          <div class="w-10 h-10 rounded-lg bg-primary/10 text-primary flex items-center justify-center shrink-0">
            <Pin :size="20" />
          </div>
          <div>
            <h3 class="text-sm font-semibold text-on-surface mb-1">Fixed Position</h3>
            <p class="text-xs text-on-surface-variant">
              The sidebar uses <code class="text-xs bg-surface-container-high px-1 rounded">position: fixed</code>
              so it remains visible as you scroll through long pages. Navigation is always accessible.
            </p>
          </div>
        </div>
      </WxCard>

      <WxCard>
        <div class="flex items-start gap-4">
          <div class="w-10 h-10 rounded-lg bg-emerald-500/10 text-emerald-500 flex items-center justify-center shrink-0">
            <ScrollText :size="20" />
          </div>
          <div>
            <h3 class="text-sm font-semibold text-on-surface mb-1">Independent Scrolling</h3>
            <p class="text-xs text-on-surface-variant">
              The content area scrolls independently from the sidebar. If the sidebar content overflows,
              it gets its own scrollbar.
            </p>
          </div>
        </div>
      </WxCard>

      <WxCard>
        <div class="flex items-start gap-4">
          <div class="w-10 h-10 rounded-lg bg-amber-500/10 text-amber-500 flex items-center justify-center shrink-0">
            <PanelLeft :size="20" />
          </div>
          <div>
            <h3 class="text-sm font-semibold text-on-surface mb-1">Consistent Access</h3>
            <p class="text-xs text-on-surface-variant">
              Users can navigate to any section without scrolling back to the top. This is especially
              valuable for data-heavy admin panels with deep navigation trees.
            </p>
          </div>
        </div>
      </WxCard>
    </div>

    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">How It Works</h3>
      </template>
      <div class="text-sm text-on-surface-variant space-y-3">
        <p>
          In a fixed sidebar layout, the sidebar element is taken out of the normal document flow using
          CSS fixed positioning. The main content area receives a left margin equal to the sidebar width,
          ensuring no overlap.
        </p>
        <p>
          Scroll down this page to see the effect in action. The sidebar on the left remains stationary
          while this content scrolls. This pattern is the default in most enterprise admin applications.
        </p>
      </div>
    </WxCard>

    <!-- Filler content to demonstrate scrolling -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <WxCard v-for="n in 6" :key="n">
        <div class="h-24 flex items-center justify-center">
          <div class="text-center">
            <p class="text-2xl font-bold text-on-surface">{{ n * 12 }}%</p>
            <p class="text-xs text-on-surface-variant mt-1">Metric {{ n }}</p>
          </div>
        </div>
      </WxCard>
    </div>
  </div>
</template>
