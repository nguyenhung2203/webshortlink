<script setup lang="ts">
import { ref, reactive } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxModal from '@/components/ui/WxModal.vue'
import WxConfirmDialog from '@/components/ui/WxConfirmDialog.vue'
import WxDrawer from '@/components/ui/WxDrawer.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxSelect from '@/components/ui/WxSelect.vue'
import WxTextarea from '@/components/ui/WxTextarea.vue'
import WxSlider from '@/components/ui/WxSlider.vue'
import WxSwitch from '@/components/ui/WxSwitch.vue'
import WxTabs from '@/components/ui/WxTabs.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import WxDatePicker from '@/components/ui/WxDatePicker.vue'
import { useToast } from '@/composables/useToast'
import {
  Info,
  CheckCircle2,
  XCircle,
  HelpCircle,
  Award,
  Trash2,
  AlertTriangle,
  PanelRight,
  Monitor,
  Settings,
  Megaphone,
  Cpu,
} from 'lucide-vue-next'

const toast = useToast()

// ---- Message modals ----
const showInfoModal = ref(false)
const showSuccessModal = ref(false)
const showErrorModal = ref(false)
const showQuestionModal = ref(false)

// ---- Result dialog ----
const showResultDialog = ref(false)

// ---- Delete confirm ----
const showDeleteConfirm = ref(false)

// ---- Warning alert ----
const showWarningAlert = ref(false)

// ---- Drawer ----
const showDrawer = ref(false)
const drawerForm = reactive({
  name: '',
  email: '',
  role: null as string | number | null,
  bio: '',
  notify: true,
})
const roleOptions = [
  { label: 'Admin', value: 'admin' },
  { label: 'Editor', value: 'editor' },
  { label: 'Viewer', value: 'viewer' },
  { label: 'Contributor', value: 'contributor' },
]

// ---- Default modal ----
const showDefaultModal = ref(false)

// ---- Config dialog (tabbed) ----
const showConfigDialog = ref(false)
const configTab = ref<string | number>('proxy')
const configTabs = [
  { key: 'proxy', label: 'Proxy' },
  { key: 'general', label: 'Chung' },
  { key: 'advanced', label: 'Nang cao' },
]
const proxyForm = reactive({
  host: '192.168.1.100',
  port: '8080',
  username: '',
  password: '',
  protocol: 'http' as string | number | null,
  enabled: true,
})
const protocolOptions = [
  { label: 'HTTP', value: 'http' },
  { label: 'HTTPS', value: 'https' },
  { label: 'SOCKS5', value: 'socks5' },
]
const generalConfig = reactive({
  autoStart: true,
  language: 'vi' as string | number | null,
  maxRetries: 3,
  timeout: 30,
})
const langOptions = [
  { label: 'Tieng Viet', value: 'vi' },
  { label: 'English', value: 'en' },
  { label: 'Japanese', value: 'ja' },
]
const advancedConfig = reactive({
  debugMode: false,
  logLevel: 'info' as string | number | null,
  cacheTtl: 60,
  experimentalFeatures: false,
})
const logLevelOptions = [
  { label: 'Debug', value: 'debug' },
  { label: 'Info', value: 'info' },
  { label: 'Warning', value: 'warn' },
  { label: 'Error', value: 'error' },
]

// ---- Campaign form modal ----
const showCampaignForm = ref(false)
const campaignForm = reactive({
  name: '',
  method: null as string | number | null,
  startDate: '',
  endDate: '',
  budget: 50,
  notes: '',
  autoOptimize: true,
})
const methodOptions = [
  { label: 'CPC - Cost Per Click', value: 'cpc' },
  { label: 'CPM - Cost Per Mille', value: 'cpm' },
  { label: 'CPA - Cost Per Action', value: 'cpa' },
  { label: 'CPV - Cost Per View', value: 'cpv' },
]

// ---- System info modal ----
const showSystemInfo = ref(false)
const systemData = {
  version: '2.4.1',
  buildDate: '2026-03-25',
  runtime: 'Node.js 20.11.0',
  uptime: '14 ngay, 7 gio, 23 phut',
  memoryUsed: '1.2 GB',
  memoryTotal: '4.0 GB',
  memoryPercent: 30,
  cpuUsage: '23%',
  diskUsed: '45.2 GB',
  diskTotal: '256 GB',
  diskPercent: 18,
  os: 'Ubuntu 22.04 LTS',
  hostname: 'prod-server-01',
  activeConnections: 142,
  requestsToday: '24,891',
}

// ---- Handlers ----
function handleQuestionConfirm() {
  toast.success('Confirmed', 'You chose to proceed with the action.')
}

function handleDelete() {
  toast.success('Deleted', 'Item has been permanently removed.')
}

function handleWarningConfirm() {
  toast.success('Acknowledged', 'Warning has been acknowledged.')
}

function handleSaveConfig() {
  showConfigDialog.value = false
  toast.success('Saved', 'Configuration settings updated successfully.')
}

function handleCreateCampaign() {
  if (!campaignForm.name) {
    toast.error('Error', 'Campaign name is required.')
    return
  }
  showCampaignForm.value = false
  toast.success('Created', `Campaign "${campaignForm.name}" has been created.`)
  campaignForm.name = ''
  campaignForm.method = null
  campaignForm.startDate = ''
  campaignForm.endDate = ''
  campaignForm.budget = 50
  campaignForm.notes = ''
}

function handleDrawerSave() {
  showDrawer.value = false
  toast.success('Saved', 'Sidebar form data saved successfully.')
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Dialogs & Overlays"
      description="Comprehensive showcase of modals, confirmation dialogs, tabbed config dialogs, form modals, drawers, and system info panels."
    />

    <!-- Trigger Buttons -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Dialog Triggers</h3>
      </template>
      <p class="text-xs text-on-surface-variant mb-4">Click any button below to open the corresponding dialog or overlay.</p>
      <div class="flex flex-wrap gap-3">
        <!-- Info message -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-white bg-gradient-to-r from-cyan-400 to-blue-600 shadow-lg shadow-cyan-500/25 hover:shadow-cyan-500/40 hover:brightness-110 transition-all cursor-pointer"
          @click="showInfoModal = true"
        >
          <Info class="w-3.5 h-3.5" />
          Message (Info)
        </button>

        <!-- Success message -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-white bg-success shadow-lg shadow-green-500/25 hover:brightness-110 transition-all cursor-pointer"
          @click="showSuccessModal = true"
        >
          <CheckCircle2 class="w-3.5 h-3.5" />
          Message (Success)
        </button>

        <!-- Error message -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-white bg-danger shadow-lg shadow-red-500/25 hover:brightness-110 transition-all cursor-pointer"
          @click="showErrorModal = true"
        >
          <XCircle class="w-3.5 h-3.5" />
          Message (Error)
        </button>

        <!-- Question message -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-white bg-gradient-to-r from-cyan-400 to-teal-500 shadow-lg shadow-teal-500/25 hover:brightness-110 transition-all cursor-pointer"
          @click="showQuestionModal = true"
        >
          <HelpCircle class="w-3.5 h-3.5" />
          Message (Question)
        </button>

        <!-- Result dialog -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-on-surface bg-surface-container border border-outline-variant hover:bg-surface-container-high transition-all cursor-pointer"
          @click="showResultDialog = true"
        >
          <Award class="w-3.5 h-3.5" />
          Result Dialog
        </button>

        <!-- Delete confirm -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-danger border border-danger/50 bg-danger/5 hover:bg-danger/10 transition-all cursor-pointer"
          @click="showDeleteConfirm = true"
        >
          <Trash2 class="w-3.5 h-3.5" />
          Delete Confirm
        </button>

        <!-- Warning alert -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-amber-900 dark:text-amber-300 bg-amber-400/80 hover:bg-amber-400 shadow-lg shadow-amber-500/25 transition-all cursor-pointer"
          @click="showWarningAlert = true"
        >
          <AlertTriangle class="w-3.5 h-3.5" />
          Warning Alert
        </button>

        <!-- Sidebar panel -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-on-surface bg-surface-container border border-outline-variant hover:bg-surface-container-high transition-all cursor-pointer"
          @click="showDrawer = true"
        >
          <PanelRight class="w-3.5 h-3.5" />
          Sidebar Panel
        </button>

        <!-- Default modal -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-on-surface-variant bg-surface-container-high hover:bg-outline-variant transition-all cursor-pointer"
          @click="showDefaultModal = true"
        >
          <Monitor class="w-3.5 h-3.5" />
          Default Modal
        </button>

        <!-- Config dialog -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-white bg-gradient-to-r from-violet-500 to-purple-600 shadow-lg shadow-violet-500/25 hover:brightness-110 transition-all cursor-pointer"
          @click="showConfigDialog = true"
        >
          <Settings class="w-3.5 h-3.5" />
          Config Dialog
        </button>

        <!-- Campaign form -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-white bg-gradient-to-r from-orange-400 to-pink-500 shadow-lg shadow-orange-500/25 hover:brightness-110 transition-all cursor-pointer"
          @click="showCampaignForm = true"
        >
          <Megaphone class="w-3.5 h-3.5" />
          Campaign Form
        </button>

        <!-- System info -->
        <button
          class="inline-flex items-center gap-2 px-4 py-2 text-xs font-medium rounded-lg text-white bg-gradient-to-r from-slate-600 to-slate-800 shadow-lg shadow-slate-500/25 hover:brightness-110 transition-all cursor-pointer"
          @click="showSystemInfo = true"
        >
          <Cpu class="w-3.5 h-3.5" />
          System Info
        </button>
      </div>
    </WxCard>

    <!-- ===== INFO MODAL ===== -->
    <WxModal v-model="showInfoModal" title="Information" size="sm">
      <div class="flex flex-col items-center text-center gap-4">
        <div class="w-14 h-14 rounded-full bg-info/10 flex items-center justify-center">
          <Info class="w-7 h-7 text-info" />
        </div>
        <div>
          <h4 class="text-base font-semibold text-on-surface mb-1">Thong bao</h4>
          <p class="text-sm text-on-surface-variant leading-relaxed">
            He thong da duoc cap nhat len phien ban moi nhat. Tat ca cac tinh nang dang hoat dong binh thuong.
          </p>
        </div>
      </div>
      <template #footer>
        <WxButton variant="primary" size="sm" @click="showInfoModal = false">OK</WxButton>
      </template>
    </WxModal>

    <!-- ===== SUCCESS MODAL ===== -->
    <WxModal v-model="showSuccessModal" title="Success" size="sm">
      <div class="flex flex-col items-center text-center gap-4">
        <div class="w-14 h-14 rounded-full bg-success/10 flex items-center justify-center">
          <CheckCircle2 class="w-7 h-7 text-success" />
        </div>
        <div>
          <h4 class="text-base font-semibold text-on-surface mb-1">Thanh cong!</h4>
          <p class="text-sm text-on-surface-variant leading-relaxed">
            Chien dich quang cao da duoc tao thanh cong va se bat dau phan phoi trong vong 24 gio.
          </p>
        </div>
      </div>
      <template #footer>
        <WxButton variant="success" size="sm" @click="showSuccessModal = false">OK</WxButton>
      </template>
    </WxModal>

    <!-- ===== ERROR MODAL ===== -->
    <WxModal v-model="showErrorModal" title="Error" size="sm">
      <div class="flex flex-col items-center text-center gap-4">
        <div class="w-14 h-14 rounded-full bg-danger/10 flex items-center justify-center">
          <XCircle class="w-7 h-7 text-danger" />
        </div>
        <div>
          <h4 class="text-base font-semibold text-on-surface mb-1">Loi xay ra!</h4>
          <p class="text-sm text-on-surface-variant leading-relaxed">
            Khong the ket noi den may chu. Vui long kiem tra ket noi mang va thu lai sau it phut.
          </p>
          <p class="text-xs text-danger/70 mt-2 font-mono">Error Code: ERR_CONNECTION_TIMEOUT</p>
        </div>
      </div>
      <template #footer>
        <WxButton variant="danger" size="sm" @click="showErrorModal = false">Dong</WxButton>
      </template>
    </WxModal>

    <!-- ===== QUESTION MODAL ===== -->
    <WxModal v-model="showQuestionModal" title="Xac nhan" size="sm">
      <div class="flex flex-col items-center text-center gap-4">
        <div class="w-14 h-14 rounded-full bg-info/10 flex items-center justify-center">
          <HelpCircle class="w-7 h-7 text-info" />
        </div>
        <div>
          <h4 class="text-base font-semibold text-on-surface mb-1">Ban co muon tiep tuc?</h4>
          <p class="text-sm text-on-surface-variant leading-relaxed">
            Hanh dong nay se thay doi cau hinh hien tai. Ban co chac chan muon ap dung thay doi khong?
          </p>
        </div>
      </div>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="showQuestionModal = false">Huy bo</WxButton>
        <WxButton variant="primary" size="sm" @click="showQuestionModal = false; handleQuestionConfirm()">Xac nhan</WxButton>
      </template>
    </WxModal>

    <!-- ===== RESULT DIALOG ===== -->
    <WxModal v-model="showResultDialog" title="Ket qua chien dich" size="md">
      <div class="space-y-4">
        <div class="flex flex-col items-center text-center gap-3 pb-4 border-b border-outline-variant">
          <div class="w-16 h-16 rounded-full bg-success/10 flex items-center justify-center">
            <Award class="w-8 h-8 text-success" />
          </div>
          <div>
            <h4 class="text-lg font-bold text-on-surface">Chien dich hoan thanh!</h4>
            <p class="text-sm text-on-surface-variant">Tet Holiday Promotion - 30 ngay</p>
          </div>
        </div>
        <div class="grid grid-cols-2 gap-3">
          <div class="bg-surface-container rounded-lg p-3 text-center">
            <p class="text-xs text-on-surface-variant">Luot hien thi</p>
            <p class="text-lg font-bold text-on-surface">2.4M</p>
            <p class="text-xs text-success">+18% so voi ky truoc</p>
          </div>
          <div class="bg-surface-container rounded-lg p-3 text-center">
            <p class="text-xs text-on-surface-variant">Luot click</p>
            <p class="text-lg font-bold text-on-surface">89,234</p>
            <p class="text-xs text-success">+12% so voi ky truoc</p>
          </div>
          <div class="bg-surface-container rounded-lg p-3 text-center">
            <p class="text-xs text-on-surface-variant">Chuyen doi</p>
            <p class="text-lg font-bold text-on-surface">3,456</p>
            <p class="text-xs text-success">+25% so voi ky truoc</p>
          </div>
          <div class="bg-surface-container rounded-lg p-3 text-center">
            <p class="text-xs text-on-surface-variant">Chi phi / Click</p>
            <p class="text-lg font-bold text-on-surface">$0.38</p>
            <p class="text-xs text-success">-8% so voi ky truoc</p>
          </div>
        </div>
      </div>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="showResultDialog = false">Dong</WxButton>
        <WxButton variant="primary" size="sm" @click="showResultDialog = false; toast.success('Exported', 'Report PDF downloaded.')">Tai bao cao</WxButton>
      </template>
    </WxModal>

    <!-- ===== DELETE CONFIRM ===== -->
    <WxConfirmDialog
      v-model="showDeleteConfirm"
      type="danger"
      title="Xoa du lieu"
      message="Ban co chac muon xoa? Hanh dong nay khong the hoan tac. Tat ca du lieu lien quan se bi xoa vinh vien khoi he thong."
      confirm-text="Xoa"
      cancel-text="Huy"
      @confirm="handleDelete"
    />

    <!-- ===== WARNING ALERT ===== -->
    <WxConfirmDialog
      v-model="showWarningAlert"
      type="warning"
      title="Canh bao ngan sach"
      message="Ngan sach chien dich da su dung 92%. Neu khong dieu chinh, chien dich se tu dong tam dung trong vong 24 gio toi."
      confirm-text="Da hieu"
      cancel-text="Xem chi tiet"
      @confirm="handleWarningConfirm"
    />

    <!-- ===== DEFAULT MODAL ===== -->
    <WxModal v-model="showDefaultModal" title="Modal mac dinh" size="md">
      <div class="space-y-3">
        <p class="text-sm text-on-surface-variant leading-relaxed">
          Day la modal mac dinh voi kich thuoc trung binh. No co the chua bat ky noi dung nao ban muon hien thi cho nguoi dung.
        </p>
        <div class="bg-surface-container rounded-lg p-4">
          <p class="text-xs text-on-surface-variant mb-2">Vi du noi dung ben trong:</p>
          <ul class="text-sm text-on-surface space-y-1">
            <li class="flex items-center gap-2">
              <span class="w-1.5 h-1.5 rounded-full bg-primary" />
              Hien thi thong tin chi tiet
            </li>
            <li class="flex items-center gap-2">
              <span class="w-1.5 h-1.5 rounded-full bg-primary" />
              Xac nhan hanh dong nguoi dung
            </li>
            <li class="flex items-center gap-2">
              <span class="w-1.5 h-1.5 rounded-full bg-primary" />
              Thu thap du lieu tu form
            </li>
          </ul>
        </div>
      </div>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="showDefaultModal = false">Dong</WxButton>
      </template>
    </WxModal>

    <!-- ===== CONFIG DIALOG (TABBED) ===== -->
    <WxModal v-model="showConfigDialog" title="Cau hinh he thong" size="lg">
      <WxTabs v-model="configTab" :items="configTabs">
        <template #default="{ activeKey }">
          <!-- Proxy tab -->
          <div v-if="activeKey === 'proxy'" class="space-y-4">
            <div class="flex items-center justify-between mb-2">
              <p class="text-sm font-medium text-on-surface">Proxy Settings</p>
              <WxSwitch v-model="proxyForm.enabled" label="Kich hoat" size="sm" />
            </div>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-on-surface mb-1.5">Host</label>
                <WxInput v-model="proxyForm.host" placeholder="192.168.1.1" :disabled="!proxyForm.enabled" />
              </div>
              <div>
                <label class="block text-sm font-medium text-on-surface mb-1.5">Port</label>
                <WxInput v-model="proxyForm.port" placeholder="8080" :disabled="!proxyForm.enabled" />
              </div>
              <div>
                <label class="block text-sm font-medium text-on-surface mb-1.5">Username</label>
                <WxInput v-model="proxyForm.username" placeholder="proxy_user" :disabled="!proxyForm.enabled" />
              </div>
              <div>
                <label class="block text-sm font-medium text-on-surface mb-1.5">Password</label>
                <WxPasswordInput v-model="proxyForm.password" placeholder="Proxy password" :disabled="!proxyForm.enabled" />
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1.5">Protocol</label>
              <WxSelect v-model="proxyForm.protocol" :options="protocolOptions" :disabled="!proxyForm.enabled" />
            </div>
          </div>

          <!-- General tab -->
          <div v-if="activeKey === 'general'" class="space-y-4">
            <div class="flex items-center justify-between py-3 border-b border-outline-variant">
              <div>
                <p class="text-sm font-medium text-on-surface">Tu dong khoi dong</p>
                <p class="text-xs text-on-surface-variant">Khoi dong ung dung khi dang nhap</p>
              </div>
              <WxSwitch v-model="generalConfig.autoStart" />
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1.5">Ngon ngu</label>
              <WxSelect v-model="generalConfig.language" :options="langOptions" />
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1.5">So lan thu lai toi da</label>
              <WxSlider v-model="generalConfig.maxRetries" :min="0" :max="10" :step="1" show-value />
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1.5">Timeout (giay)</label>
              <WxSlider v-model="generalConfig.timeout" :min="5" :max="120" :step="5" show-value />
            </div>
          </div>

          <!-- Advanced tab -->
          <div v-if="activeKey === 'advanced'" class="space-y-4">
            <div class="flex items-center justify-between py-3 border-b border-outline-variant">
              <div>
                <p class="text-sm font-medium text-on-surface">Che do debug</p>
                <p class="text-xs text-on-surface-variant">Ghi log chi tiet de debug</p>
              </div>
              <WxSwitch v-model="advancedConfig.debugMode" />
            </div>
            <div class="flex items-center justify-between py-3 border-b border-outline-variant">
              <div>
                <p class="text-sm font-medium text-on-surface">Tinh nang thu nghiem</p>
                <p class="text-xs text-on-surface-variant">Bat cac tinh nang chua chinh thuc</p>
              </div>
              <WxSwitch v-model="advancedConfig.experimentalFeatures" />
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1.5">Muc do log</label>
              <WxSelect v-model="advancedConfig.logLevel" :options="logLevelOptions" />
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1.5">Cache TTL (giay)</label>
              <WxSlider v-model="advancedConfig.cacheTtl" :min="10" :max="3600" :step="10" show-value />
            </div>
          </div>
        </template>
      </WxTabs>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="showConfigDialog = false">Huy</WxButton>
        <WxButton variant="primary" size="sm" @click="handleSaveConfig">Luu cau hinh</WxButton>
      </template>
    </WxModal>

    <!-- ===== CAMPAIGN FORM MODAL ===== -->
    <WxModal v-model="showCampaignForm" title="Tao chien dich moi" size="lg">
      <div class="space-y-4">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="md:col-span-2">
            <label class="block text-sm font-medium text-on-surface mb-1.5">Ten chien dich <span class="text-danger">*</span></label>
            <WxInput v-model="campaignForm.name" placeholder="vd: Khuyen mai He 2026" />
          </div>
          <div>
            <label class="block text-sm font-medium text-on-surface mb-1.5">Phuong thuc tinh phi</label>
            <WxSelect v-model="campaignForm.method" :options="methodOptions" placeholder="Chon phuong thuc..." />
          </div>
          <div>
            <label class="block text-sm font-medium text-on-surface mb-1.5">Ngay bat dau</label>
            <WxDatePicker v-model="campaignForm.startDate" placeholder="Chon ngay bat dau" />
          </div>
          <div>
            <label class="block text-sm font-medium text-on-surface mb-1.5">Ngay ket thuc</label>
            <WxDatePicker v-model="campaignForm.endDate" placeholder="Chon ngay ket thuc" />
          </div>
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1.5">
            Ngan sach hang ngay: <span class="text-primary font-semibold">${{ campaignForm.budget }}</span>
          </label>
          <WxSlider v-model="campaignForm.budget" :min="10" :max="500" :step="5" show-value />
        </div>
        <div class="flex items-center justify-between py-2">
          <div>
            <p class="text-sm font-medium text-on-surface">Tu dong toi uu</p>
            <p class="text-xs text-on-surface-variant">He thong se tu dong dieu chinh gia thau</p>
          </div>
          <WxSwitch v-model="campaignForm.autoOptimize" />
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1.5">Ghi chu</label>
          <WxTextarea v-model="campaignForm.notes" placeholder="Them ghi chu cho chien dich..." :rows="3" />
        </div>
      </div>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="showCampaignForm = false">Huy</WxButton>
        <WxButton variant="primary" size="sm" @click="handleCreateCampaign">Tao chien dich</WxButton>
      </template>
    </WxModal>

    <!-- ===== SYSTEM INFO MODAL ===== -->
    <WxModal v-model="showSystemInfo" title="Thong tin he thong" size="lg">
      <div class="space-y-5">
        <!-- Version info -->
        <div class="flex items-start gap-4 p-4 bg-surface-container rounded-lg">
          <div class="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center shrink-0">
            <Cpu class="w-5 h-5 text-primary" />
          </div>
          <div class="flex-1 space-y-1">
            <p class="text-sm font-semibold text-on-surface">WeUI Platform v{{ systemData.version }}</p>
            <p class="text-xs text-on-surface-variant">Build: {{ systemData.buildDate }} | Runtime: {{ systemData.runtime }}</p>
            <p class="text-xs text-on-surface-variant">Hostname: {{ systemData.hostname }} | OS: {{ systemData.os }}</p>
          </div>
        </div>

        <!-- Stats grid -->
        <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
          <div class="bg-surface-container rounded-lg p-3">
            <p class="text-xs text-on-surface-variant">Uptime</p>
            <p class="text-sm font-bold text-on-surface">{{ systemData.uptime }}</p>
          </div>
          <div class="bg-surface-container rounded-lg p-3">
            <p class="text-xs text-on-surface-variant">CPU Usage</p>
            <p class="text-sm font-bold text-on-surface">{{ systemData.cpuUsage }}</p>
          </div>
          <div class="bg-surface-container rounded-lg p-3">
            <p class="text-xs text-on-surface-variant">Active Connections</p>
            <p class="text-sm font-bold text-on-surface">{{ systemData.activeConnections }}</p>
          </div>
          <div class="bg-surface-container rounded-lg p-3">
            <p class="text-xs text-on-surface-variant">Requests Today</p>
            <p class="text-sm font-bold text-on-surface">{{ systemData.requestsToday }}</p>
          </div>
        </div>

        <!-- Memory bar -->
        <div>
          <div class="flex items-center justify-between mb-1.5">
            <p class="text-sm font-medium text-on-surface">Memory</p>
            <p class="text-xs text-on-surface-variant">{{ systemData.memoryUsed }} / {{ systemData.memoryTotal }}</p>
          </div>
          <div class="w-full h-2.5 bg-outline-variant rounded-full overflow-hidden">
            <div class="h-full bg-gradient-to-r from-cyan-400 to-blue-500 rounded-full transition-all" :style="{ width: systemData.memoryPercent + '%' }" />
          </div>
        </div>

        <!-- Disk bar -->
        <div>
          <div class="flex items-center justify-between mb-1.5">
            <p class="text-sm font-medium text-on-surface">Disk</p>
            <p class="text-xs text-on-surface-variant">{{ systemData.diskUsed }} / {{ systemData.diskTotal }}</p>
          </div>
          <div class="w-full h-2.5 bg-outline-variant rounded-full overflow-hidden">
            <div class="h-full bg-gradient-to-r from-green-400 to-emerald-500 rounded-full transition-all" :style="{ width: systemData.diskPercent + '%' }" />
          </div>
        </div>
      </div>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="showSystemInfo = false">Dong</WxButton>
      </template>
    </WxModal>

    <!-- ===== DRAWER (SIDEBAR PANEL) ===== -->
    <WxDrawer v-model="showDrawer" title="Sidebar Panel" size="md">
      <div class="space-y-5">
        <p class="text-sm text-on-surface-variant">
          Day la panel truot tu ben phai, thuong dung de hien thi form nhap lieu hoac chi tiet nhanh.
        </p>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1.5">Ho va ten</label>
          <WxInput v-model="drawerForm.name" placeholder="Nguyen Van A" />
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1.5">Email</label>
          <WxInput v-model="drawerForm.email" type="email" placeholder="email@example.com" />
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1.5">Vai tro</label>
          <WxSelect v-model="drawerForm.role" :options="roleOptions" placeholder="Chon vai tro..." />
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1.5">Gioi thieu</label>
          <WxTextarea v-model="drawerForm.bio" placeholder="Viet vai dong gioi thieu..." :rows="3" />
        </div>
        <div class="flex items-center justify-between py-2">
          <div>
            <p class="text-sm font-medium text-on-surface">Thong bao email</p>
            <p class="text-xs text-on-surface-variant">Nhan thong bao qua email</p>
          </div>
          <WxSwitch v-model="drawerForm.notify" />
        </div>
      </div>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="showDrawer = false">Huy</WxButton>
        <WxButton variant="primary" size="sm" @click="handleDrawerSave">Luu</WxButton>
      </template>
    </WxDrawer>
  </div>
</template>
