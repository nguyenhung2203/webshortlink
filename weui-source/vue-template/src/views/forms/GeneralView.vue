<script setup lang="ts">
import { ref } from 'vue'
import {
  Zap, User, Search, Lock, Check, Loader2, Bell, Settings,
  Trash2, FileText, Home, List, ChevronDown, X, Plus,
} from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import WxTextarea from '@/components/ui/WxTextarea.vue'
import WxInputNumber from '@/components/ui/WxInputNumber.vue'
import WxSelect from '@/components/ui/WxSelect.vue'
import WxMultiSelect from '@/components/ui/WxMultiSelect.vue'
import WxDropdown from '@/components/ui/WxDropdown.vue'
import WxSwitch from '@/components/ui/WxSwitch.vue'
import WxRadio from '@/components/ui/WxRadio.vue'
import WxCheckbox from '@/components/ui/WxCheckbox.vue'
import WxSlider from '@/components/ui/WxSlider.vue'
import WxCard from '@/components/ui/WxCard.vue'

// -- Reactive state --
const inputVal = ref('')
const searchVal = ref('')
const passwordVal = ref('')
const textareaVal = ref('')
const numberVal = ref(42)
const errorInputVal = ref('Invalid value')
const disabledInputVal = ref('Cannot edit')

const selectVal = ref<string | number | null>(null)
const selectOptions = [
  { label: 'Facebook Ads', value: 'facebook' },
  { label: 'Google Ads', value: 'google' },
  { label: 'TikTok Ads', value: 'tiktok' },
  { label: 'Zalo Ads', value: 'zalo' },
]
const multiVal = ref<(string | number)[]>([])
const multiOptions = [
  { label: 'Ho Chi Minh City', value: 'hcm' },
  { label: 'Ha Noi', value: 'hn' },
  { label: 'Da Nang', value: 'dn' },
  { label: 'Can Tho', value: 'ct' },
  { label: 'Hai Phong', value: 'hp' },
]

const switchOn = ref(true)
const switchOff = ref(false)
const radioVal = ref('option1')
const checkVal = ref(true)
const checkVal2 = ref(false)
const checkVal3 = ref(false)
const sliderVal = ref(65)

const readonlyVal = ref('Read-only content')
</script>

<template>
  <div class="space-y-10">
    <WxPageHeader
      title="General Form Elements"
      description="A comprehensive showcase of all form primitives available in the design system."
    />

    <!-- ==================== Buttons ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Zap class="w-5 h-5" />
        <span>Buttons</span>
      </div>
      <WxCard>
        <div class="space-y-8">
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Variants</h4>
            <div class="flex flex-wrap items-center gap-3">
              <WxButton variant="primary">Primary</WxButton>
              <WxButton variant="cta">CTA</WxButton>
              <WxButton variant="secondary">Secondary</WxButton>
              <WxButton variant="ghost">Ghost</WxButton>
              <WxButton variant="danger">Danger</WxButton>
              <WxButton variant="success">Success</WxButton>
              <WxButton variant="text">Text</WxButton>
            </div>
          </div>
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Sizes</h4>
            <div class="flex flex-wrap items-center gap-3">
              <WxButton size="sm">Small</WxButton>
              <WxButton size="md">Medium</WxButton>
              <WxButton size="lg">Large</WxButton>
            </div>
          </div>
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">States</h4>
            <div class="flex flex-wrap items-center gap-3">
              <WxButton variant="primary">
                <template #icon><Zap class="w-4 h-4" /></template>
                With Icon
              </WxButton>
              <WxButton variant="primary" :loading="true">Loading</WxButton>
              <WxButton variant="primary" :disabled="true">Disabled</WxButton>
              <WxButton variant="danger">
                <template #icon><Trash2 class="w-4 h-4" /></template>
                Delete
              </WxButton>
              <WxButton variant="secondary" size="sm">
                <template #icon><Plus class="w-4 h-4" /></template>
              </WxButton>
            </div>
          </div>
        </div>
      </WxCard>
    </section>

    <!-- ==================== Text Inputs ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <FileText class="w-5 h-5" />
        <span>Text Inputs</span>
      </div>
      <WxCard>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Default Input</label>
            <WxInput v-model="inputVal" placeholder="Enter text...">
              <template #prefix><User class="w-4 h-4" /></template>
            </WxInput>
          </div>
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Search</label>
            <WxInput v-model="searchVal" type="search" placeholder="Search...">
              <template #prefix><Search class="w-4 h-4" /></template>
            </WxInput>
          </div>
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Password</label>
            <WxPasswordInput v-model="passwordVal" />
          </div>
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Textarea</label>
            <WxTextarea v-model="textareaVal" placeholder="Write a description..." :rows="3" />
          </div>
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Number Input</label>
            <WxInputNumber v-model="numberVal" :min="0" :max="100" />
          </div>
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Error State</label>
            <WxInput v-model="errorInputVal" :error="true" error-message="This field is required" />
          </div>
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Disabled</label>
            <WxInput v-model="disabledInputVal" :disabled="true" />
          </div>
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Read-only</label>
            <WxInput v-model="readonlyVal" :readonly="true" />
          </div>
        </div>
      </WxCard>
    </section>

    <!-- ==================== Select & MultiSelect ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <List class="w-5 h-5" />
        <span>Select &amp; MultiSelect</span>
      </div>
      <WxCard>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Single Select</label>
            <WxSelect v-model="selectVal" :options="selectOptions" placeholder="Choose a platform..." />
          </div>
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Multi Select</label>
            <WxMultiSelect v-model="multiVal" :options="multiOptions" placeholder="Select regions..." />
          </div>
        </div>
      </WxCard>
    </section>

    <!-- ==================== Dropdown ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <ChevronDown class="w-5 h-5" />
        <span>Dropdown Menus</span>
      </div>
      <WxCard>
        <div class="flex flex-wrap items-start gap-6">
          <WxDropdown>
            <template #trigger>
              <WxButton variant="secondary" size="sm">
                <template #icon><Settings class="w-4 h-4" /></template>
                Options
              </WxButton>
            </template>
            <template #default="{ close }">
              <div class="px-3 py-2 text-sm text-on-surface hover:bg-surface-container cursor-pointer flex items-center gap-2" @click="close()">
                <FileText class="w-4 h-4" /> Duplicate
              </div>
              <div class="px-3 py-2 text-sm text-on-surface hover:bg-surface-container cursor-pointer flex items-center gap-2" @click="close()">
                <Settings class="w-4 h-4" /> Settings
              </div>
              <div class="border-t border-outline-variant my-1" />
              <div class="px-3 py-2 text-sm text-danger hover:bg-surface-container cursor-pointer flex items-center gap-2" @click="close()">
                <Trash2 class="w-4 h-4" /> Delete
              </div>
            </template>
          </WxDropdown>
          <WxDropdown>
            <template #trigger>
              <WxButton variant="ghost" size="sm">
                <template #icon><User class="w-4 h-4" /></template>
                Profile
              </WxButton>
            </template>
            <template #default="{ close }">
              <div class="px-3 py-2 text-sm text-on-surface hover:bg-surface-container cursor-pointer flex items-center gap-2" @click="close()">
                <User class="w-4 h-4" /> My Account
              </div>
              <div class="px-3 py-2 text-sm text-on-surface hover:bg-surface-container cursor-pointer flex items-center gap-2" @click="close()">
                <Bell class="w-4 h-4" /> Notifications
              </div>
              <div class="border-t border-outline-variant my-1" />
              <div class="px-3 py-2 text-sm text-danger hover:bg-surface-container cursor-pointer flex items-center gap-2" @click="close()">
                <X class="w-4 h-4" /> Sign Out
              </div>
            </template>
          </WxDropdown>
        </div>
      </WxCard>
    </section>

    <!-- ==================== Controls ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Settings class="w-5 h-5" />
        <span>Controls</span>
      </div>
      <WxCard>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Switch</h4>
            <div class="space-y-2">
              <WxSwitch v-model="switchOn" label="Enabled" />
              <WxSwitch v-model="switchOff" label="Disabled toggle" />
            </div>
          </div>
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Radio</h4>
            <div class="space-y-2">
              <WxRadio v-model="radioVal" value="option1" label="Option One" name="demo-radio" />
              <WxRadio v-model="radioVal" value="option2" label="Option Two" name="demo-radio" />
              <WxRadio v-model="radioVal" value="option3" label="Option Three" name="demo-radio" />
            </div>
          </div>
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Checkbox</h4>
            <div class="space-y-2">
              <WxCheckbox v-model="checkVal" label="Checked item" />
              <WxCheckbox v-model="checkVal2" label="Unchecked item" />
              <WxCheckbox v-model="checkVal3" label="Another option" />
            </div>
          </div>
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Slider</h4>
            <WxSlider v-model="sliderVal" :min="0" :max="100" show-value />
            <p class="text-xs text-on-surface-variant">Current value: {{ sliderVal }}</p>
          </div>
        </div>
      </WxCard>
    </section>
  </div>
</template>
