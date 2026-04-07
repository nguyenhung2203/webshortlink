<script setup lang="ts">
import { ref, computed } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxBreadcrumb from '@/components/ui/WxBreadcrumb.vue'
import WxFilterBar from '@/components/ui/WxFilterBar.vue'
import WxDataTable from '@/components/ui/WxDataTable.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxPagination from '@/components/ui/WxPagination.vue'
import WxStatCard from '@/components/ui/WxStatCard.vue'
import WxAlert from '@/components/ui/WxAlert.vue'
import WxSwitch from '@/components/ui/WxSwitch.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxSelect from '@/components/ui/WxSelect.vue'
import WxCheckbox from '@/components/ui/WxCheckbox.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxSpinner from '@/components/ui/WxSpinner.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'
import WxProgressBar from '@/components/ui/WxProgressBar.vue'
import type { TableColumn, BreadcrumbItem } from '@/types'
import {
  Layout, Plus, FileText, Users, Activity, TrendingUp,
  DollarSign, BarChart3, RefreshCw, Filter,
  Briefcase, ClipboardList, Loader2, Download,
} from 'lucide-vue-next'

// ── Breadcrumb ──
const breadcrumbs: BreadcrumbItem[] = [
  { label: 'Dashboard', to: '/' },
  { label: 'Tables' },
]

// ── Campaign Table ──
const campaignColumns: TableColumn[] = [
  { key: 'id', label: 'ID', width: '60px', sortable: true },
  { key: 'name', label: 'Name', sortable: true },
  { key: 'status', label: 'Status', width: '130px' },
  { key: 'spend', label: 'Spend', width: '130px', sortable: true },
  { key: 'updated', label: 'Updated', width: '140px', sortable: true },
]

const campaignData = ref([
  { id: 1, name: 'TKQC_Campaign_A', status: 'ACTIVE', spend: '$1,234.56', updated: '22/03/2026' },
  { id: 2, name: 'TKQC_Campaign_B', status: 'PAUSED', spend: '$567.89', updated: '21/03/2026' },
  { id: 3, name: 'TKQC_Retarget_01', status: 'ACTIVE', spend: '$2,345.00', updated: '22/03/2026' },
  { id: 4, name: 'TKQC_Lookalike_VN', status: 'ERROR', spend: '$0.00', updated: '20/03/2026' },
  { id: 5, name: 'TKQC_Brand_Winter', status: 'ACTIVE', spend: '$890.12', updated: '22/03/2026' },
  { id: 6, name: 'TKQC_DPA_Catalog', status: 'COMPLETED', spend: '$3,456.78', updated: '19/03/2026' },
])

const statusVariant: Record<string, 'success' | 'warning' | 'error' | 'info'> = {
  ACTIVE: 'success', PAUSED: 'warning', ERROR: 'error', COMPLETED: 'info',
}

const campaignSortKey = ref('')
const campaignSortDir = ref<'asc' | 'desc'>('asc')
const campaignPage = ref(1)

// ── Business Manager Table ──
const bmColumns: TableColumn[] = [
  { key: 'select', label: '', width: '48px' },
  { key: 'bmId', label: 'BM ID', width: '130px', sortable: true },
  { key: 'name', label: 'Business Name', sortable: true },
  { key: 'accounts', label: 'Ad Accounts', width: '120px', sortable: true },
  { key: 'status', label: 'Status', width: '130px' },
  { key: 'limit', label: 'Spend Limit', width: '130px', sortable: true },
]

const bmData = ref([
  { bmId: 'BM-001-VN', name: 'Wemake Official VN', accounts: '12 accounts', status: 'VERIFIED', limit: '$50,000' },
  { bmId: 'BM-002-VN', name: 'Wemake Ads Agency', accounts: '8 accounts', status: 'VERIFIED', limit: '$25,000' },
  { bmId: 'BM-003-US', name: 'Wemake Global', accounts: '5 accounts', status: 'RESTRICTED', limit: '$10,000' },
])

const bmStatusVariant: Record<string, 'success' | 'warning' | 'error' | 'info'> = {
  VERIFIED: 'success', RESTRICTED: 'error', PENDING: 'warning',
}

const bmSortKey = ref('')
const bmSortDir = ref<'asc' | 'desc'>('asc')
const bmSelected = ref<Record<string, boolean>>({})
const bmSelectAll = ref(false)
const bmStatusFilter = ref<string | number | null>(null)

const bmStatusOptions = [
  { label: 'All Statuses', value: 'all' },
  { label: 'Verified', value: 'VERIFIED' },
  { label: 'Restricted', value: 'RESTRICTED' },
  { label: 'Pending', value: 'PENDING' },
]

function toggleBmRow(bmId: string) {
  bmSelected.value[bmId] = !bmSelected.value[bmId]
  bmSelectAll.value = filteredBmData.value.every((row) => bmSelected.value[row.bmId])
}

const filteredBmData = computed(() => {
  if (!bmStatusFilter.value || bmStatusFilter.value === 'all') return bmData.value
  return bmData.value.filter((row) => row.status === bmStatusFilter.value)
})

const bmSelectedCount = computed(() => Object.values(bmSelected.value).filter(Boolean).length)

// ── Stat Cards ──
const stats = [
  { title: 'Total Spend', value: '$12,450', trend: 12.5, trendUp: true, icon: DollarSign },
  { title: 'Active Campaigns', value: '24', trend: 8, trendUp: true, icon: Activity },
  { title: 'Conversion Rate', value: '3.2%', trend: -2.1, trendUp: false, icon: BarChart3 },
  { title: 'ROI', value: '245%', trend: 18, trendUp: true, icon: TrendingUp },
]

// ── Alerts ──
const alertsVisible = ref({ info: true, success: true, warning: true, error: true })

// ── Settings Toggles ──
const settings = ref({
  autoDownload: true,
  showNotifications: true,
  backgroundSync: false,
  loginProtection: true,
})

const settingsConfig = [
  { key: 'autoDownload' as const, label: 'Auto Download', desc: 'Download reports automatically when ready' },
  { key: 'showNotifications' as const, label: 'Notifications', desc: 'Receive campaign status notifications' },
  { key: 'backgroundSync' as const, label: 'Background Sync', desc: 'Sync data automatically when online' },
  { key: 'loginProtection' as const, label: 'Login Protection', desc: 'Require two-factor authentication' },
]

// ── Filter Bar ──
const filters = ref(['Status: Active', 'Region: VN', 'Budget: >$500', 'Platform: FB'])

function removeFilter(index: number) { filters.value.splice(index, 1) }
function clearFilters() { filters.value = [] }
function resetFilters() { filters.value = ['Status: Active', 'Region: VN', 'Budget: >$500', 'Platform: FB'] }

// ── Activity Log Table ──
const activityLogColumns: TableColumn[] = [
  { key: 'time', label: 'Time', width: '100px' },
  { key: 'user', label: 'User', sortable: true },
  { key: 'action', label: 'Action', width: '160px' },
  { key: 'object', label: 'Object', sortable: true },
  { key: 'ip', label: 'IP', width: '140px' },
]

const activityLogData = ref([
  { time: '08:45:12', user: 'admin@wemake.vn', action: 'Login', object: 'Dashboard', ip: '192.168.1.101' },
  { time: '08:47:30', user: 'admin@wemake.vn', action: 'Create Campaign', object: 'Campaign_Spring_2026', ip: '192.168.1.101' },
  { time: '09:12:05', user: 'marketing@wemake.vn', action: 'Edit Ad', object: 'TKQC_Campaign_A', ip: '10.0.0.55' },
  { time: '09:30:18', user: 'support@wemake.vn', action: 'Export Report', object: 'Report_March_2026.xlsx', ip: '10.0.0.88' },
  { time: '10:05:42', user: 'dev@wemake.vn', action: 'Delete Account', object: 'TKQC_Test_Old', ip: '172.16.0.12' },
])

const actionBadgeVariant: Record<string, 'success' | 'warning' | 'error' | 'info'> = {
  'Login': 'info', 'Create Campaign': 'success', 'Edit Ad': 'warning',
  'Export Report': 'info', 'Delete Account': 'error',
}

const activitySortKey = ref('')
const activitySortDir = ref<'asc' | 'desc'>('asc')

// ── Member Table ──
const memberColumns: TableColumn[] = [
  { key: 'name', label: 'Member', sortable: true },
  { key: 'email', label: 'Email' },
  { key: 'role', label: 'Role', width: '120px' },
  { key: 'lastActive', label: 'Last Active', width: '160px', sortable: true },
  { key: 'status', label: 'Status', width: '100px' },
]

const memberData = ref([
  { name: 'Nguyen Van Admin', email: 'admin@wemake.vn', role: 'Admin', lastActive: '23/03/2026 08:30', status: 'online' },
  { name: 'Tran Thi Marketing', email: 'marketing@wemake.vn', role: 'Editor', lastActive: '23/03/2026 09:15', status: 'online' },
  { name: 'Le Minh Dev', email: 'dev@wemake.vn', role: 'Developer', lastActive: '22/03/2026 18:00', status: 'offline' },
  { name: 'Pham Hong Support', email: 'support@wemake.vn', role: 'Viewer', lastActive: '23/03/2026 07:45', status: 'online' },
  { name: 'Vo Duc Manager', email: 'manager@wemake.vn', role: 'Admin', lastActive: '21/03/2026 16:20', status: 'offline' },
])

const memberSortKey = ref('')
const memberSortDir = ref<'asc' | 'desc'>('asc')

const roleBadgeVariant: Record<string, 'success' | 'warning' | 'error' | 'info'> = {
  Admin: 'error', Editor: 'warning', Developer: 'info', Viewer: 'success',
}

// ── Loading/Empty State ──
const demoState = ref<'loading' | 'empty' | 'loaded'>('loaded')
const demoItems = ['Campaign Alpha', 'Campaign Beta', 'Campaign Gamma', 'Campaign Delta']
</script>

<template>
  <div class="space-y-10">
    <WxPageHeader
      title="Advanced Tables"
      description="Data tables with sorting, filtering, selection, stat cards, alerts, settings, and state management patterns."
    >
      <template #breadcrumb>
        <WxBreadcrumb :items="breadcrumbs" />
      </template>
      <template #actions>
        <WxButton variant="secondary" size="sm">
          <template #icon><RefreshCw :size="14" /></template>
          Refresh
        </WxButton>
        <WxButton variant="secondary" size="sm">
          <template #icon><Download :size="14" /></template>
          Export
        </WxButton>
        <WxButton variant="primary" size="sm">
          <template #icon><Plus :size="14" /></template>
          Create
        </WxButton>
      </template>
    </WxPageHeader>

    <!-- Section 1 - Campaign Data Table -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Layout class="w-6 h-6" />
        <span>Campaign Data Table</span>
      </div>
      <WxDataTable
        :columns="campaignColumns"
        :data="campaignData"
        v-model:sort-key="campaignSortKey"
        v-model:sort-direction="campaignSortDir"
      >
        <template #cell-name="{ value }">
          <span class="font-medium">{{ value }}</span>
        </template>
        <template #cell-status="{ value }">
          <WxBadge :variant="statusVariant[value as string]" size="sm">{{ value }}</WxBadge>
        </template>
        <template #cell-spend="{ value }">
          <span class="tabular-nums font-medium">{{ value }}</span>
        </template>
        <template #cell-updated="{ value }">
          <span class="text-on-surface-variant">{{ value }}</span>
        </template>
      </WxDataTable>
      <WxPagination :current-page="campaignPage" :total-pages="1" :total-items="6" :page-size="10" @update:current-page="campaignPage = $event" />
    </section>

    <!-- Section 2 - Business Manager Table -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Briefcase class="w-6 h-6" />
        <span>Business Manager Table</span>
      </div>
      <div class="flex items-center justify-between gap-4 flex-wrap">
        <div class="flex items-center gap-3">
          <div class="w-48">
            <WxSelect v-model="bmStatusFilter" :options="bmStatusOptions" placeholder="Filter by status" size="sm" />
          </div>
          <span v-if="bmSelectedCount > 0" class="text-sm text-on-surface-variant">
            {{ bmSelectedCount }} row{{ bmSelectedCount > 1 ? 's' : '' }} selected
          </span>
        </div>
        <WxButton v-if="bmSelectedCount > 0" variant="danger" size="sm">Delete Selected</WxButton>
      </div>
      <WxDataTable :columns="bmColumns" :data="filteredBmData" v-model:sort-key="bmSortKey" v-model:sort-direction="bmSortDir">
        <template #cell-select="{ row }">
          <WxCheckbox
            :model-value="!!bmSelected[(row as Record<string, unknown>).bmId as string]"
            @update:model-value="toggleBmRow((row as Record<string, unknown>).bmId as string)"
          />
        </template>
        <template #cell-name="{ value }"><span class="font-medium">{{ value }}</span></template>
        <template #cell-status="{ value }">
          <WxBadge :variant="bmStatusVariant[value as string]" size="sm">{{ value }}</WxBadge>
        </template>
        <template #cell-limit="{ value }"><span class="tabular-nums font-medium">{{ value }}</span></template>
      </WxDataTable>
    </section>

    <!-- Section 3 - Stat Cards -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <BarChart3 class="w-6 h-6" />
        <span>Stat Cards</span>
      </div>
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <WxStatCard v-for="stat in stats" :key="stat.title" :title="stat.title" :value="stat.value" :trend="stat.trend" :trend-up="stat.trendUp">
          <template #icon><component :is="stat.icon" :size="22" /></template>
        </WxStatCard>
      </div>
    </section>

    <!-- Section 4 - Alert Variants -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Activity class="w-6 h-6" />
        <span>Alert Variants</span>
      </div>
      <div class="space-y-3">
        <WxAlert v-if="alertsVisible.info" type="info" title="Information" closable @close="alertsVisible.info = false">
          System maintenance scheduled for 30/03/2026 from 02:00 - 04:00 (GMT+7).
        </WxAlert>
        <WxAlert v-if="alertsVisible.success" type="success" title="Success" closable @close="alertsVisible.success = false">
          Campaign TKQC_Campaign_A has been activated successfully.
        </WxAlert>
        <WxAlert v-if="alertsVisible.warning" type="warning" title="Warning" closable @close="alertsVisible.warning = false">
          BM-003-US budget is approaching the spending limit. Please review.
        </WxAlert>
        <WxAlert v-if="alertsVisible.error" type="error" title="Error" closable @close="alertsVisible.error = false">
          Campaign TKQC_Lookalike_VN encountered a distribution error. Check targeting settings.
        </WxAlert>
      </div>
    </section>

    <!-- Section 5 - Settings Toggles -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <FileText class="w-6 h-6" />
        <span>Settings</span>
      </div>
      <WxCard>
        <div class="divide-y divide-outline-variant">
          <div v-for="setting in settingsConfig" :key="setting.key" class="flex items-center justify-between py-4 first:pt-0 last:pb-0">
            <div class="flex-1 min-w-0 pr-4">
              <p class="text-sm font-semibold text-on-surface">{{ setting.label }}</p>
              <p class="text-xs text-on-surface-variant mt-0.5">{{ setting.desc }}</p>
            </div>
            <WxSwitch v-model="settings[setting.key]" />
          </div>
        </div>
      </WxCard>
    </section>

    <!-- Section 6 - Filter Bar -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Filter class="w-6 h-6" />
        <span>Filter Bar</span>
      </div>
      <WxCard>
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <p class="text-sm font-semibold text-on-surface">Active Filters</p>
            <WxButton variant="ghost" size="sm" @click="resetFilters">
              <template #icon><RefreshCw :size="14" /></template>
              Reset
            </WxButton>
          </div>
          <WxFilterBar :filters="filters" @remove="removeFilter" @clear="clearFilters" />
          <p v-if="filters.length === 0" class="text-sm text-on-surface-variant italic">
            No active filters. Press "Reset" to restore defaults.
          </p>
        </div>
      </WxCard>
    </section>

    <!-- Section 7 - Activity Log Table -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <ClipboardList class="w-6 h-6" />
        <span>Activity Log Table</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <WxDataTable :columns="activityLogColumns" :data="activityLogData" v-model:sort-key="activitySortKey" v-model:sort-direction="activitySortDir">
          <template #cell-time="{ value }">
            <span class="tabular-nums text-on-surface-variant font-mono text-xs">{{ value }}</span>
          </template>
          <template #cell-user="{ value }"><span class="font-medium">{{ value }}</span></template>
          <template #cell-action="{ value }">
            <WxBadge :variant="actionBadgeVariant[value as string] || 'info'" size="sm">{{ value }}</WxBadge>
          </template>
          <template #cell-object="{ value }"><span class="font-medium text-on-surface">{{ value }}</span></template>
          <template #cell-ip="{ value }">
            <span class="tabular-nums text-on-surface-variant font-mono text-xs">{{ value }}</span>
          </template>
        </WxDataTable>
      </div>
    </section>

    <!-- Section 8 - Member Table -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Users class="w-6 h-6" />
        <span>Member Table</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <WxDataTable :columns="memberColumns" :data="memberData" v-model:sort-key="memberSortKey" v-model:sort-direction="memberSortDir">
          <template #cell-name="{ value, row }">
            <div class="flex items-center gap-3">
              <WxAvatar :name="value as string" size="sm" :status="(row as Record<string, unknown>).status === 'online' ? 'online' : 'offline'" />
              <span class="font-medium">{{ value }}</span>
            </div>
          </template>
          <template #cell-email="{ value }"><span class="text-on-surface-variant text-sm">{{ value }}</span></template>
          <template #cell-role="{ value }">
            <WxBadge :variant="roleBadgeVariant[value as string] || 'info'" size="sm">{{ value }}</WxBadge>
          </template>
          <template #cell-lastActive="{ value }"><span class="text-on-surface-variant text-sm">{{ value }}</span></template>
          <template #cell-status="{ value }">
            <div class="flex items-center gap-2">
              <span class="w-2 h-2 rounded-full" :class="value === 'online' ? 'bg-success' : 'bg-on-surface-variant/30'" />
              <span class="text-sm capitalize">{{ value }}</span>
            </div>
          </template>
        </WxDataTable>
      </div>
    </section>

    <!-- Section 9 - Loading/Empty State -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Loader2 class="w-6 h-6" />
        <span>Loading / Empty State Demo</span>
      </div>
      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-6">
        <div class="flex items-center gap-3">
          <WxButton :variant="demoState === 'loading' ? 'primary' : 'secondary'" size="sm" @click="demoState = 'loading'">Loading</WxButton>
          <WxButton :variant="demoState === 'empty' ? 'primary' : 'secondary'" size="sm" @click="demoState = 'empty'">Empty</WxButton>
          <WxButton :variant="demoState === 'loaded' ? 'primary' : 'secondary'" size="sm" @click="demoState = 'loaded'">Loaded</WxButton>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="border border-outline-variant rounded-xl p-6 min-h-50 flex items-center justify-center">
            <div v-if="demoState === 'loading'" class="flex flex-col items-center gap-3">
              <WxSpinner size="lg" />
              <span class="text-sm text-on-surface-variant">Loading data...</span>
            </div>
            <WxEmptyState v-else-if="demoState === 'empty'" title="No data" description="No campaigns have been created yet.">
              <template #action>
                <WxButton variant="primary" size="sm">
                  <template #icon><Plus :size="14" /></template>
                  Create Campaign
                </WxButton>
              </template>
            </WxEmptyState>
            <div v-else class="w-full space-y-2">
              <div v-for="item in demoItems" :key="item" class="flex items-center justify-between p-3 rounded-lg bg-surface-container hover:bg-surface-container-high transition-colors">
                <span class="font-medium text-sm">{{ item }}</span>
                <WxBadge variant="success" size="sm">Active</WxBadge>
              </div>
            </div>
          </div>
          <div class="border border-outline-variant rounded-xl p-6 min-h-50 flex items-center justify-center">
            <div v-if="demoState === 'loading'" class="flex flex-col items-center gap-3">
              <WxSpinner size="lg" />
              <span class="text-sm text-on-surface-variant">Syncing...</span>
            </div>
            <WxEmptyState v-else-if="demoState === 'empty'" title="No results" description="Try adjusting your filters." />
            <div v-else class="w-full space-y-3">
              <div class="flex items-center justify-between">
                <span class="text-sm font-semibold">Total Spend</span>
                <span class="text-lg font-bold text-primary">$4,567.89</span>
              </div>
              <WxProgressBar :value="68" variant="primary" size="md" show-value />
              <p class="text-xs text-on-surface-variant">68% of budget used</p>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>
