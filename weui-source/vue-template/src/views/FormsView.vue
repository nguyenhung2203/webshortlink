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
import WxTabs from '@/components/ui/WxTabs.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxTag from '@/components/ui/WxTag.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxSpinner from '@/components/ui/WxSpinner.vue'
import WxProgressBar from '@/components/ui/WxProgressBar.vue'
import WxPagination from '@/components/ui/WxPagination.vue'
import WxSkeleton from '@/components/ui/WxSkeleton.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'

// -- Reactive state for interactive demos --

// Inputs
const inputVal = ref('')
const searchVal = ref('')
const passwordVal = ref('')
const textareaVal = ref('')
const numberVal = ref(42)
const errorInputVal = ref('Invalid value')

// Select
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

// Controls
const switchOn = ref(true)
const switchOff = ref(false)
const radioVal = ref('option1')
const checkVal = ref(true)
const checkVal2 = ref(false)
const sliderVal = ref(65)

// Tabs
const underlineTab = ref<string | number>('tab1')
const underlineTabs = [
  { key: 'tab1', label: 'Overview' },
  { key: 'tab2', label: 'Analytics' },
  { key: 'tab3', label: 'Reports' },
  { key: 'tab4', label: 'Disabled', disabled: true },
]
const pillTab = ref<string | number>('p1')
const pillTabs = [
  { key: 'p1', label: 'All' },
  { key: 'p2', label: 'Active' },
  { key: 'p3', label: 'Archived' },
]

// Tags
const removableTags = ref(['Vue.js', 'TypeScript', 'Tailwind'])

// Pagination
const currentPage = ref(1)

// Progress
const progress1 = ref(72)
const progress2 = ref(45)
const progress3 = ref(88)
</script>

<template>
  <div class="space-y-10">
    <WxPageHeader
      title="Primitives"
      description="A comprehensive showcase of all primitive UI components available in the design system."
    />

    <!-- ==================== 1. WxButton ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Zap class="w-5 h-5" />
        <span>WxButton</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <!-- Variants -->
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

        <!-- Sizes -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Sizes</h4>
          <div class="flex flex-wrap items-center gap-3">
            <WxButton size="sm">Small</WxButton>
            <WxButton size="md">Medium</WxButton>
            <WxButton size="lg">Large</WxButton>
          </div>
        </div>

        <!-- States -->
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
    </section>

    <!-- ==================== 2. Inputs ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <FileText class="w-5 h-5" />
        <span>Inputs</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <!-- Text Input with User icon -->
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">WXINPUT</label>
            <WxInput v-model="inputVal" placeholder="Nhap text...">
              <template #prefix><User class="w-4 h-4" /></template>
            </WxInput>
            <p class="text-xs text-on-surface-variant">Ho tro icon, clearable, error</p>
          </div>

          <!-- Search Input with Search icon -->
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Search</label>
            <WxInput v-model="searchVal" type="search" placeholder="Tim kiem...">
              <template #prefix><Search class="w-4 h-4" /></template>
            </WxInput>
          </div>

          <!-- Password Input with Lock icon -->
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Password</label>
            <WxPasswordInput v-model="passwordVal" />
          </div>

          <!-- Textarea -->
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Textarea</label>
            <WxTextarea v-model="textareaVal" placeholder="Write a description..." :rows="3" />
          </div>

          <!-- InputNumber -->
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Number Input</label>
            <WxInputNumber v-model="numberVal" :min="0" :max="100" />
          </div>

          <!-- Error state -->
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Error State</label>
            <WxInput v-model="errorInputVal" placeholder="Error example" :error="true" error-message="This field is required" />
          </div>
        </div>
      </div>
    </section>

    <!-- ==================== 3. Select & MultiSelect ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <List class="w-5 h-5" />
        <span>Select &amp; MultiSelect</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
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
      </div>
    </section>

    <!-- ==================== 4. WxDropdown ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <ChevronDown class="w-5 h-5" />
        <span>WxDropdown</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <div class="flex flex-wrap items-start gap-6">
          <!-- Text Menu -->
          <div class="space-y-1.5">
            <label class="block text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Text Menu</label>
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
          </div>

          <!-- Profile Menu -->
          <div class="space-y-1.5">
            <label class="block text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Profile Menu</label>
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
                <div class="px-3 py-2 text-sm text-on-surface hover:bg-surface-container cursor-pointer flex items-center gap-2" @click="close()">
                  <Settings class="w-4 h-4" /> Settings
                </div>
                <div class="border-t border-outline-variant my-1" />
                <div class="px-3 py-2 text-sm text-danger hover:bg-surface-container cursor-pointer flex items-center gap-2" @click="close()">
                  <X class="w-4 h-4" /> Sign Out
                </div>
              </template>
            </WxDropdown>
          </div>
        </div>
      </div>
    </section>

    <!-- ==================== 5. Controls ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Settings class="w-5 h-5" />
        <span>Controls</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
          <!-- Switch -->
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Switch</h4>
            <div class="space-y-2">
              <WxSwitch v-model="switchOn" label="Enabled" />
              <WxSwitch v-model="switchOff" label="Disabled" />
            </div>
          </div>

          <!-- Radio -->
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Radio</h4>
            <div class="space-y-2">
              <WxRadio v-model="radioVal" value="option1" label="Option One" name="demo-radio" />
              <WxRadio v-model="radioVal" value="option2" label="Option Two" name="demo-radio" />
              <WxRadio v-model="radioVal" value="option3" label="Option Three" name="demo-radio" />
            </div>
          </div>

          <!-- Checkbox -->
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Checkbox</h4>
            <div class="space-y-2">
              <WxCheckbox v-model="checkVal" label="Checked item" />
              <WxCheckbox v-model="checkVal2" label="Unchecked item" />
            </div>
          </div>

          <!-- Slider -->
          <div class="space-y-3">
            <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Slider</h4>
            <WxSlider v-model="sliderVal" :min="0" :max="100" show-value />
          </div>
        </div>
      </div>
    </section>

    <!-- ==================== 6. WxTabs ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <List class="w-5 h-5" />
        <span>WxTabs</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <!-- Underline variant -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Underline Variant</h4>
          <WxTabs v-model="underlineTab" :items="underlineTabs">
            <template #default="{ activeKey }">
              <div class="text-sm text-on-surface-variant">
                Content for tab: <span class="font-semibold text-on-surface">{{ activeKey }}</span>
              </div>
            </template>
          </WxTabs>
        </div>

        <!-- Pills variant (simulated with custom styling) -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Pills Variant</h4>
          <div class="flex gap-1 bg-surface-container rounded-lg p-1 w-fit">
            <button
              v-for="tab in pillTabs"
              :key="tab.key"
              class="px-4 py-1.5 text-sm font-medium rounded-md transition-all duration-200 cursor-pointer"
              :class="pillTab === tab.key
                ? 'bg-primary text-white shadow-sm'
                : 'text-on-surface-variant hover:text-on-surface'"
              @click="pillTab = tab.key"
            >
              {{ tab.label }}
            </button>
          </div>
        </div>
      </div>
    </section>

    <!-- ==================== 7. WxAvatar ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <User class="w-5 h-5" />
        <span>WxAvatar</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <!-- Sizes -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Sizes</h4>
          <div class="flex items-center gap-4">
            <WxAvatar name="Alice Nguyen" size="sm" />
            <WxAvatar name="Bob Tran" size="md" />
            <WxAvatar name="Charlie Pham" size="lg" />
            <WxAvatar name="Diana Le" size="xl" />
          </div>
        </div>

        <!-- With status -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">With Status</h4>
          <div class="flex items-center gap-4">
            <WxAvatar name="Online User" size="md" status="online" />
            <WxAvatar name="Offline User" size="md" status="offline" />
            <WxAvatar name="Away User" size="md" status="away" />
          </div>
        </div>
      </div>
    </section>

    <!-- ==================== 8. WxTag ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Zap class="w-5 h-5" />
        <span>WxTag</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <!-- Variants -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Variants</h4>
          <div class="flex flex-wrap items-center gap-2">
            <WxTag variant="default">DEFAULT</WxTag>
            <WxTag variant="primary">PRIMARY</WxTag>
            <WxTag variant="success">SUCCESS</WxTag>
            <WxTag variant="error">DANGER</WxTag>
            <WxTag variant="warning">WARNING</WxTag>
          </div>
        </div>

        <!-- Removable -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Removable Tags</h4>
          <div class="flex flex-wrap items-center gap-2">
            <WxTag
              v-for="tag in removableTags"
              :key="tag"
              variant="primary"
              removable
              @remove="removableTags = removableTags.filter(t => t !== tag)"
            >
              {{ tag }}
            </WxTag>
            <span v-if="removableTags.length === 0" class="text-sm text-on-surface-variant italic">All tags removed. Refresh to reset.</span>
          </div>
        </div>
      </div>
    </section>

    <!-- ==================== 9. WxBadge ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Bell class="w-5 h-5" />
        <span>WxBadge</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <!-- Badge variants -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Variants</h4>
          <div class="flex flex-wrap items-center gap-4">
            <WxBadge variant="info">3</WxBadge>
            <WxBadge variant="success">12</WxBadge>
            <WxBadge variant="warning">5</WxBadge>
            <WxBadge variant="error">99</WxBadge>
            <WxBadge variant="neutral">0</WxBadge>
          </div>
        </div>

        <!-- Sizes -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Sizes</h4>
          <div class="flex flex-wrap items-center gap-4">
            <WxBadge variant="info" size="sm">4</WxBadge>
            <WxBadge variant="info" size="md">8</WxBadge>
            <WxBadge variant="info" size="lg">16</WxBadge>
          </div>
        </div>

        <!-- Dot indicators -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Dot Indicators</h4>
          <div class="flex flex-wrap items-center gap-4">
            <div class="flex items-center gap-2">
              <WxBadge variant="success" dot />
              <span class="text-sm text-on-surface">Online</span>
            </div>
            <div class="flex items-center gap-2">
              <WxBadge variant="error" dot />
              <span class="text-sm text-on-surface">Error</span>
            </div>
            <div class="flex items-center gap-2">
              <WxBadge variant="warning" dot />
              <span class="text-sm text-on-surface">Warning</span>
            </div>
            <div class="flex items-center gap-2">
              <WxBadge variant="neutral" dot />
              <span class="text-sm text-on-surface">Inactive</span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ==================== 10. Spinner & ProgressBar ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Loader2 class="w-5 h-5" />
        <span>Spinner &amp; ProgressBar</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <!-- Spinners -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Spinner Sizes</h4>
          <div class="flex items-center gap-6">
            <div class="flex flex-col items-center gap-2">
              <WxSpinner size="sm" />
              <span class="text-xs text-on-surface-variant">Small</span>
            </div>
            <div class="flex flex-col items-center gap-2">
              <WxSpinner size="md" />
              <span class="text-xs text-on-surface-variant">Medium</span>
            </div>
            <div class="flex flex-col items-center gap-2">
              <WxSpinner size="lg" />
              <span class="text-xs text-on-surface-variant">Large</span>
            </div>
          </div>
        </div>

        <!-- Progress Bars -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Progress Bars</h4>
          <div class="space-y-4 max-w-lg">
            <WxProgressBar :value="progress1" variant="primary" show-value>
              <span class="text-sm text-on-surface">Uploading...</span>
            </WxProgressBar>
            <WxProgressBar :value="progress2" variant="warning" show-value>
              <span class="text-sm text-on-surface">Processing</span>
            </WxProgressBar>
            <WxProgressBar :value="progress3" variant="success" show-value>
              <span class="text-sm text-on-surface">Complete</span>
            </WxProgressBar>
            <WxProgressBar :value="30" variant="danger" show-value>
              <span class="text-sm text-on-surface">Failed</span>
            </WxProgressBar>
          </div>
        </div>
      </div>
    </section>

    <!-- ==================== 11. WxPagination ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <List class="w-5 h-5" />
        <span>WxPagination</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Full Pagination</h4>
          <WxPagination
            v-model:current-page="currentPage"
            :total-pages="12"
            :page-size="10"
            :total-items="115"
          />
        </div>
      </div>
    </section>

    <!-- ==================== 12. WxSkeleton ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Loader2 class="w-5 h-5" />
        <span>WxSkeleton</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Loading Animation</h4>
          <div class="flex gap-6">
            <!-- Card skeleton -->
            <div class="flex-1 space-y-3">
              <div class="flex items-center gap-3">
                <WxSkeleton variant="circular" width="2.5rem" />
                <div class="flex-1 space-y-2">
                  <WxSkeleton variant="text" width="60%" />
                  <WxSkeleton variant="text" width="40%" />
                </div>
              </div>
              <WxSkeleton variant="rectangular" height="6rem" />
              <WxSkeleton variant="text" :lines="3" />
            </div>

            <!-- Another card skeleton -->
            <div class="flex-1 space-y-3">
              <WxSkeleton variant="rectangular" height="8rem" />
              <WxSkeleton variant="text" width="75%" />
              <WxSkeleton variant="text" :lines="2" />
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ==================== 13. Cards ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Home class="w-5 h-5" />
        <span>WxCard</span>
      </div>
      <div class="bg-primary/10 p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Glass Morphism Cards</h4>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <!-- Glass Heavy -->
            <div class="backdrop-blur-xl bg-white/10 border border-white/20 rounded-xl p-6 shadow-lg">
              <h5 class="text-sm font-bold text-on-surface mb-2">Glass Heavy</h5>
              <p class="text-xs text-on-surface-variant">backdrop-blur-xl with 10% white background for a frosted glass effect.</p>
            </div>

            <!-- Glass Medium -->
            <div class="backdrop-blur-md bg-white/15 border border-white/25 rounded-xl p-6 shadow-lg">
              <h5 class="text-sm font-bold text-on-surface mb-2">Glass Medium</h5>
              <p class="text-xs text-on-surface-variant">backdrop-blur-md with 15% white background for moderate transparency.</p>
            </div>

            <!-- Glass Light -->
            <div class="backdrop-blur-sm bg-white/20 border border-white/30 rounded-xl p-6 shadow-lg">
              <h5 class="text-sm font-bold text-on-surface mb-2">Glass Light</h5>
              <p class="text-xs text-on-surface-variant">backdrop-blur-sm with 20% white background for subtle frosting.</p>
            </div>
          </div>
        </div>

        <!-- Standard Card -->
        <div class="space-y-3">
          <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant">Standard Card</h4>
          <div class="max-w-md">
            <WxCard>
              <template #header>
                <h3 class="text-sm font-semibold text-on-surface">Card Header</h3>
              </template>
              <p class="text-sm text-on-surface-variant">This is a standard WxCard with header, body, and footer slots.</p>
              <template #footer>
                <div class="flex justify-end gap-2">
                  <WxButton variant="ghost" size="sm">Cancel</WxButton>
                  <WxButton variant="primary" size="sm">Confirm</WxButton>
                </div>
              </template>
            </WxCard>
          </div>
        </div>
      </div>
    </section>

    <!-- ==================== 14. WxEmptyState ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <FileText class="w-5 h-5" />
        <span>WxEmptyState</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-8 transition-colors duration-300">
        <WxEmptyState
          title="No results found"
          description="Try adjusting your search or filter criteria to find what you are looking for."
        >
          <template #action>
            <WxButton variant="primary" size="sm">
              <template #icon><Plus class="w-4 h-4" /></template>
              Create New
            </WxButton>
          </template>
        </WxEmptyState>
      </div>
    </section>
  </div>
</template>
