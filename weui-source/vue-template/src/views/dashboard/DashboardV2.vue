<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxStatCard from '@/components/ui/WxStatCard.vue'
import WxCard from '@/components/ui/WxCard.vue'
import { Users, Eye, ArrowDownRight, Clock } from 'lucide-vue-next'

const trafficSources = ref([
  { label: 'Direct', value: 65, color: 'from-blue-500 to-blue-600' },
  { label: 'Organic', value: 45, color: 'from-emerald-500 to-emerald-600' },
  { label: 'Referral', value: 30, color: 'from-amber-500 to-amber-600' },
  { label: 'Social', value: 20, color: 'from-purple-500 to-purple-600' },
])

const topPages = ref([
  { url: '/home', views: '12,340', bounce: '21.3%', duration: '4m 12s' },
  { url: '/products', views: '8,720', bounce: '28.7%', duration: '3m 45s' },
  { url: '/blog/getting-started', views: '6,150', bounce: '18.2%', duration: '5m 08s' },
  { url: '/pricing', views: '4,890', bounce: '32.1%', duration: '2m 56s' },
  { url: '/about', views: '3,210', bounce: '35.8%', duration: '1m 34s' },
])

const referralSources = ref([
  { domain: 'google.com', visits: 12480, pct: 85 },
  { domain: 'facebook.com', visits: 8320, pct: 57 },
  { domain: 'twitter.com', visits: 5640, pct: 38 },
  { domain: 'linkedin.com', visits: 3920, pct: 27 },
  { domain: 'reddit.com', visits: 2150, pct: 15 },
])
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Analytics Dashboard" description="Website traffic analytics and user behavior insights." />

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <WxStatCard title="Total Users" value="2,340" :trend="14.2" :trend-up="true">
        <template #icon><Users :size="20" /></template>
      </WxStatCard>
      <WxStatCard title="Page Views" value="45.2K" :trend="8.7" :trend-up="true">
        <template #icon><Eye :size="20" /></template>
      </WxStatCard>
      <WxStatCard title="Bounce Rate" value="24.5%" :trend="2.1" :trend-up="false">
        <template #icon><ArrowDownRight :size="20" /></template>
      </WxStatCard>
      <WxStatCard title="Session Duration" value="3m 42s" :trend="5.3" :trend-up="true">
        <template #icon><Clock :size="20" /></template>
      </WxStatCard>
    </div>

    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Traffic Sources</h3>
      </template>
      <div class="space-y-4">
        <div v-for="source in trafficSources" :key="source.label" class="space-y-1.5">
          <div class="flex items-center justify-between text-sm">
            <span class="font-medium text-on-surface">{{ source.label }}</span>
            <span class="text-on-surface-variant">{{ source.value }}%</span>
          </div>
          <div class="h-2.5 rounded-full bg-surface-container-high overflow-hidden">
            <div
              class="h-full rounded-full bg-gradient-to-r transition-all duration-700"
              :class="source.color"
              :style="{ width: `${source.value}%` }"
            />
          </div>
        </div>
      </div>
    </WxCard>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="lg:col-span-2">
        <WxCard>
          <template #header>
            <h3 class="text-sm font-semibold text-on-surface">Top Pages</h3>
          </template>
          <div class="overflow-x-auto">
            <table class="w-full text-sm">
              <thead>
                <tr class="border-b border-outline-variant text-left">
                  <th class="pb-3 font-medium text-on-surface-variant">Page URL</th>
                  <th class="pb-3 font-medium text-on-surface-variant text-right">Views</th>
                  <th class="pb-3 font-medium text-on-surface-variant text-right">Bounce Rate</th>
                  <th class="pb-3 font-medium text-on-surface-variant text-right">Avg Duration</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-outline-variant">
                <tr v-for="page in topPages" :key="page.url">
                  <td class="py-3 text-on-surface font-medium">{{ page.url }}</td>
                  <td class="py-3 text-on-surface-variant text-right">{{ page.views }}</td>
                  <td class="py-3 text-on-surface-variant text-right">{{ page.bounce }}</td>
                  <td class="py-3 text-on-surface-variant text-right">{{ page.duration }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </WxCard>
      </div>

      <WxCard>
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">Referral Sources</h3>
        </template>
        <div class="space-y-4">
          <div v-for="ref in referralSources" :key="ref.domain" class="space-y-1.5">
            <div class="flex items-center justify-between text-sm">
              <span class="font-medium text-on-surface">{{ ref.domain }}</span>
              <span class="text-xs text-on-surface-variant">{{ ref.visits.toLocaleString() }}</span>
            </div>
            <div class="h-1.5 rounded-full bg-surface-container-high overflow-hidden">
              <div
                class="h-full rounded-full bg-gradient-to-r from-cyan-500 to-blue-500 transition-all duration-500"
                :style="{ width: `${ref.pct}%` }"
              />
            </div>
          </div>
        </div>
      </WxCard>
    </div>
  </div>
</template>
