<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'

const areaData = ref([30, 55, 40, 70, 50, 80, 60])
const areaLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
const areaMax = Math.max(...areaData.value)

const barData = ref([
  { label: 'Jan', value: 65 },
  { label: 'Feb', value: 40 },
  { label: 'Mar', value: 85 },
  { label: 'Apr', value: 55 },
  { label: 'May', value: 72 },
  { label: 'Jun', value: 48 },
  { label: 'Jul', value: 90 },
])
const barMax = Math.max(...barData.value.map(d => d.value))

const hBarData = ref([
  { label: 'Chrome', value: 64 },
  { label: 'Safari', value: 19 },
  { label: 'Firefox', value: 8 },
  { label: 'Edge', value: 6 },
  { label: 'Other', value: 3 },
])

const donutSegments = ref([
  { label: 'Desktop', value: 55, color: '#3b82f6' },
  { label: 'Mobile', value: 30, color: '#10b981' },
  { label: 'Tablet', value: 15, color: '#f59e0b' },
])

const linePoints = ref([20, 45, 35, 65, 50, 75, 55])
const lineLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']

const sparklines = ref([
  [3, 7, 4, 8, 5, 9, 6],
  [8, 4, 6, 3, 7, 2, 5],
  [2, 5, 3, 8, 6, 4, 7],
  [6, 3, 7, 5, 9, 4, 8],
])

function donutGradient() {
  let acc = 0
  const stops: string[] = []
  for (const seg of donutSegments.value) {
    stops.push(`${seg.color} ${acc}%`)
    acc += seg.value
    stops.push(`${seg.color} ${acc}%`)
  }
  return `conic-gradient(${stops.join(', ')})`
}

function linePointsSvg(width: number, height: number) {
  const max = Math.max(...linePoints.value)
  const padding = 10
  const w = width - padding * 2
  const h = height - padding * 2
  return linePoints.value.map((v, i) => ({
    x: padding + (i / (linePoints.value.length - 1)) * w,
    y: padding + h - (v / max) * h,
  }))
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Charts Gallery" description="CSS-only chart demonstrations with various chart types." />

    <!-- Area Chart -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Area Chart</h3>
      </template>
      <div class="relative h-48 flex items-end gap-0">
        <div
          v-for="(val, i) in areaData"
          :key="i"
          class="flex-1 flex flex-col items-center justify-end h-full relative"
        >
          <div
            class="w-full bg-gradient-to-t from-blue-500/30 to-blue-500/5 relative"
            :style="{ height: `${(val / areaMax) * 100}%` }"
          >
            <div class="absolute top-0 left-0 right-0 h-0.5 bg-primary" />
          </div>
          <span class="text-[10px] text-on-surface-variant mt-1">{{ areaLabels[i] }}</span>
        </div>
      </div>
    </WxCard>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Vertical Bar Chart -->
      <WxCard>
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">Bar Chart</h3>
        </template>
        <div class="flex items-end gap-3 h-44">
          <div v-for="bar in barData" :key="bar.label" class="flex-1 flex flex-col items-center gap-1.5">
            <span class="text-[10px] font-medium text-on-surface-variant">{{ bar.value }}%</span>
            <div
              class="w-full max-w-[36px] rounded-t-md bg-gradient-to-t from-violet-600 to-violet-400 transition-all duration-500"
              :style="{ height: `${(bar.value / barMax) * 100}%` }"
            />
            <span class="text-[10px] text-on-surface-variant">{{ bar.label }}</span>
          </div>
        </div>
      </WxCard>

      <!-- Horizontal Bar Chart -->
      <WxCard>
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">Horizontal Bar Chart</h3>
        </template>
        <div class="space-y-3">
          <div v-for="bar in hBarData" :key="bar.label" class="flex items-center gap-3">
            <span class="text-xs font-medium text-on-surface w-16 text-right shrink-0">{{ bar.label }}</span>
            <div class="flex-1 h-5 rounded-full bg-surface-container-high overflow-hidden">
              <div
                class="h-full rounded-full bg-gradient-to-r from-emerald-500 to-teal-400 transition-all duration-700 flex items-center justify-end pr-2"
                :style="{ width: `${bar.value}%` }"
              >
                <span v-if="bar.value > 10" class="text-[10px] font-bold text-white">{{ bar.value }}%</span>
              </div>
            </div>
          </div>
        </div>
      </WxCard>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Donut Chart -->
      <WxCard>
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">Donut Chart</h3>
        </template>
        <div class="flex items-center gap-8">
          <div class="relative shrink-0">
            <div
              class="w-36 h-36 rounded-full"
              :style="{ background: donutGradient() }"
            >
              <div class="absolute inset-4 rounded-full bg-surface-container-lowest flex items-center justify-center">
                <span class="text-lg font-bold text-on-surface">100%</span>
              </div>
            </div>
          </div>
          <div class="space-y-3">
            <div v-for="seg in donutSegments" :key="seg.label" class="flex items-center gap-2">
              <span class="w-3 h-3 rounded-sm shrink-0" :style="{ backgroundColor: seg.color }" />
              <span class="text-sm text-on-surface">{{ seg.label }}</span>
              <span class="text-sm font-semibold text-on-surface-variant ml-auto">{{ seg.value }}%</span>
            </div>
          </div>
        </div>
      </WxCard>

      <!-- Line Chart (SVG) -->
      <WxCard>
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">Line Chart</h3>
        </template>
        <div class="relative">
          <svg viewBox="0 0 320 160" class="w-full h-40">
            <!-- Grid lines -->
            <line v-for="i in 4" :key="'g'+i" x1="10" :y1="10 + (i - 1) * 46.67" x2="310" :y2="10 + (i - 1) * 46.67"
              stroke="currentColor" class="text-outline-variant" stroke-width="0.5" stroke-dasharray="4" />
            <!-- Line path -->
            <polyline
              :points="linePointsSvg(320, 160).map(p => `${p.x},${p.y}`).join(' ')"
              fill="none" stroke="#3b82f6" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"
            />
            <!-- Dots -->
            <circle
              v-for="(pt, i) in linePointsSvg(320, 160)"
              :key="i"
              :cx="pt.x" :cy="pt.y" r="4" fill="#3b82f6" stroke="white" stroke-width="2"
            />
          </svg>
          <div class="flex justify-between px-2 mt-1">
            <span v-for="label in lineLabels" :key="label" class="text-[10px] text-on-surface-variant">{{ label }}</span>
          </div>
        </div>
      </WxCard>
    </div>

    <!-- Mini Charts / Sparklines -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Mini Charts / Sparklines</h3>
      </template>
      <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
        <div v-for="(spark, si) in sparklines" :key="si" class="flex flex-col items-center gap-2">
          <div class="flex items-end gap-1 h-12">
            <div
              v-for="(v, vi) in spark"
              :key="vi"
              class="w-3 rounded-sm bg-gradient-to-t transition-all duration-300"
              :class="[
                si === 0 ? 'from-blue-500 to-blue-400' :
                si === 1 ? 'from-emerald-500 to-emerald-400' :
                si === 2 ? 'from-amber-500 to-amber-400' :
                'from-purple-500 to-purple-400'
              ]"
              :style="{ height: `${(v / 10) * 100}%` }"
            />
          </div>
          <span class="text-xs text-on-surface-variant">Series {{ si + 1 }}</span>
        </div>
      </div>
    </WxCard>
  </div>
</template>
