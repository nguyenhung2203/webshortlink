<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxProgressBar from '@/components/ui/WxProgressBar.vue'
import { ShoppingCart, Users, DollarSign, MessageSquare, ArrowUp, ArrowDown, Cloud, Heart, UserPlus } from 'lucide-vue-next'

const infoBoxes = ref([
  { icon: ShoppingCart, label: 'New Orders', value: '1,250', borderColor: 'border-l-info', iconBg: 'bg-info/10 text-info' },
  { icon: Users, label: 'Total Users', value: '5,830', borderColor: 'border-l-emerald-500', iconBg: 'bg-emerald-500/10 text-emerald-500' },
  { icon: DollarSign, label: 'Revenue', value: '$32,400', borderColor: 'border-l-amber-500', iconBg: 'bg-amber-500/10 text-amber-500' },
  { icon: MessageSquare, label: 'Messages', value: '482', borderColor: 'border-l-danger', iconBg: 'bg-danger/10 text-danger' },
])

const smallBoxes = ref([
  { pct: 17, up: true, label: 'Bounce Rate', bg: 'bg-gradient-to-br from-blue-500 to-blue-700' },
  { pct: 8.5, up: true, label: 'Session Duration', bg: 'bg-gradient-to-br from-emerald-500 to-emerald-700' },
  { pct: 3.2, up: false, label: 'Page Views', bg: 'bg-gradient-to-br from-amber-500 to-amber-700' },
  { pct: 12, up: true, label: 'Conversions', bg: 'bg-gradient-to-br from-purple-500 to-purple-700' },
])

const progressItems = ref([
  { label: 'Server Migration', value: 78, variant: 'primary' as const },
  { label: 'Database Backup', value: 92, variant: 'success' as const },
  { label: 'Bug Fixes', value: 55, variant: 'warning' as const },
  { label: 'UI Redesign', value: 34, variant: 'danger' as const },
])
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Widgets" description="Showcase of reusable widget components for dashboards and admin pages." />

    <!-- Info Box Widgets -->
    <h2 class="text-lg font-semibold text-on-surface">Info Box Widgets</h2>
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div
        v-for="box in infoBoxes"
        :key="box.label"
        class="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm border-l-4 p-5 flex items-center gap-4"
        :class="box.borderColor"
      >
        <div class="p-2.5 rounded-lg shrink-0" :class="box.iconBg">
          <component :is="box.icon" :size="22" />
        </div>
        <div>
          <p class="text-2xl font-bold text-on-surface">{{ box.value }}</p>
          <p class="text-xs text-on-surface-variant font-medium">{{ box.label }}</p>
        </div>
      </div>
    </div>

    <!-- Small Box Widgets -->
    <h2 class="text-lg font-semibold text-on-surface">Small Box Widgets</h2>
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div
        v-for="box in smallBoxes"
        :key="box.label"
        class="rounded-xl p-5 text-white shadow-sm"
        :class="box.bg"
      >
        <div class="flex items-center gap-1.5 text-white/90 mb-1">
          <component :is="box.up ? ArrowUp : ArrowDown" :size="16" />
          <span class="text-lg font-bold">{{ box.pct }}%</span>
        </div>
        <p class="text-sm text-white/80">{{ box.label }}</p>
      </div>
    </div>

    <!-- Card Widgets -->
    <h2 class="text-lg font-semibold text-on-surface">Card Widgets</h2>
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <!-- User Profile Card -->
      <WxCard>
        <div class="flex flex-col items-center text-center py-2">
          <WxAvatar name="Alex Johnson" size="xl" status="online" />
          <h3 class="mt-3 text-base font-semibold text-on-surface">Alex Johnson</h3>
          <p class="text-sm text-on-surface-variant">Product Manager</p>
          <div class="grid grid-cols-3 gap-4 mt-5 w-full border-t border-outline-variant pt-4">
            <div>
              <p class="text-lg font-bold text-on-surface">142</p>
              <p class="text-xs text-on-surface-variant">Projects</p>
            </div>
            <div>
              <p class="text-lg font-bold text-on-surface">87</p>
              <p class="text-xs text-on-surface-variant">Tasks</p>
            </div>
            <div>
              <p class="text-lg font-bold text-on-surface">12</p>
              <p class="text-xs text-on-surface-variant">Teams</p>
            </div>
          </div>
        </div>
      </WxCard>

      <!-- Social Card -->
      <WxCard>
        <div class="flex flex-col items-center text-center py-2">
          <div class="w-16 h-16 rounded-full bg-gradient-to-br from-pink-500 to-rose-500 flex items-center justify-center text-white">
            <Heart :size="28" />
          </div>
          <h3 class="mt-3 text-base font-semibold text-on-surface">Social Stats</h3>
          <p class="text-sm text-on-surface-variant mb-4">Engagement overview</p>
          <div class="grid grid-cols-2 gap-4 w-full border-t border-outline-variant pt-4">
            <div class="flex items-center gap-2 justify-center">
              <UserPlus :size="16" class="text-primary" />
              <div class="text-left">
                <p class="text-base font-bold text-on-surface">8.4K</p>
                <p class="text-xs text-on-surface-variant">Followers</p>
              </div>
            </div>
            <div class="flex items-center gap-2 justify-center">
              <Heart :size="16" class="text-danger" />
              <div class="text-left">
                <p class="text-base font-bold text-on-surface">24.1K</p>
                <p class="text-xs text-on-surface-variant">Likes</p>
              </div>
            </div>
          </div>
        </div>
      </WxCard>

      <!-- Weather Card -->
      <WxCard>
        <div class="flex flex-col items-center text-center py-2">
          <div class="w-16 h-16 rounded-full bg-gradient-to-br from-sky-400 to-cyan-500 flex items-center justify-center text-white">
            <Cloud :size="28" />
          </div>
          <h3 class="mt-3 text-base font-semibold text-on-surface">Ho Chi Minh City</h3>
          <p class="text-4xl font-bold text-on-surface mt-2">32&deg;C</p>
          <p class="text-sm text-on-surface-variant">Partly Cloudy</p>
          <div class="grid grid-cols-3 gap-4 mt-4 w-full border-t border-outline-variant pt-4">
            <div>
              <p class="text-sm font-bold text-on-surface">72%</p>
              <p class="text-xs text-on-surface-variant">Humidity</p>
            </div>
            <div>
              <p class="text-sm font-bold text-on-surface">12 km/h</p>
              <p class="text-xs text-on-surface-variant">Wind</p>
            </div>
            <div>
              <p class="text-sm font-bold text-on-surface">UV 6</p>
              <p class="text-xs text-on-surface-variant">Index</p>
            </div>
          </div>
        </div>
      </WxCard>
    </div>

    <!-- Progress Widgets -->
    <h2 class="text-lg font-semibold text-on-surface">Progress Widgets</h2>
    <WxCard>
      <div class="space-y-5">
        <div v-for="item in progressItems" :key="item.label" class="space-y-2">
          <div class="flex items-center justify-between text-sm">
            <span class="font-medium text-on-surface">{{ item.label }}</span>
            <span class="text-on-surface-variant">{{ item.value }}%</span>
          </div>
          <WxProgressBar :value="item.value" :variant="item.variant" size="md" show-value />
        </div>
      </div>
    </WxCard>
  </div>
</template>
