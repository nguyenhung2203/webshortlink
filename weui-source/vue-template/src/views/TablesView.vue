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
import WxTag from '@/components/ui/WxTag.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxSpinner from '@/components/ui/WxSpinner.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxInputNumber from '@/components/ui/WxInputNumber.vue'
import WxDatePicker from '@/components/ui/WxDatePicker.vue'
import WxMultiSelect from '@/components/ui/WxMultiSelect.vue'
import WxTextarea from '@/components/ui/WxTextarea.vue'
import WxProgressBar from '@/components/ui/WxProgressBar.vue'
import WxRadio from '@/components/ui/WxRadio.vue'
import type { TableColumn, BreadcrumbItem } from '@/types'
import {
  Layout,
  Plus,
  FileText,
  Users,
  Activity,
  TrendingUp,
  DollarSign,
  BarChart3,
  RefreshCw,
  MoreHorizontal,
  Filter,
  Globe,
  Download,
  Briefcase,
  ChevronRight,
  ClipboardList,
  Loader2,
  ChevronDown,
  Share2,
  Bell,
  LayoutGrid,
  PenTool,
  Shield,
  Eye,
  Server,
  Monitor,
  AlignCenter,
} from 'lucide-vue-next'

// ── Breadcrumb ──
const breadcrumbs: BreadcrumbItem[] = [
  { label: 'H\u1EC7 th\u1ED1ng', to: '/' },
  { label: 'Patterns' },
]

// ── Campaign Table (Section 1) ──
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
  ACTIVE: 'success',
  PAUSED: 'warning',
  ERROR: 'error',
  COMPLETED: 'info',
}

const campaignSortKey = ref('')
const campaignSortDir = ref<'asc' | 'desc'>('asc')
const campaignPage = ref(1)

// ── Business Manager Table (Section 2) ──
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
  VERIFIED: 'success',
  RESTRICTED: 'error',
  PENDING: 'warning',
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

function toggleBmSelectAll() {
  bmSelectAll.value = !bmSelectAll.value
  filteredBmData.value.forEach((row) => {
    bmSelected.value[row.bmId] = bmSelectAll.value
  })
}

function toggleBmRow(bmId: string) {
  bmSelected.value[bmId] = !bmSelected.value[bmId]
  bmSelectAll.value = filteredBmData.value.every((row) => bmSelected.value[row.bmId])
}

const filteredBmData = computed(() => {
  if (!bmStatusFilter.value || bmStatusFilter.value === 'all') return bmData.value
  return bmData.value.filter((row) => row.status === bmStatusFilter.value)
})

const bmSelectedCount = computed(() => {
  return Object.values(bmSelected.value).filter(Boolean).length
})

// ── Stat Cards (Section 3) ──
const stats = [
  { title: 'Total Spend', value: '$12,450', trend: 12.5, trendUp: true, icon: DollarSign },
  { title: 'Active Campaigns', value: '24', trend: 8, trendUp: true, icon: Activity },
  { title: 'Conversion Rate', value: '3.2%', trend: -2.1, trendUp: false, icon: BarChart3 },
  { title: 'ROI', value: '245%', trend: 18, trendUp: true, icon: TrendingUp },
]

// ── Alerts (Section 4) ──
const alertsVisible = ref({ info: true, success: true, warning: true, error: true })

// ── Settings Toggles (Section 5) ──
const settings = ref({
  autoDownload: true,
  showNotifications: true,
  backgroundSync: false,
  loginProtection: true,
})

const settingsConfig = [
  {
    key: 'autoDownload' as const,
    label: 'T\u1EA3i xu\u1ED1ng t\u1EF1 \u0111\u1ED9ng',
    desc: 'T\u1EF1 \u0111\u1ED9ng t\u1EA3i b\u00E1o c\u00E1o khi ho\u00E0n th\u00E0nh',
  },
  {
    key: 'showNotifications' as const,
    label: 'Hi\u1EC3n th\u1ECB th\u00F4ng b\u00E1o',
    desc: 'Nh\u1EADn th\u00F4ng b\u00E1o v\u1EC1 tr\u1EA1ng th\u00E1i chi\u1EBFn d\u1ECBch',
  },
  {
    key: 'backgroundSync' as const,
    label: '\u0110\u1ED3ng b\u1ED9 n\u1EC1n',
    desc: 'T\u1EF1 \u0111\u1ED9ng \u0111\u1ED3ng b\u1ED9 d\u1EEF li\u1EC7u khi c\u00F3 k\u1EBFt n\u1ED1i m\u1EA1ng',
  },
  {
    key: 'loginProtection' as const,
    label: 'B\u1EA3o v\u1EC7 \u0111\u0103ng nh\u1EADp',
    desc: 'Y\u00EAu c\u1EA7u x\u00E1c th\u1EF1c hai y\u1EBFu t\u1ED1 khi \u0111\u0103ng nh\u1EADp',
  },
]

// ── Filter Bar (Section 6) ──
const filters = ref(['Status: Active', 'Region: VN', 'Budget: >$500', 'Platform: FB'])

function removeFilter(index: number) {
  filters.value.splice(index, 1)
}

function clearFilters() {
  filters.value = []
}

function resetFilters() {
  filters.value = ['Status: Active', 'Region: VN', 'Budget: >$500', 'Platform: FB']
}

// ── Section 7: Activity Log Table ──
const activityLogColumns: TableColumn[] = [
  { key: 'time', label: 'Time', width: '100px' },
  { key: 'user', label: 'User', sortable: true },
  { key: 'action', label: 'Action', width: '160px' },
  { key: 'object', label: 'Object', sortable: true },
  { key: 'ip', label: 'IP', width: '140px' },
]

const activityLogData = ref([
  { time: '08:45:12', user: 'admin@wemake.vn', action: 'Dang nhap', object: 'Dashboard', ip: '192.168.1.101' },
  { time: '08:47:30', user: 'admin@wemake.vn', action: 'Tao chien dich', object: 'Campaign_Spring_2026', ip: '192.168.1.101' },
  { time: '09:12:05', user: 'marketing@wemake.vn', action: 'Chinh sua TKQC', object: 'TKQC_Campaign_A', ip: '10.0.0.55' },
  { time: '09:30:18', user: 'support@wemake.vn', action: 'Xuat bao cao', object: 'Report_March_2026.xlsx', ip: '10.0.0.88' },
  { time: '10:05:42', user: 'dev@wemake.vn', action: 'Xoa tai khoan', object: 'TKQC_Test_Old', ip: '172.16.0.12' },
  { time: '10:15:00', user: 'admin@wemake.vn', action: 'Cap nhat proxy', object: 'Proxy Pool #3', ip: '192.168.1.101' },
  { time: '11:00:33', user: 'marketing@wemake.vn', action: 'Upload media', object: 'banner_summer.png', ip: '10.0.0.55' },
])

const actionBadgeVariant: Record<string, 'success' | 'warning' | 'error' | 'info'> = {
  'Dang nhap': 'info',
  'Tao chien dich': 'success',
  'Chinh sua TKQC': 'warning',
  'Xuat bao cao': 'info',
  'Xoa tai khoan': 'error',
  'Cap nhat proxy': 'warning',
  'Upload media': 'success',
}

const activitySortKey = ref('')
const activitySortDir = ref<'asc' | 'desc'>('asc')

// ── Section 8: Member Table ──
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
  Admin: 'error',
  Editor: 'warning',
  Developer: 'info',
  Viewer: 'success',
}

function getInitials(name: string) {
  return name.split(' ').map(w => w[0]).join('').slice(0, 2).toUpperCase()
}

// ── Section 9: Loading/Empty State Demo ──
const demoState = ref<'loading' | 'empty' | 'loaded'>('loaded')
const demoItems = ['Campaign Alpha', 'Campaign Beta', 'Campaign Gamma', 'Campaign Delta']

// ── Section 10: WxOptionCard ──
const selectedMode = ref('standard')

// ── Section 11: WxCollapsiblePanel ──
const expandedPanel = ref<number | null>(null)
const faqPanels = [
  { title: 'TKQC la gi?', content: 'TKQC (Tai khoan quang cao) la tai khoan duoc su dung de chay quang cao tren cac nen tang nhu Facebook, Google. Moi TKQC co the duoc gan vao mot Business Manager va quan ly nhieu chien dich cung luc.' },
  { title: 'Cach thiet lap proxy?', content: 'De thiet lap proxy, vao muc Cai dat > Proxy > Them moi. Nhap dia chi IP, port, username va password. He thong ho tro HTTP, HTTPS va SOCKS5. Nen su dung proxy residential de dam bao do on dinh cao nhat.' },
  { title: 'Cac phuong thuc thanh toan?', content: 'He thong ho tro thanh toan qua the Visa/Mastercard, chuyen khoan ngan hang (VCB, TCB, MB), vi dien tu (MoMo, ZaloPay) va USDT. Hoa don se duoc gui tu dong qua email sau moi giao dich.' },
]

function togglePanel(index: number) {
  expandedPanel.value = expandedPanel.value === index ? null : index
}

// ── Section 12: WxFeatureCard ──
const featureCards = [
  { title: 'Quan ly TKQC', icon: Briefcase, description: 'Quan ly tap trung tat ca tai khoan quang cao, theo doi trang thai, ngan sach va hieu suat chien dich theo thoi gian thuc.' },
  { title: 'Kiem tra BM', icon: Globe, description: 'Tu dong kiem tra tinh trang Business Manager, phat hien van de xac minh, gioi han chi tieu va canh bao som.' },
  { title: 'Chia se Pixel', icon: Share2, description: 'Chia se Pixel giua cac TKQC va BM de toi uu hoa du lieu chuyen doi va nham muc tieu quang cao hieu qua hon.' },
]

// ── Section 13: WxNotificationPatterns ──
const notifications = [
  { id: 1, avatar: 'SYS', title: 'He thong da duoc cap nhat len v2.4', badge: 'info' as const, time: '2 phut truoc', type: 'System' },
  { id: 2, avatar: 'TTB', title: 'Tran Thi B da chinh sua Campaign_A', badge: 'warning' as const, time: '15 phut truoc', type: 'User' },
  { id: 3, avatar: 'ALR', title: 'Budget Campaign_B vuot 80% gioi han', badge: 'error' as const, time: '1 gio truoc', type: 'Alert' },
]

// ── Section 14: WxCardPatterns ──
// (static data, no reactive needed)

// ── Section 15: WxFormLayout ──
const formCampaignName = ref('')
const formObjective = ref<string | number | null>(null)
const formDailyBudget = ref(500000)
const formStartDate = ref('')
const formEndDate = ref('')
const formRegions = ref<(string | number)[]>([])
const formNotes = ref('')

const objectiveOptions = [
  { label: 'Tang luot truy cap', value: 'traffic' },
  { label: 'Tang chuyen doi', value: 'conversion' },
  { label: 'Tang nhan dien thuong hieu', value: 'awareness' },
  { label: 'Tang tuong tac', value: 'engagement' },
]

const regionOptions = [
  { label: 'Ha Noi', value: 'hanoi' },
  { label: 'Ho Chi Minh', value: 'hcm' },
  { label: 'Da Nang', value: 'danang' },
  { label: 'Can Tho', value: 'cantho' },
  { label: 'Hai Phong', value: 'haiphong' },
]

// ── Section 16: WxDataVisualization ──
const barChartData = [
  { day: 'T2', value: 1200000, percent: 60 },
  { day: 'T3', value: 1800000, percent: 90 },
  { day: 'T4', value: 1500000, percent: 75 },
  { day: 'T5', value: 2000000, percent: 100 },
  { day: 'T6', value: 1700000, percent: 85 },
  { day: 'T7', value: 900000, percent: 45 },
  { day: 'CN', value: 600000, percent: 30 },
]

const lineChartPoints = [
  { day: 'T2', value: 800000, y: 70 },
  { day: 'T3', value: 1200000, y: 50 },
  { day: 'T4', value: 1000000, y: 60 },
  { day: 'T5', value: 1600000, y: 30 },
  { day: 'T6', value: 1400000, y: 40 },
  { day: 'T7', value: 700000, y: 75 },
  { day: 'CN', value: 500000, y: 85 },
]

function formatVND(value: number) {
  return new Intl.NumberFormat('vi-VN').format(value) + 'd'
}
</script>

<template>
  <div class="space-y-10">
    <!-- Page Header with Breadcrumb & Actions -->
    <WxPageHeader
      title="Patterns"
      description="T\u1ED5ng h\u1EE3p c\u00E1c m\u1EABu giao di\u1EC7n ph\u1ED5 bi\u1EBFn: b\u1EA3ng d\u1EEF li\u1EC7u, th\u1EBB th\u1ED1ng k\u00EA, c\u1EA3nh b\u00E1o, c\u00E0i \u0111\u1EB7t v\u00E0 b\u1ED9 l\u1ECDc."
    >
      <template #breadcrumb>
        <WxBreadcrumb :items="breadcrumbs" />
      </template>
      <template #actions>
        <WxButton variant="secondary" size="sm">
          <template #icon><RefreshCw :size="14" /></template>
          L\u00E0m m\u1EDBi
        </WxButton>
        <WxButton variant="secondary" size="sm">
          <template #icon><Download :size="14" /></template>
          Xu\u1EA5t file
        </WxButton>
        <WxButton variant="primary" size="sm">
          <template #icon><Plus :size="14" /></template>
          T\u1EA1o m\u1EDBi
        </WxButton>
      </template>
    </WxPageHeader>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 1 — Campaign Data Table                -->
    <!-- ═══════════════════════════════════════════════ -->
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
          <WxBadge :variant="statusVariant[value as string]" size="sm">
            {{ value }}
          </WxBadge>
        </template>
        <template #cell-spend="{ value }">
          <span class="tabular-nums font-medium">{{ value }}</span>
        </template>
        <template #cell-updated="{ value }">
          <span class="text-on-surface-variant">{{ value }}</span>
        </template>
      </WxDataTable>

      <WxPagination
        :current-page="campaignPage"
        :total-pages="1"
        :total-items="6"
        :page-size="10"
        @update:current-page="campaignPage = $event"
      />
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 2 — Business Manager Table             -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Briefcase class="w-6 h-6" />
        <span>Business Manager Table</span>
      </div>

      <!-- Toolbar: filter + selection count -->
      <div class="flex items-center justify-between gap-4 flex-wrap">
        <div class="flex items-center gap-3">
          <div class="w-48">
            <WxSelect
              v-model="bmStatusFilter"
              :options="bmStatusOptions"
              placeholder="Filter by status"
              size="sm"
            />
          </div>
          <span v-if="bmSelectedCount > 0" class="text-sm text-on-surface-variant">
            {{ bmSelectedCount }} row{{ bmSelectedCount > 1 ? 's' : '' }} selected
          </span>
        </div>
        <WxButton v-if="bmSelectedCount > 0" variant="danger" size="sm">
          Delete Selected
        </WxButton>
      </div>

      <WxDataTable
        :columns="bmColumns"
        :data="filteredBmData"
        v-model:sort-key="bmSortKey"
        v-model:sort-direction="bmSortDir"
      >
        <!-- Header checkbox via the select column -->
        <template #cell-select="{ row }">
          <WxCheckbox
            :model-value="!!bmSelected[(row as Record<string, unknown>).bmId as string]"
            @update:model-value="toggleBmRow((row as Record<string, unknown>).bmId as string)"
          />
        </template>
        <template #cell-name="{ value }">
          <span class="font-medium">{{ value }}</span>
        </template>
        <template #cell-status="{ value }">
          <WxBadge :variant="bmStatusVariant[value as string]" size="sm">
            {{ value }}
          </WxBadge>
        </template>
        <template #cell-limit="{ value }">
          <span class="tabular-nums font-medium">{{ value }}</span>
        </template>
      </WxDataTable>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 3 — Stat Cards                         -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <BarChart3 class="w-6 h-6" />
        <span>Stat Cards</span>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <WxStatCard
          v-for="stat in stats"
          :key="stat.title"
          :title="stat.title"
          :value="stat.value"
          :trend="stat.trend"
          :trend-up="stat.trendUp"
        >
          <template #icon>
            <component :is="stat.icon" :size="22" />
          </template>
        </WxStatCard>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 4 — Alert Variants                     -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Activity class="w-6 h-6" />
        <span>Alert Variants</span>
      </div>

      <div class="space-y-3">
        <WxAlert
          v-if="alertsVisible.info"
          type="info"
          title="Th\u00F4ng tin"
          closable
          @close="alertsVisible.info = false"
        >
          H\u1EC7 th\u1ED1ng s\u1EBD b\u1EA3o tr\u00EC \u0111\u1ECBnh k\u1EF3 v\u00E0o ng\u00E0y 30/03/2026 t\u1EEB 02:00 - 04:00 (GMT+7).
        </WxAlert>

        <WxAlert
          v-if="alertsVisible.success"
          type="success"
          title="Th\u00E0nh c\u00F4ng"
          closable
          @close="alertsVisible.success = false"
        >
          Chi\u1EBFn d\u1ECBch TKQC_Campaign_A \u0111\u00E3 \u0111\u01B0\u1EE3c k\u00EDch ho\u1EA1t th\u00E0nh c\u00F4ng.
        </WxAlert>

        <WxAlert
          v-if="alertsVisible.warning"
          type="warning"
          title="C\u1EA3nh b\u00E1o"
          closable
          @close="alertsVisible.warning = false"
        >
          Ng\u00E2n s\u00E1ch c\u1EE7a BM-003-US s\u1EAFp \u0111\u1EA1t gi\u1EDBi h\u1EA1n chi ti\u00EAu. Vui l\u00F2ng ki\u1EC3m tra.
        </WxAlert>

        <WxAlert
          v-if="alertsVisible.error"
          type="error"
          title="L\u1ED7i"
          closable
          @close="alertsVisible.error = false"
        >
          Chi\u1EBFn d\u1ECBch TKQC_Lookalike_VN g\u1EB7p l\u1ED7i ph\u00E2n ph\u1ED1i. Vui l\u00F2ng ki\u1EC3m tra c\u00E0i \u0111\u1EB7t \u0111\u1ED1i t\u01B0\u1EE3ng m\u1EE5c ti\u00EAu.
        </WxAlert>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 5 — Settings Toggles                   -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <FileText class="w-6 h-6" />
        <span>Settings</span>
      </div>

      <WxCard>
        <div class="divide-y divide-outline-variant">
          <div
            v-for="setting in settingsConfig"
            :key="setting.key"
            class="flex items-center justify-between py-4 first:pt-0 last:pb-0"
          >
            <div class="flex-1 min-w-0 pr-4">
              <p class="text-sm font-semibold text-on-surface">{{ setting.label }}</p>
              <p class="text-xs text-on-surface-variant mt-0.5">{{ setting.desc }}</p>
            </div>
            <WxSwitch v-model="settings[setting.key]" />
          </div>
        </div>
      </WxCard>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 6 — Filter Bar with Removable Tags     -->
    <!-- ═══════════════════════════════════════════════ -->
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

          <WxFilterBar
            :filters="filters"
            @remove="removeFilter"
            @clear="clearFilters"
          />

          <p v-if="filters.length === 0" class="text-sm text-on-surface-variant italic">
            Kh\u00F4ng c\u00F3 b\u1ED9 l\u1ECDc n\u00E0o \u0111ang ho\u1EA1t \u0111\u1ED9ng. Nh\u1EA5n "Reset" \u0111\u1EC3 kh\u00F4i ph\u1EE5c.
          </p>
        </div>
      </WxCard>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 7 — Activity Log Table                 -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <ClipboardList class="w-6 h-6" />
        <span>Activity Log Table</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <WxDataTable
          :columns="activityLogColumns"
          :data="activityLogData"
          v-model:sort-key="activitySortKey"
          v-model:sort-direction="activitySortDir"
        >
          <template #cell-time="{ value }">
            <span class="tabular-nums text-on-surface-variant font-mono text-xs">{{ value }}</span>
          </template>
          <template #cell-user="{ value }">
            <span class="font-medium">{{ value }}</span>
          </template>
          <template #cell-action="{ value }">
            <WxBadge :variant="actionBadgeVariant[value as string] || 'info'" size="sm">
              {{ value }}
            </WxBadge>
          </template>
          <template #cell-object="{ value }">
            <span class="font-medium text-on-surface">{{ value }}</span>
          </template>
          <template #cell-ip="{ value }">
            <span class="tabular-nums text-on-surface-variant font-mono text-xs">{{ value }}</span>
          </template>
        </WxDataTable>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 8 — Member Table                       -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Users class="w-6 h-6" />
        <span>Member Table</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <WxDataTable
          :columns="memberColumns"
          :data="memberData"
          v-model:sort-key="memberSortKey"
          v-model:sort-direction="memberSortDir"
        >
          <template #cell-name="{ value, row }">
            <div class="flex items-center gap-3">
              <WxAvatar :name="value as string" size="sm" :status="(row as Record<string, unknown>).status === 'online' ? 'online' : 'offline'" />
              <span class="font-medium">{{ value }}</span>
            </div>
          </template>
          <template #cell-email="{ value }">
            <span class="text-on-surface-variant text-sm">{{ value }}</span>
          </template>
          <template #cell-role="{ value }">
            <WxBadge :variant="roleBadgeVariant[value as string] || 'info'" size="sm">
              {{ value }}
            </WxBadge>
          </template>
          <template #cell-lastActive="{ value }">
            <span class="text-on-surface-variant text-sm">{{ value }}</span>
          </template>
          <template #cell-status="{ value }">
            <div class="flex items-center gap-2">
              <span
                class="w-2 h-2 rounded-full"
                :class="value === 'online' ? 'bg-success' : 'bg-on-surface-variant/30'"
              />
              <span class="text-sm capitalize">{{ value }}</span>
            </div>
          </template>
        </WxDataTable>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 9 — Loading/Empty State Demo           -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Loader2 class="w-6 h-6" />
        <span>Loading / Empty State Demo</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant space-y-6">
        <!-- State toggle buttons -->
        <div class="flex items-center gap-3">
          <WxButton
            :variant="demoState === 'loading' ? 'primary' : 'secondary'"
            size="sm"
            @click="demoState = 'loading'"
          >
            Loading
          </WxButton>
          <WxButton
            :variant="demoState === 'empty' ? 'primary' : 'secondary'"
            size="sm"
            @click="demoState = 'empty'"
          >
            Empty
          </WxButton>
          <WxButton
            :variant="demoState === 'loaded' ? 'primary' : 'secondary'"
            size="sm"
            @click="demoState = 'loaded'"
          >
            Loaded
          </WxButton>
        </div>

        <!-- Two side-by-side cards -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Card A -->
          <div class="border border-outline-variant rounded-xl p-6 min-h-50 flex items-center justify-center">
            <div v-if="demoState === 'loading'" class="flex flex-col items-center gap-3">
              <WxSpinner size="lg" />
              <span class="text-sm text-on-surface-variant">Dang tai du lieu...</span>
            </div>
            <WxEmptyState
              v-else-if="demoState === 'empty'"
              title="Khong co du lieu"
              description="Chua co chien dich nao duoc tao. Hay tao chien dich dau tien."
            >
              <template #action>
                <WxButton variant="primary" size="sm">
                  <template #icon><Plus :size="14" /></template>
                  Tao chien dich
                </WxButton>
              </template>
            </WxEmptyState>
            <div v-else class="w-full space-y-2">
              <div
                v-for="item in demoItems"
                :key="item"
                class="flex items-center justify-between p-3 rounded-lg bg-surface-container hover:bg-surface-container-high transition-colors"
              >
                <span class="font-medium text-sm">{{ item }}</span>
                <WxBadge variant="success" size="sm">Active</WxBadge>
              </div>
            </div>
          </div>

          <!-- Card B -->
          <div class="border border-outline-variant rounded-xl p-6 min-h-50 flex items-center justify-center">
            <div v-if="demoState === 'loading'" class="flex flex-col items-center gap-3">
              <WxSpinner size="lg" />
              <span class="text-sm text-on-surface-variant">Dang dong bo...</span>
            </div>
            <WxEmptyState
              v-else-if="demoState === 'empty'"
              title="Khong co ket qua"
              description="Thu thay doi bo loc de tim kiem lai."
            />
            <div v-else class="w-full space-y-3">
              <div class="flex items-center justify-between">
                <span class="text-sm font-semibold">Tong chi tieu</span>
                <span class="text-lg font-bold text-primary">$4,567.89</span>
              </div>
              <WxProgressBar :value="68" variant="primary" size="md" show-value />
              <p class="text-xs text-on-surface-variant">68% ngan sach da su dung</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 10 — WxOptionCard                      -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <LayoutGrid class="w-6 h-6" />
        <span>WxOptionCard</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <!-- Standard -->
          <div
            class="relative border-2 rounded-xl p-6 cursor-pointer transition-all duration-200"
            :class="selectedMode === 'standard' ? 'border-primary bg-primary/5' : 'border-outline-variant hover:border-outline'"
            @click="selectedMode = 'standard'"
          >
            <div class="flex items-start gap-3">
              <span
                class="mt-0.5 flex items-center justify-center w-5 h-5 rounded-full border-2 transition-colors shrink-0"
                :class="selectedMode === 'standard' ? 'border-primary' : 'border-outline-variant'"
              >
                <span v-if="selectedMode === 'standard'" class="w-2.5 h-2.5 rounded-full bg-primary" />
              </span>
              <div>
                <p class="font-semibold text-on-surface">Standard</p>
                <p class="text-sm text-on-surface-variant mt-1">Goi co ban danh cho ca nhan va nhom nho. Bao gom quan ly toi da 5 TKQC va 1 BM.</p>
              </div>
            </div>
          </div>

          <!-- Advanced -->
          <div
            class="relative border-2 rounded-xl p-6 cursor-pointer transition-all duration-200"
            :class="selectedMode === 'advanced' ? 'border-primary bg-primary/5' : 'border-outline-variant hover:border-outline'"
            @click="selectedMode = 'advanced'"
          >
            <div class="flex items-start gap-3">
              <span
                class="mt-0.5 flex items-center justify-center w-5 h-5 rounded-full border-2 transition-colors shrink-0"
                :class="selectedMode === 'advanced' ? 'border-primary' : 'border-outline-variant'"
              >
                <span v-if="selectedMode === 'advanced'" class="w-2.5 h-2.5 rounded-full bg-primary" />
              </span>
              <div>
                <p class="font-semibold text-on-surface">Advanced</p>
                <p class="text-sm text-on-surface-variant mt-1">Goi nang cao cho doanh nghiep. Quan ly khong gioi han TKQC, 10 BM, ho tro proxy va API.</p>
              </div>
            </div>
          </div>

          <!-- Enterprise (disabled) -->
          <div
            class="relative border-2 rounded-xl p-6 opacity-50 cursor-not-allowed transition-all duration-200 border-outline-variant"
          >
            <div class="flex items-start gap-3">
              <span class="mt-0.5 flex items-center justify-center w-5 h-5 rounded-full border-2 border-outline-variant shrink-0" />
              <div>
                <p class="font-semibold text-on-surface">Enterprise</p>
                <p class="text-sm text-on-surface-variant mt-1">Goi cao cap voi bao mat toi da, SLA 99.9%, ho tro 24/7 va tuy chinh khong gioi han.</p>
                <WxBadge variant="warning" size="sm" class="mt-2">Requires 2FA</WxBadge>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 11 — WxCollapsiblePanel                -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <ChevronDown class="w-6 h-6" />
        <span>WxCollapsiblePanel</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <div class="divide-y divide-outline-variant rounded-xl border border-outline-variant overflow-hidden">
          <div v-for="(panel, index) in faqPanels" :key="index">
            <button
              class="w-full flex items-center justify-between px-6 py-4 text-left hover:bg-surface-container-high transition-colors"
              @click="togglePanel(index)"
            >
              <span class="font-semibold text-on-surface">{{ panel.title }}</span>
              <ChevronDown
                class="w-5 h-5 text-on-surface-variant transition-transform duration-300"
                :class="expandedPanel === index ? 'rotate-180' : ''"
              />
            </button>
            <Transition
              enter-active-class="transition-all duration-300 ease-out"
              enter-from-class="max-h-0 opacity-0"
              enter-to-class="max-h-40 opacity-100"
              leave-active-class="transition-all duration-200 ease-in"
              leave-from-class="max-h-40 opacity-100"
              leave-to-class="max-h-0 opacity-0"
            >
              <div v-if="expandedPanel === index" class="overflow-hidden">
                <p class="px-6 pb-4 text-sm text-on-surface-variant leading-relaxed">
                  {{ panel.content }}
                </p>
              </div>
            </Transition>
          </div>
        </div>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 12 — WxFeatureCard                     -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Briefcase class="w-6 h-6" />
        <span>WxFeatureCard</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div
            v-for="feature in featureCards"
            :key="feature.title"
            class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6 hover:shadow-lg transition-shadow duration-300 group"
          >
            <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center mb-4 group-hover:bg-primary/20 transition-colors">
              <component :is="feature.icon" class="w-6 h-6 text-primary" />
            </div>
            <h3 class="font-semibold text-on-surface text-lg mb-2">{{ feature.title }}</h3>
            <p class="text-sm text-on-surface-variant leading-relaxed">{{ feature.description }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 13 — WxNotificationPatterns            -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Bell class="w-6 h-6" />
        <span>WxNotificationPatterns</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <div class="space-y-1">
          <div
            v-for="notif in notifications"
            :key="notif.id"
            class="flex items-start gap-4 p-4 rounded-xl hover:bg-surface-container-high transition-colors"
          >
            <WxAvatar :name="notif.avatar" size="md" />
            <div class="flex-1 min-w-0">
              <div class="flex items-center gap-2 mb-1">
                <WxBadge :variant="notif.badge" size="sm">{{ notif.type }}</WxBadge>
              </div>
              <p class="text-sm font-medium text-on-surface">{{ notif.title }}</p>
              <p class="text-xs text-on-surface-variant mt-1">{{ notif.time }}</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 14 — WxCardPatterns                    -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <LayoutGrid class="w-6 h-6" />
        <span>WxCardPatterns</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <!-- Image Card -->
          <div class="rounded-xl border border-outline-variant overflow-hidden hover:shadow-lg transition-shadow">
            <div class="h-32 bg-linear-to-br from-primary to-primary/60 flex items-end p-4">
              <h3 class="text-white font-bold text-lg">Campaign Overview</h3>
            </div>
            <div class="p-4 space-y-3">
              <p class="text-sm text-on-surface-variant">Tong quan hieu suat chien dich quang cao trong thang 3/2026.</p>
              <div class="flex items-center justify-between text-sm">
                <span class="text-on-surface-variant">Impressions</span>
                <span class="font-bold text-on-surface">1.2M</span>
              </div>
              <div class="flex items-center justify-between text-sm">
                <span class="text-on-surface-variant">Clicks</span>
                <span class="font-bold text-on-surface">45.6K</span>
              </div>
              <div class="flex items-center justify-between text-sm">
                <span class="text-on-surface-variant">CTR</span>
                <span class="font-bold text-primary">3.8%</span>
              </div>
            </div>
          </div>

          <!-- Stat Card -->
          <div class="rounded-xl border border-outline-variant p-6 flex flex-col justify-between hover:shadow-lg transition-shadow">
            <div>
              <p class="text-sm text-on-surface-variant mb-2">Tong chi tieu thang nay</p>
              <p class="text-3xl font-bold text-on-surface">$24,580</p>
              <p class="text-sm text-success mt-1">+12.5% so voi thang truoc</p>
            </div>
            <div class="mt-6">
              <div class="flex items-center justify-between text-xs text-on-surface-variant mb-2">
                <span>Da chi</span>
                <span>72%</span>
              </div>
              <WxProgressBar :value="72" variant="primary" size="md" />
            </div>
          </div>

          <!-- Dark Card -->
          <div class="rounded-xl bg-[#1a1a2e] p-6 flex flex-col justify-between hover:shadow-lg transition-shadow">
            <div>
              <div class="w-10 h-10 rounded-lg bg-primary/20 flex items-center justify-center mb-4">
                <Shield class="w-5 h-5 text-primary" />
              </div>
              <h3 class="font-bold text-white text-lg mb-2">Bao mat tai khoan</h3>
              <p class="text-sm text-on-surface-variant leading-relaxed">
                He thong bao mat nhieu lop giup bao ve TKQC va BM khoi truy cap trai phep. Ho tro 2FA, IP whitelist va canh bao dang nhap bat thuong.
              </p>
            </div>
            <WxButton variant="primary" size="sm" class="mt-4 self-start">
              <template #icon><Shield :size="14" /></template>
              Cai dat bao mat
            </WxButton>
          </div>
        </div>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 15 — WxFormLayout                      -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <PenTool class="w-6 h-6" />
        <span>WxFormLayout</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <form @submit.prevent class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Column 1 -->
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-semibold text-on-surface mb-1.5">Ten chien dich</label>
                <WxInput v-model="formCampaignName" placeholder="Nhap ten chien dich..." />
              </div>
              <div>
                <label class="block text-sm font-semibold text-on-surface mb-1.5">Muc tieu</label>
                <WxSelect v-model="formObjective" :options="objectiveOptions" placeholder="Chon muc tieu..." />
              </div>
              <div>
                <label class="block text-sm font-semibold text-on-surface mb-1.5">Ngan sach hang ngay (VND)</label>
                <WxInputNumber v-model="formDailyBudget" :min="100000" :max="100000000" :step="100000" />
              </div>
            </div>

            <!-- Column 2 -->
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-semibold text-on-surface mb-1.5">Ngay bat dau</label>
                <WxDatePicker v-model="formStartDate" placeholder="Chon ngay bat dau" />
              </div>
              <div>
                <label class="block text-sm font-semibold text-on-surface mb-1.5">Ngay ket thuc</label>
                <WxDatePicker v-model="formEndDate" placeholder="Chon ngay ket thuc" />
              </div>
              <div>
                <label class="block text-sm font-semibold text-on-surface mb-1.5">Khu vuc muc tieu</label>
                <WxMultiSelect v-model="formRegions" :options="regionOptions" placeholder="Chon khu vuc..." />
              </div>
            </div>
          </div>

          <!-- Full-width textarea -->
          <div>
            <label class="block text-sm font-semibold text-on-surface mb-1.5">Ghi chu</label>
            <WxTextarea v-model="formNotes" placeholder="Nhap ghi chu ve chien dich..." :rows="3" />
          </div>

          <!-- Submit buttons -->
          <div class="flex items-center gap-3 pt-2">
            <WxButton variant="primary">
              <template #icon><Plus :size="14" /></template>
              Tao chien dich
            </WxButton>
            <WxButton variant="secondary">Luu nhap</WxButton>
            <WxButton variant="ghost">Huy bo</WxButton>
          </div>
        </form>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 16 — WxDataVisualization               -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <BarChart3 class="w-6 h-6" />
        <span>WxDataVisualization</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <!-- Bar Chart -->
          <div>
            <h4 class="font-semibold text-on-surface mb-4">Chi tieu hang ngay (VND)</h4>
            <div class="flex items-end gap-3 h-48">
              <div
                v-for="bar in barChartData"
                :key="bar.day"
                class="flex-1 flex flex-col items-center gap-2"
              >
                <span class="text-xs text-on-surface-variant tabular-nums">{{ formatVND(bar.value) }}</span>
                <div class="w-full rounded-t-md bg-linear-to-t from-primary to-primary/60 transition-all duration-500" :style="{ height: bar.percent + '%' }" />
                <span class="text-xs font-medium text-on-surface-variant">{{ bar.day }}</span>
              </div>
            </div>
          </div>

          <!-- Line Chart (CSS mockup) -->
          <div>
            <h4 class="font-semibold text-on-surface mb-4">Doanh thu hang ngay (VND)</h4>
            <div class="relative h-48">
              <!-- Background grid lines -->
              <div class="absolute inset-0 flex flex-col justify-between">
                <div class="border-b border-outline-variant/30 w-full" />
                <div class="border-b border-outline-variant/30 w-full" />
                <div class="border-b border-outline-variant/30 w-full" />
                <div class="border-b border-outline-variant/30 w-full" />
                <div class="border-b border-outline-variant/30 w-full" />
              </div>
              <!-- Gradient area under the line -->
              <div class="absolute inset-x-0 bottom-6 top-0">
                <svg class="w-full h-full" viewBox="0 0 700 160" preserveAspectRatio="none">
                  <defs>
                    <linearGradient id="lineGradient" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="0%" style="stop-color: var(--color-primary); stop-opacity: 0.3" />
                      <stop offset="100%" style="stop-color: var(--color-primary); stop-opacity: 0.02" />
                    </linearGradient>
                  </defs>
                  <polygon
                    points="50,112 150,80 250,96 350,48 450,64 550,120 650,136 650,160 50,160"
                    fill="url(#lineGradient)"
                  />
                  <polyline
                    points="50,112 150,80 250,96 350,48 450,64 550,120 650,136"
                    fill="none"
                    stroke="var(--color-primary)"
                    stroke-width="2.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                  <!-- Data points -->
                  <circle cx="50" cy="112" r="4" fill="var(--color-primary)" />
                  <circle cx="150" cy="80" r="4" fill="var(--color-primary)" />
                  <circle cx="250" cy="96" r="4" fill="var(--color-primary)" />
                  <circle cx="350" cy="48" r="4" fill="var(--color-primary)" />
                  <circle cx="450" cy="64" r="4" fill="var(--color-primary)" />
                  <circle cx="550" cy="120" r="4" fill="var(--color-primary)" />
                  <circle cx="650" cy="136" r="4" fill="var(--color-primary)" />
                </svg>
              </div>
              <!-- Day labels -->
              <div class="absolute bottom-0 inset-x-0 flex justify-between px-2">
                <span v-for="pt in lineChartPoints" :key="pt.day" class="text-xs font-medium text-on-surface-variant">{{ pt.day }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- ═══════════════════════════════════════════════ -->
    <!-- Section 17 — WxLayoutPatterns                  -->
    <!-- ═══════════════════════════════════════════════ -->
    <section class="space-y-4">
      <div class="flex items-center space-x-2 text-primary font-bold text-xl">
        <Monitor class="w-6 h-6" />
        <span>WxLayoutPatterns</span>
      </div>

      <div class="bg-surface-container-lowest p-8 rounded-xl shadow-sm border border-outline-variant">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- App Shell Mockup -->
          <div class="border border-outline-variant rounded-xl overflow-hidden">
            <div class="bg-surface-container-high px-4 py-2 flex items-center gap-2 border-b border-outline-variant">
              <div class="flex gap-1.5">
                <span class="w-3 h-3 rounded-full bg-error/60" />
                <span class="w-3 h-3 rounded-full bg-warning/60" />
                <span class="w-3 h-3 rounded-full bg-success/60" />
              </div>
              <span class="text-xs text-on-surface-variant ml-2">App Shell</span>
            </div>
            <div class="flex h-48">
              <!-- Sidebar -->
              <div class="w-16 bg-surface-container border-r border-outline-variant flex flex-col items-center gap-3 py-4">
                <div class="w-8 h-8 rounded-lg bg-primary/20" />
                <div class="w-8 h-2 rounded bg-on-surface-variant/20" />
                <div class="w-8 h-2 rounded bg-on-surface-variant/20" />
                <div class="w-8 h-2 rounded bg-primary/40" />
                <div class="w-8 h-2 rounded bg-on-surface-variant/20" />
              </div>
              <!-- Content -->
              <div class="flex-1 p-4 space-y-3">
                <div class="w-1/3 h-3 rounded bg-on-surface-variant/20" />
                <div class="w-full h-2 rounded bg-on-surface-variant/10" />
                <div class="w-full h-2 rounded bg-on-surface-variant/10" />
                <div class="w-2/3 h-2 rounded bg-on-surface-variant/10" />
                <div class="flex gap-2 mt-4">
                  <div class="flex-1 h-16 rounded-lg bg-primary/10 border border-primary/20" />
                  <div class="flex-1 h-16 rounded-lg bg-surface-container border border-outline-variant" />
                  <div class="flex-1 h-16 rounded-lg bg-surface-container border border-outline-variant" />
                </div>
              </div>
            </div>
          </div>

          <!-- Centered Layout Mockup -->
          <div class="border border-outline-variant rounded-xl overflow-hidden">
            <div class="bg-surface-container-high px-4 py-2 flex items-center gap-2 border-b border-outline-variant">
              <div class="flex gap-1.5">
                <span class="w-3 h-3 rounded-full bg-error/60" />
                <span class="w-3 h-3 rounded-full bg-warning/60" />
                <span class="w-3 h-3 rounded-full bg-success/60" />
              </div>
              <span class="text-xs text-on-surface-variant ml-2">Centered Layout</span>
            </div>
            <div class="h-48 flex items-center justify-center bg-surface-container-lowest">
              <div class="w-3/5 space-y-3 text-center">
                <div class="w-12 h-12 rounded-full bg-primary/20 mx-auto" />
                <div class="w-1/2 h-3 rounded bg-on-surface-variant/20 mx-auto" />
                <div class="w-3/4 h-2 rounded bg-on-surface-variant/10 mx-auto" />
                <div class="w-2/3 h-2 rounded bg-on-surface-variant/10 mx-auto" />
                <div class="flex gap-2 justify-center mt-4">
                  <div class="w-20 h-8 rounded-lg bg-primary/80" />
                  <div class="w-20 h-8 rounded-lg bg-surface-container border border-outline-variant" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>
