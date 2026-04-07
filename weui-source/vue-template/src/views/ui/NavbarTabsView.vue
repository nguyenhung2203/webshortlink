<script setup lang="ts">
import { ref } from 'vue'
import {
  Home, User, Settings, Bell, BarChart3, FileText,
  Mail, Shield, Palette, Database,
} from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxTabs from '@/components/ui/WxTabs.vue'
import WxBadge from '@/components/ui/WxBadge.vue'

// Underline tabs
const underlineTab = ref<string | number>('overview')
const underlineTabs = [
  { key: 'overview', label: 'Overview' },
  { key: 'analytics', label: 'Analytics' },
  { key: 'reports', label: 'Reports' },
  { key: 'settings', label: 'Settings' },
]

// Pills tabs
const pillsTab = ref<string | number>('all')

// Vertical tabs
const verticalTab = ref('profile')

// Icon tabs
const iconTab = ref('dashboard')

// Navbar
const activeNav = ref('home')
const navItems = [
  { key: 'home', label: 'Home', icon: Home },
  { key: 'products', label: 'Products', icon: BarChart3 },
  { key: 'orders', label: 'Orders', icon: FileText },
  { key: 'customers', label: 'Customers', icon: User },
  { key: 'settings', label: 'Settings', icon: Settings },
]
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Navbar & Tabs"
      description="Tab navigation patterns including underline, pills, vertical, icon tabs, and custom navbars."
    />

    <!-- Underline Tabs -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Underline Tabs</h3>
      </template>
      <WxTabs v-model="underlineTab" :items="underlineTabs">
        <template #default="{ activeKey }">
          <div v-if="activeKey === 'overview'" class="text-sm text-on-surface-variant">
            <p>The overview tab provides a high-level summary of your project metrics, including active users, conversion rates, and revenue trends over the last 30 days.</p>
          </div>
          <div v-if="activeKey === 'analytics'" class="text-sm text-on-surface-variant">
            <p>Deep dive into analytics with customizable date ranges, segment filters, and exportable charts. Track user behavior across funnels.</p>
          </div>
          <div v-if="activeKey === 'reports'" class="text-sm text-on-surface-variant">
            <p>Generate and schedule automated reports. Export to PDF, CSV, or share via email with your team members.</p>
          </div>
          <div v-if="activeKey === 'settings'" class="text-sm text-on-surface-variant">
            <p>Configure dashboard preferences, notification thresholds, data retention policies, and integration settings.</p>
          </div>
        </template>
      </WxTabs>
    </WxCard>

    <!-- Pills Tabs -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Pills Tabs</h3>
      </template>
      <div class="flex gap-1 p-1 bg-surface-container rounded-lg w-fit mb-4">
        <button
          v-for="item in [{ key: 'all', label: 'All' }, { key: 'active', label: 'Active' }, { key: 'inactive', label: 'Inactive' }, { key: 'archived', label: 'Archived' }]"
          :key="item.key"
          class="px-4 py-1.5 text-sm font-medium rounded-md transition-all"
          :class="pillsTab === item.key
            ? 'bg-primary text-white shadow-sm'
            : 'text-on-surface-variant hover:text-on-surface'"
          @click="pillsTab = item.key"
        >
          {{ item.label }}
        </button>
      </div>
      <div class="text-sm text-on-surface-variant">
        <p v-if="pillsTab === 'all'">Showing all 248 items across every status category.</p>
        <p v-if="pillsTab === 'active'">142 active items currently in use and being monitored.</p>
        <p v-if="pillsTab === 'inactive'">86 inactive items that have been paused or disabled.</p>
        <p v-if="pillsTab === 'archived'">20 archived items stored for historical reference.</p>
      </div>
    </WxCard>

    <!-- Vertical Tabs -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Vertical Tabs</h3>
      </template>
      <div class="flex gap-6 min-h-[200px]">
        <div class="w-48 shrink-0 border-r border-outline-variant pr-4 space-y-1">
          <button
            v-for="item in [
              { key: 'profile', label: 'Profile', icon: User },
              { key: 'security', label: 'Security', icon: Shield },
              { key: 'appearance', label: 'Appearance', icon: Palette },
              { key: 'notifications', label: 'Notifications', icon: Bell },
              { key: 'data', label: 'Data & Storage', icon: Database },
            ]"
            :key="item.key"
            class="w-full flex items-center gap-2 px-3 py-2 text-sm font-medium rounded-lg transition-colors text-left"
            :class="verticalTab === item.key
              ? 'bg-primary/10 text-primary'
              : 'text-on-surface-variant hover:text-on-surface hover:bg-surface-container'"
            @click="verticalTab = item.key"
          >
            <component :is="item.icon" :size="16" />
            {{ item.label }}
          </button>
        </div>
        <div class="flex-1">
          <div v-if="verticalTab === 'profile'" class="space-y-2">
            <h4 class="text-sm font-semibold text-on-surface">Profile Settings</h4>
            <p class="text-sm text-on-surface-variant">Manage your personal information, avatar, display name, and bio. Changes are reflected across all connected services.</p>
          </div>
          <div v-if="verticalTab === 'security'" class="space-y-2">
            <h4 class="text-sm font-semibold text-on-surface">Security Settings</h4>
            <p class="text-sm text-on-surface-variant">Configure two-factor authentication, manage active sessions, and review recent login activity for your account.</p>
          </div>
          <div v-if="verticalTab === 'appearance'" class="space-y-2">
            <h4 class="text-sm font-semibold text-on-surface">Appearance</h4>
            <p class="text-sm text-on-surface-variant">Customize your theme, color scheme, font size, and density preferences for a personalized experience.</p>
          </div>
          <div v-if="verticalTab === 'notifications'" class="space-y-2">
            <h4 class="text-sm font-semibold text-on-surface">Notification Preferences</h4>
            <p class="text-sm text-on-surface-variant">Choose which notifications you receive via email, push, or in-app. Set quiet hours and priority levels.</p>
          </div>
          <div v-if="verticalTab === 'data'" class="space-y-2">
            <h4 class="text-sm font-semibold text-on-surface">Data & Storage</h4>
            <p class="text-sm text-on-surface-variant">Review storage usage, manage cached data, export your data, or request account deletion.</p>
          </div>
        </div>
      </div>
    </WxCard>

    <!-- Tabs with Icons and Badges -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Tabs with Icons &amp; Badges</h3>
      </template>
      <div class="flex border-b border-outline-variant mb-4">
        <button
          v-for="item in [
            { key: 'dashboard', label: 'Dashboard', icon: Home, badge: null },
            { key: 'messages', label: 'Messages', icon: Mail, badge: 5 },
            { key: 'alerts', label: 'Alerts', icon: Bell, badge: 12 },
            { key: 'settings', label: 'Settings', icon: Settings, badge: null },
          ]"
          :key="item.key"
          class="relative flex items-center gap-2 px-4 py-2.5 text-sm font-medium transition-colors"
          :class="iconTab === item.key
            ? 'text-primary'
            : 'text-on-surface-variant hover:text-on-surface'"
          @click="iconTab = item.key"
        >
          <component :is="item.icon" :size="16" />
          {{ item.label }}
          <WxBadge v-if="item.badge" variant="error" size="sm">{{ item.badge }}</WxBadge>
          <span v-if="iconTab === item.key" class="absolute bottom-0 left-0 right-0 h-0.5 bg-primary rounded-t" />
        </button>
      </div>
      <div class="text-sm text-on-surface-variant">
        <p v-if="iconTab === 'dashboard'">Welcome back. Your dashboard shows 3 active projects, 14 pending tasks, and 2 upcoming deadlines this week.</p>
        <p v-if="iconTab === 'messages'">You have 5 unread messages from team members. The latest is from Sarah regarding the Q2 roadmap review.</p>
        <p v-if="iconTab === 'alerts'">12 new alerts detected: 8 info-level, 3 warnings, and 1 critical alert requiring immediate attention.</p>
        <p v-if="iconTab === 'settings'">Access all application settings including integrations, API keys, and team management options.</p>
      </div>
    </WxCard>

    <!-- Custom Navbar -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Custom Navbar</h3>
      </template>
      <div class="bg-surface-container rounded-lg px-2 py-1.5 flex items-center gap-1 overflow-x-auto">
        <button
          v-for="item in navItems"
          :key="item.key"
          class="flex items-center gap-2 px-4 py-2 text-sm font-medium rounded-lg whitespace-nowrap transition-all"
          :class="activeNav === item.key
            ? 'bg-primary text-white shadow-sm'
            : 'text-on-surface-variant hover:text-on-surface hover:bg-surface-container-high'"
          @click="activeNav = item.key"
        >
          <component :is="item.icon" :size="16" />
          {{ item.label }}
        </button>
      </div>
      <div class="mt-4 p-4 bg-surface-container rounded-lg">
        <p class="text-sm text-on-surface-variant">
          Currently viewing: <span class="font-semibold text-on-surface">{{ navItems.find(n => n.key === activeNav)?.label }}</span>
        </p>
      </div>
    </WxCard>
  </div>
</template>
