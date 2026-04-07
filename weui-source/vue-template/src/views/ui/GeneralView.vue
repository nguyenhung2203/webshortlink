<script setup lang="ts">
import { ref } from 'vue'
import {
  Users, DollarSign, ShoppingCart, TrendingUp,
  Info, AlertTriangle, AlertCircle, Shield,
} from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxAlert from '@/components/ui/WxAlert.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxTag from '@/components/ui/WxTag.vue'
import WxStatCard from '@/components/ui/WxStatCard.vue'

const showDismissible = ref(true)
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="General UI Elements"
      description="Alerts, badges, tags, cards, callouts, and ribbons for building rich interfaces."
    />

    <!-- Alerts -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Alerts</h3>
      </template>
      <div class="space-y-3">
        <WxAlert type="info" title="Information">
          A new software update is available. See what's new in version 4.2.
        </WxAlert>
        <WxAlert type="success" title="Success">
          Your profile has been updated successfully. Changes are now live.
        </WxAlert>
        <WxAlert type="warning" title="Warning">
          Your subscription will expire in 3 days. Renew now to avoid interruption.
        </WxAlert>
        <WxAlert type="error" title="Error">
          Payment processing failed. Please check your billing details and try again.
        </WxAlert>
        <WxAlert v-if="showDismissible" type="info" title="Dismissible Alert" closable @close="showDismissible = false">
          Click the X button to dismiss this alert. It can be restored by refreshing the page.
        </WxAlert>
      </div>
    </WxCard>

    <!-- Badges & Tags -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Badges &amp; Tags</h3>
      </template>
      <div class="space-y-5">
        <div>
          <p class="text-xs font-medium text-on-surface-variant mb-3">Badge Variants</p>
          <div class="flex flex-wrap items-center gap-3">
            <WxBadge variant="info">4</WxBadge>
            <WxBadge variant="success">12</WxBadge>
            <WxBadge variant="warning">3</WxBadge>
            <WxBadge variant="error">99</WxBadge>
            <WxBadge variant="neutral">0</WxBadge>
            <WxBadge variant="error" :max="99">150</WxBadge>
            <WxBadge variant="info" dot />
            <WxBadge variant="success" dot />
            <WxBadge variant="error" dot />
          </div>
        </div>
        <div>
          <p class="text-xs font-medium text-on-surface-variant mb-3">Badge Sizes</p>
          <div class="flex flex-wrap items-center gap-3">
            <WxBadge variant="info" size="sm">SM</WxBadge>
            <WxBadge variant="info" size="md">MD</WxBadge>
            <WxBadge variant="info" size="lg">LG</WxBadge>
          </div>
        </div>
        <div>
          <p class="text-xs font-medium text-on-surface-variant mb-3">Tag Variants</p>
          <div class="flex flex-wrap items-center gap-2">
            <WxTag variant="default">Default</WxTag>
            <WxTag variant="primary">Primary</WxTag>
            <WxTag variant="success">Success</WxTag>
            <WxTag variant="warning">Warning</WxTag>
            <WxTag variant="error">Error</WxTag>
            <WxTag variant="primary" removable @remove="() => {}">Removable</WxTag>
            <WxTag variant="success" size="sm">Small</WxTag>
          </div>
        </div>
      </div>
    </WxCard>

    <!-- Cards -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Cards</h3>
      </template>

      <p class="text-xs font-medium text-on-surface-variant mb-3">Card with Header, Body &amp; Footer</p>
      <WxCard>
        <template #header>
          <h4 class="text-sm font-semibold text-on-surface">Project Overview</h4>
        </template>
        <p class="text-sm text-on-surface-variant">
          This card demonstrates the standard header, body, and footer layout commonly used for content sections in dashboards and detail views.
        </p>
        <template #footer>
          <p class="text-xs text-on-surface-variant">Last updated 2 hours ago</p>
        </template>
      </WxCard>

      <p class="text-xs font-medium text-on-surface-variant mt-5 mb-3">Colored Top-Border Cards</p>
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div class="rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm border-t-4 border-t-primary p-4">
          <h4 class="text-sm font-semibold text-on-surface mb-1">Primary</h4>
          <p class="text-xs text-on-surface-variant">Feature highlight card with primary accent border.</p>
        </div>
        <div class="rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm border-t-4 border-t-success p-4">
          <h4 class="text-sm font-semibold text-on-surface mb-1">Success</h4>
          <p class="text-xs text-on-surface-variant">Status card indicating successful completion.</p>
        </div>
        <div class="rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm border-t-4 border-t-warning p-4">
          <h4 class="text-sm font-semibold text-on-surface mb-1">Warning</h4>
          <p class="text-xs text-on-surface-variant">Attention needed card with warning indicator.</p>
        </div>
        <div class="rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm border-t-4 border-t-danger p-4">
          <h4 class="text-sm font-semibold text-on-surface mb-1">Danger</h4>
          <p class="text-xs text-on-surface-variant">Critical issue card requiring immediate action.</p>
        </div>
      </div>

      <p class="text-xs font-medium text-on-surface-variant mt-5 mb-3">Stat Cards</p>
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <WxStatCard title="Total Users" value="24,891" :trend="12.5" :trend-up="true">
          <template #icon><Users :size="20" /></template>
        </WxStatCard>
        <WxStatCard title="Revenue" value="$48,290" :trend="8.2" :trend-up="true">
          <template #icon><DollarSign :size="20" /></template>
        </WxStatCard>
        <WxStatCard title="Orders" value="1,423" :trend="3.1" :trend-up="false">
          <template #icon><ShoppingCart :size="20" /></template>
        </WxStatCard>
        <WxStatCard title="Conversion" value="3.24%" :trend="0.8" :trend-up="true">
          <template #icon><TrendingUp :size="20" /></template>
        </WxStatCard>
      </div>
    </WxCard>

    <!-- Callouts -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Callouts</h3>
      </template>
      <div class="space-y-4">
        <div class="flex gap-3 rounded-lg border-l-4 border-l-info bg-info/5 p-4">
          <Info :size="20" class="shrink-0 mt-0.5 text-info" />
          <div>
            <p class="text-sm font-semibold text-on-surface mb-1">Did you know?</p>
            <p class="text-sm text-on-surface-variant">You can customize your dashboard layout by dragging and dropping widgets. Access this feature from Settings &gt; Dashboard.</p>
          </div>
        </div>
        <div class="flex gap-3 rounded-lg border-l-4 border-l-warning bg-warning/5 p-4">
          <AlertTriangle :size="20" class="shrink-0 mt-0.5 text-warning" />
          <div>
            <p class="text-sm font-semibold text-on-surface mb-1">Maintenance Window</p>
            <p class="text-sm text-on-surface-variant">Scheduled maintenance will occur on Saturday, April 5th from 2:00 AM to 6:00 AM UTC. Expect brief service interruptions.</p>
          </div>
        </div>
        <div class="flex gap-3 rounded-lg border-l-4 border-l-danger bg-danger/5 p-4">
          <AlertCircle :size="20" class="shrink-0 mt-0.5 text-danger" />
          <div>
            <p class="text-sm font-semibold text-on-surface mb-1">Breaking Change</p>
            <p class="text-sm text-on-surface-variant">API v1 will be deprecated on June 1st, 2026. Please migrate to API v2 before the deadline to avoid service disruption.</p>
          </div>
        </div>
      </div>
    </WxCard>

    <!-- Ribbons -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Ribbons</h3>
      </template>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="relative overflow-hidden rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm p-5 min-h-[140px]">
          <div class="ribbon ribbon-primary">NEW</div>
          <h4 class="text-sm font-semibold text-on-surface mb-1 mt-2">Feature Release</h4>
          <p class="text-xs text-on-surface-variant">A brand new analytics dashboard is now available for all Pro plan users.</p>
        </div>
        <div class="relative overflow-hidden rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm p-5 min-h-[140px]">
          <div class="ribbon ribbon-success">SALE</div>
          <h4 class="text-sm font-semibold text-on-surface mb-1 mt-2">Annual Plan</h4>
          <p class="text-xs text-on-surface-variant">Save 40% when you upgrade to the annual billing plan. Limited time offer.</p>
        </div>
        <div class="relative overflow-hidden rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm p-5 min-h-[140px]">
          <div class="ribbon ribbon-danger">HOT</div>
          <h4 class="text-sm font-semibold text-on-surface mb-1 mt-2">Popular Choice</h4>
          <p class="text-xs text-on-surface-variant">This plan is the most popular among enterprise customers worldwide.</p>
        </div>
      </div>
    </WxCard>
  </div>
</template>

<style scoped>
.ribbon {
  position: absolute;
  top: 12px;
  right: -32px;
  width: 120px;
  padding: 4px 0;
  text-align: center;
  font-size: 10px;
  font-weight: 700;
  letter-spacing: 1px;
  color: white;
  transform: rotate(45deg);
  z-index: 1;
}
.ribbon-primary { background: linear-gradient(135deg, #22d3ee, #3b82f6); }
.ribbon-success { background: linear-gradient(135deg, #22c55e, #16a34a); }
.ribbon-danger { background: linear-gradient(135deg, #f43f5e, #dc2626); }
</style>
