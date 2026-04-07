<script setup lang="ts">
import { ref } from 'vue'
import {
  Plus, Download, Trash2, Settings, ChevronDown, Send, Save,
  Share2, Heart, Search, Edit, Mail, Printer, RefreshCw,
  Bold, Italic, Underline, AlignLeft, AlignCenter, AlignRight,
} from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxDropdown from '@/components/ui/WxDropdown.vue'

const loadingBtn = ref(false)

function simulateLoading() {
  loadingBtn.value = true
  setTimeout(() => { loadingBtn.value = false }, 2000)
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Buttons"
      description="Button variants, sizes, states, icon buttons, button groups, dropdowns, and block buttons."
    />

    <!-- Variants -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Variants</h3>
      </template>
      <div class="flex flex-wrap items-center gap-3">
        <WxButton variant="primary">Primary</WxButton>
        <WxButton variant="cta">Call to Action</WxButton>
        <WxButton variant="secondary">Secondary</WxButton>
        <WxButton variant="ghost">Ghost</WxButton>
        <WxButton variant="danger">Danger</WxButton>
        <WxButton variant="success">Success</WxButton>
        <WxButton variant="text">Text</WxButton>
      </div>
    </WxCard>

    <!-- Sizes -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Sizes</h3>
      </template>
      <div class="flex flex-wrap items-end gap-3">
        <WxButton variant="primary" size="sm">Small</WxButton>
        <WxButton variant="primary" size="md">Medium</WxButton>
        <WxButton variant="primary" size="lg">Large</WxButton>
      </div>
      <div class="flex flex-wrap items-end gap-3 mt-4">
        <WxButton variant="secondary" size="sm">Small</WxButton>
        <WxButton variant="secondary" size="md">Medium</WxButton>
        <WxButton variant="secondary" size="lg">Large</WxButton>
      </div>
    </WxCard>

    <!-- States -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">States</h3>
      </template>
      <div class="flex flex-wrap items-center gap-3">
        <WxButton variant="primary">Normal</WxButton>
        <WxButton variant="primary" :loading="loadingBtn" @click="simulateLoading">
          {{ loadingBtn ? 'Saving...' : 'Click to Load' }}
        </WxButton>
        <WxButton variant="primary" loading>Loading</WxButton>
        <WxButton variant="primary" disabled>Disabled</WxButton>
        <WxButton variant="secondary" disabled>Disabled</WxButton>
        <WxButton variant="danger" disabled>Disabled</WxButton>
      </div>
    </WxCard>

    <!-- Icon Buttons -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Icon Buttons</h3>
      </template>
      <div class="space-y-4">
        <p class="text-xs font-medium text-on-surface-variant">With leading icon</p>
        <div class="flex flex-wrap items-center gap-3">
          <WxButton variant="primary">
            <template #icon><Plus :size="16" /></template>
            Create New
          </WxButton>
          <WxButton variant="secondary">
            <template #icon><Download :size="16" /></template>
            Download
          </WxButton>
          <WxButton variant="danger">
            <template #icon><Trash2 :size="16" /></template>
            Delete
          </WxButton>
          <WxButton variant="success">
            <template #icon><Send :size="16" /></template>
            Send
          </WxButton>
          <WxButton variant="ghost">
            <template #icon><Settings :size="16" /></template>
            Settings
          </WxButton>
        </div>
        <p class="text-xs font-medium text-on-surface-variant">Icon-only buttons</p>
        <div class="flex flex-wrap items-center gap-2">
          <WxButton variant="secondary" size="sm"><Edit :size="14" /></WxButton>
          <WxButton variant="secondary" size="sm"><Save :size="14" /></WxButton>
          <WxButton variant="secondary" size="sm"><Share2 :size="14" /></WxButton>
          <WxButton variant="secondary" size="sm"><Heart :size="14" /></WxButton>
          <WxButton variant="secondary" size="sm"><Search :size="14" /></WxButton>
          <WxButton variant="secondary" size="sm"><Mail :size="14" /></WxButton>
          <WxButton variant="secondary" size="sm"><Printer :size="14" /></WxButton>
          <WxButton variant="secondary" size="sm"><RefreshCw :size="14" /></WxButton>
        </div>
      </div>
    </WxCard>

    <!-- Button Groups -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Button Groups</h3>
      </template>
      <div class="space-y-4">
        <div class="inline-flex rounded-lg overflow-hidden border border-outline-variant">
          <button class="px-4 py-2 text-sm font-medium bg-primary text-white">
            <Bold :size="14" />
          </button>
          <button class="px-4 py-2 text-sm font-medium bg-surface-container text-on-surface hover:bg-surface-container-high border-l border-outline-variant">
            <Italic :size="14" />
          </button>
          <button class="px-4 py-2 text-sm font-medium bg-surface-container text-on-surface hover:bg-surface-container-high border-l border-outline-variant">
            <Underline :size="14" />
          </button>
        </div>
        <div class="inline-flex rounded-lg overflow-hidden border border-outline-variant ml-4">
          <button class="px-4 py-2 text-sm font-medium bg-surface-container text-on-surface hover:bg-surface-container-high">
            <AlignLeft :size="14" />
          </button>
          <button class="px-4 py-2 text-sm font-medium bg-primary text-white border-l border-outline-variant">
            <AlignCenter :size="14" />
          </button>
          <button class="px-4 py-2 text-sm font-medium bg-surface-container text-on-surface hover:bg-surface-container-high border-l border-outline-variant">
            <AlignRight :size="14" />
          </button>
        </div>
        <div class="block">
          <div class="inline-flex rounded-lg overflow-hidden border border-outline-variant">
            <button class="px-4 py-2 text-sm font-medium bg-surface-container text-on-surface hover:bg-surface-container-high">Day</button>
            <button class="px-4 py-2 text-sm font-medium bg-primary text-white border-l border-outline-variant">Week</button>
            <button class="px-4 py-2 text-sm font-medium bg-surface-container text-on-surface hover:bg-surface-container-high border-l border-outline-variant">Month</button>
            <button class="px-4 py-2 text-sm font-medium bg-surface-container text-on-surface hover:bg-surface-container-high border-l border-outline-variant">Year</button>
          </div>
        </div>
      </div>
    </WxCard>

    <!-- Dropdown Buttons -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Dropdown Buttons</h3>
      </template>
      <div class="flex flex-wrap items-start gap-4">
        <WxDropdown>
          <template #trigger>
            <WxButton variant="primary">
              Actions <ChevronDown :size="14" class="ml-1" />
            </WxButton>
          </template>
          <template #default="{ close }">
            <button class="w-full px-4 py-2 text-sm text-left text-on-surface hover:bg-surface-container-high flex items-center gap-2" @click="close()">
              <Edit :size="14" /> Edit
            </button>
            <button class="w-full px-4 py-2 text-sm text-left text-on-surface hover:bg-surface-container-high flex items-center gap-2" @click="close()">
              <Download :size="14" /> Export
            </button>
            <button class="w-full px-4 py-2 text-sm text-left text-on-surface hover:bg-surface-container-high flex items-center gap-2" @click="close()">
              <Share2 :size="14" /> Share
            </button>
            <hr class="border-outline-variant my-1" />
            <button class="w-full px-4 py-2 text-sm text-left text-danger hover:bg-danger/5 flex items-center gap-2" @click="close()">
              <Trash2 :size="14" /> Delete
            </button>
          </template>
        </WxDropdown>

        <WxDropdown>
          <template #trigger>
            <WxButton variant="secondary">
              Options <ChevronDown :size="14" class="ml-1" />
            </WxButton>
          </template>
          <template #default="{ close }">
            <button class="w-full px-4 py-2 text-sm text-left text-on-surface hover:bg-surface-container-high" @click="close()">Profile</button>
            <button class="w-full px-4 py-2 text-sm text-left text-on-surface hover:bg-surface-container-high" @click="close()">Settings</button>
            <button class="w-full px-4 py-2 text-sm text-left text-on-surface hover:bg-surface-container-high" @click="close()">Logout</button>
          </template>
        </WxDropdown>
      </div>
    </WxCard>

    <!-- Block Buttons -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Block (Full-Width) Buttons</h3>
      </template>
      <div class="space-y-3 max-w-md">
        <WxButton variant="primary" class="!w-full">
          <template #icon><Send :size="16" /></template>
          Submit Application
        </WxButton>
        <WxButton variant="secondary" class="!w-full">
          <template #icon><Save :size="16" /></template>
          Save Draft
        </WxButton>
        <WxButton variant="ghost" class="!w-full">Cancel</WxButton>
      </div>
    </WxCard>
  </div>
</template>
