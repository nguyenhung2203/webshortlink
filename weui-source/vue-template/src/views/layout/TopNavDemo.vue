<script setup lang="ts">
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import { Navigation, Layout, Monitor } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Top Navigation Layout"
      description="This page demonstrates the Top Navigation layout. Navigate using the top menu bar."
    />

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <WxCard>
        <div class="flex flex-col items-center text-center py-4">
          <div class="w-12 h-12 rounded-xl bg-primary/10 text-primary flex items-center justify-center mb-3">
            <Navigation :size="24" />
          </div>
          <h3 class="text-sm font-semibold text-on-surface mb-1">Horizontal Navigation</h3>
          <p class="text-xs text-on-surface-variant">
            The top navigation bar provides horizontal menu items for quick access to all sections of the application.
          </p>
        </div>
      </WxCard>

      <WxCard>
        <div class="flex flex-col items-center text-center py-4">
          <div class="w-12 h-12 rounded-xl bg-emerald-500/10 text-emerald-500 flex items-center justify-center mb-3">
            <Layout :size="24" />
          </div>
          <h3 class="text-sm font-semibold text-on-surface mb-1">Full-Width Content</h3>
          <p class="text-xs text-on-surface-variant">
            Without a sidebar, the content area spans the full width of the viewport for maximum screen utilization.
          </p>
        </div>
      </WxCard>

      <WxCard>
        <div class="flex flex-col items-center text-center py-4">
          <div class="w-12 h-12 rounded-xl bg-amber-500/10 text-amber-500 flex items-center justify-center mb-3">
            <Monitor :size="24" />
          </div>
          <h3 class="text-sm font-semibold text-on-surface mb-1">Responsive Design</h3>
          <p class="text-xs text-on-surface-variant">
            On smaller screens the navigation collapses into a hamburger menu for an optimal mobile experience.
          </p>
        </div>
      </WxCard>
    </div>

    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Layout Details</h3>
      </template>
      <div class="prose prose-sm max-w-none text-on-surface-variant space-y-3">
        <p>
          The top navigation layout removes the sidebar and replaces it with a horizontal navigation bar at the top
          of the page. This layout is well-suited for applications with fewer navigation items or those that prioritize
          content width.
        </p>
        <p>
          Use this layout when your application has 5-8 primary navigation items that fit comfortably in a single
          horizontal row. For larger navigation structures, consider the sidebar layout instead.
        </p>
      </div>
    </WxCard>
  </div>
</template>
