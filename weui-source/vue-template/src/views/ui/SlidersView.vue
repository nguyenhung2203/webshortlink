<script setup lang="ts">
import { ref } from 'vue'
import { Sun, Volume2, Droplets, Gauge, Thermometer, Zap } from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxSlider from '@/components/ui/WxSlider.vue'
import WxProgressBar from '@/components/ui/WxProgressBar.vue'

const basicSlider = ref(50)
const percentSlider = ref(75)
const priceSlider = ref(250)
const stepSlider = ref(40)

const brightness = ref(80)
const volume = ref(65)
const opacity = ref(90)
const temperature = ref(22)
const speed = ref(120)
const power = ref(45)

const disabledSlider = ref(30)
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Sliders & Range"
      description="Slider controls with different configurations, labeled ranges, and complementary progress bars."
    />

    <!-- Basic Sliders -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Basic Sliders</h3>
      </template>
      <div class="space-y-6 max-w-xl">
        <div>
          <label class="block text-sm font-medium text-on-surface mb-2">Default (0-100)</label>
          <WxSlider v-model="basicSlider" show-value />
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-2">Percentage</label>
          <WxSlider v-model="percentSlider" :min="0" :max="100" show-value />
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-2">Price Range ($0 - $500)</label>
          <WxSlider v-model="priceSlider" :min="0" :max="500" :step="10" show-value />
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-2">Step: 20 (0-100)</label>
          <WxSlider v-model="stepSlider" :min="0" :max="100" :step="20" show-value />
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-2">Disabled</label>
          <WxSlider v-model="disabledSlider" disabled show-value />
        </div>
      </div>
    </WxCard>

    <!-- Sliders with Labels -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Sliders with Min/Max Labels</h3>
      </template>
      <div class="space-y-6 max-w-xl">
        <div>
          <label class="block text-sm font-medium text-on-surface mb-2">Price Range</label>
          <div class="flex items-center gap-3">
            <span class="text-xs text-on-surface-variant font-medium min-w-[2rem]">$0</span>
            <div class="flex-1">
              <WxSlider v-model="priceSlider" :min="0" :max="500" :step="10" />
            </div>
            <span class="text-xs text-on-surface-variant font-medium min-w-[2rem]">$500</span>
          </div>
          <p class="text-center text-sm text-primary font-semibold mt-2">${{ priceSlider }}</p>
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-2">Temperature</label>
          <div class="flex items-center gap-3">
            <span class="text-xs text-on-surface-variant font-medium">16&deg;C</span>
            <div class="flex-1">
              <WxSlider v-model="temperature" :min="16" :max="32" :step="1" />
            </div>
            <span class="text-xs text-on-surface-variant font-medium">32&deg;C</span>
          </div>
          <p class="text-center text-sm text-primary font-semibold mt-2">{{ temperature }}&deg;C</p>
        </div>
      </div>
    </WxCard>

    <!-- Multiple Sliders -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Control Panel</h3>
      </template>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div class="space-y-2">
          <div class="flex items-center gap-2">
            <Sun :size="16" class="text-warning" />
            <label class="text-sm font-medium text-on-surface">Brightness</label>
          </div>
          <WxSlider v-model="brightness" show-value />
        </div>
        <div class="space-y-2">
          <div class="flex items-center gap-2">
            <Volume2 :size="16" class="text-info" />
            <label class="text-sm font-medium text-on-surface">Volume</label>
          </div>
          <WxSlider v-model="volume" show-value />
        </div>
        <div class="space-y-2">
          <div class="flex items-center gap-2">
            <Droplets :size="16" class="text-primary" />
            <label class="text-sm font-medium text-on-surface">Opacity</label>
          </div>
          <WxSlider v-model="opacity" show-value />
        </div>
        <div class="space-y-2">
          <div class="flex items-center gap-2">
            <Thermometer :size="16" class="text-danger" />
            <label class="text-sm font-medium text-on-surface">Temperature</label>
          </div>
          <WxSlider v-model="temperature" :min="16" :max="32" show-value />
        </div>
        <div class="space-y-2">
          <div class="flex items-center gap-2">
            <Gauge :size="16" class="text-success" />
            <label class="text-sm font-medium text-on-surface">Speed (km/h)</label>
          </div>
          <WxSlider v-model="speed" :min="0" :max="240" :step="10" show-value />
        </div>
        <div class="space-y-2">
          <div class="flex items-center gap-2">
            <Zap :size="16" class="text-warning" />
            <label class="text-sm font-medium text-on-surface">Power Level</label>
          </div>
          <WxSlider v-model="power" show-value />
        </div>
      </div>
    </WxCard>

    <!-- Progress Bars -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Progress Bars</h3>
      </template>
      <div class="space-y-5 max-w-xl">
        <WxProgressBar :value="25" variant="primary" show-value>
          <span class="text-xs text-on-surface-variant">Project Alpha</span>
        </WxProgressBar>
        <WxProgressBar :value="60" variant="success" show-value>
          <span class="text-xs text-on-surface-variant">Upload Progress</span>
        </WxProgressBar>
        <WxProgressBar :value="85" variant="warning" show-value>
          <span class="text-xs text-on-surface-variant">Storage Used</span>
        </WxProgressBar>
        <WxProgressBar :value="95" variant="danger" show-value>
          <span class="text-xs text-on-surface-variant">CPU Usage</span>
        </WxProgressBar>
        <div class="pt-2">
          <p class="text-xs font-medium text-on-surface-variant mb-3">Sizes</p>
          <div class="space-y-3">
            <WxProgressBar :value="45" variant="primary" size="sm" />
            <WxProgressBar :value="45" variant="primary" size="md" />
            <WxProgressBar :value="45" variant="primary" size="lg" />
          </div>
        </div>
      </div>
    </WxCard>
  </div>
</template>
