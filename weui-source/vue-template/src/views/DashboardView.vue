<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxStatCard from '@/components/ui/WxStatCard.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import { DollarSign, Megaphone, TrendingUp, Users } from 'lucide-vue-next'

const weeklyData = ref([
  { label: 'Mon', value: 65 },
  { label: 'Tue', value: 40 },
  { label: 'Wed', value: 85 },
  { label: 'Thu', value: 55 },
  { label: 'Fri', value: 72 },
  { label: 'Sat', value: 30 },
  { label: 'Sun', value: 48 },
])

const recentActivity = ref([
  { user: 'Nguyen Minh Tuan', action: 'Created campaign', target: 'Tet Holiday Sale', time: '5 min ago', status: 'success' },
  { user: 'Tran Thi Mai', action: 'Updated budget for', target: 'Spring Collection', time: '12 min ago', status: 'info' },
  { user: 'Le Van Hung', action: 'Paused campaign', target: 'Flash Deal Weekend', time: '28 min ago', status: 'warning' },
  { user: 'Pham Ngoc Linh', action: 'Approved creative for', target: 'Brand Awareness Q1', time: '1 hr ago', status: 'success' },
  { user: 'Vo Thanh Dat', action: 'Flagged issue on', target: 'Retargeting Ads', time: '2 hr ago', status: 'error' },
])

const maxBar = Math.max(...weeklyData.value.map(d => d.value))
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Dashboard" description="Overview of your campaigns and performance metrics." />

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <WxStatCard title="Total Revenue" value="$45,231" :trend="12.5" :trend-up="true">
        <template #icon><DollarSign :size="20" /></template>
      </WxStatCard>
      <WxStatCard title="Active Campaigns" value="12" :trend="3.2" :trend-up="true">
        <template #icon><Megaphone :size="20" /></template>
      </WxStatCard>
      <WxStatCard title="Conversion Rate" value="3.2%" :trend="0.8" :trend-up="false">
        <template #icon><TrendingUp :size="20" /></template>
      </WxStatCard>
      <WxStatCard title="Active Users" value="573" :trend="8.1" :trend-up="true">
        <template #icon><Users :size="20" /></template>
      </WxStatCard>
    </div>

    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Weekly Performance</h3>
      </template>
      <div class="flex items-end gap-3 h-48">
        <div
          v-for="day in weeklyData"
          :key="day.label"
          class="flex-1 flex flex-col items-center gap-2"
        >
          <span class="text-xs font-medium text-on-surface-variant">{{ day.value }}%</span>
          <div class="w-full max-w-[40px] rounded-t-md bg-gradient-to-t from-cyan-500 to-blue-500 transition-all duration-500"
            :style="{ height: `${(day.value / maxBar) * 100}%` }"
          />
          <span class="text-xs text-on-surface-variant">{{ day.label }}</span>
        </div>
      </div>
    </WxCard>

    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Recent Activity</h3>
      </template>
      <div class="divide-y divide-outline-variant">
        <div
          v-for="(item, i) in recentActivity"
          :key="i"
          class="flex items-center gap-3 py-3 first:pt-0 last:pb-0"
        >
          <div class="w-8 h-8 rounded-full bg-surface-container-high flex items-center justify-center text-xs font-bold text-on-surface-variant shrink-0">
            {{ item.user.split(' ').map(n => n[0]).slice(0, 2).join('') }}
          </div>
          <div class="flex-1 min-w-0">
            <p class="text-sm text-on-surface truncate">
              <span class="font-medium">{{ item.user }}</span>
              {{ item.action }}
              <span class="font-medium">{{ item.target }}</span>
            </p>
            <p class="text-xs text-on-surface-variant">{{ item.time }}</p>
          </div>
          <WxBadge :variant="item.status as 'success' | 'info' | 'warning' | 'error'" size="sm" dot />
        </div>
      </div>
    </WxCard>
  </div>
</template>
