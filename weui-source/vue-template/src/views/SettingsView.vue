<script setup lang="ts">
import { ref, reactive } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxTabs from '@/components/ui/WxTabs.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxSelect from '@/components/ui/WxSelect.vue'
import WxSwitch from '@/components/ui/WxSwitch.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxSlider from '@/components/ui/WxSlider.vue'
import { useToast } from '@/composables/useToast'
import {
  User,
  Bell,
  Shield,
  Settings,
  Download,
  Mail,
  Smartphone,
  MessageSquare,
  BarChart3,
  DollarSign,
  Users,
  Lock,
  Fingerprint,
  Eye,
  Key,
  Globe,
  Clock,
  Camera,
  ChevronDown,
  Zap,
  Database,
  Trash2,
  AlertTriangle,
} from 'lucide-vue-next'

const toast = useToast()

// ---- Tab state ----
const activeTab = ref<string | number>('profile')
const tabs = [
  { key: 'profile', label: 'Profile' },
  { key: 'notifications', label: 'Notifications' },
  { key: 'security', label: 'Security' },
  { key: 'advanced', label: 'Advanced' },
]

// ==============================
// PROFILE TAB
// ==============================
const profile = reactive({
  fullName: 'Nguyen Minh Tuan',
  email: 'tuan.nguyen@wemake.vn',
  phone: '+84 901 234 567',
  language: 'vi' as string | number | null,
  timezone: 'asia_hcm' as string | number | null,
  bio: 'Senior Marketing Manager with 5+ years of experience in digital advertising.',
})

const languageOptions = [
  { label: 'Tieng Viet', value: 'vi' },
  { label: 'English', value: 'en' },
  { label: 'Japanese', value: 'ja' },
  { label: 'Korean', value: 'ko' },
]

const timezoneOptions = [
  { label: 'Asia/Ho_Chi_Minh (UTC+7)', value: 'asia_hcm' },
  { label: 'Asia/Tokyo (UTC+9)', value: 'asia_tokyo' },
  { label: 'America/New_York (UTC-5)', value: 'us_east' },
  { label: 'Europe/London (UTC+0)', value: 'eu_london' },
  { label: 'Australia/Sydney (UTC+11)', value: 'au_sydney' },
]

function saveProfile() {
  toast.success('Saved', 'Profile information updated successfully.')
}

// ==============================
// NOTIFICATIONS TAB
// ==============================

// Auto-download section
const autoDownload = reactive({
  images: true,
  videos: false,
  documents: true,
  autoBackup: false,
})

// Notification channels
const notifChannels = reactive({
  email: true,
  push: true,
  sms: false,
  inApp: true,
})

// Notification types
const notifTypes = reactive({
  campaignAlerts: true,
  budgetAlerts: true,
  weeklyDigest: true,
  teamMentions: false,
  performanceReports: true,
  systemUpdates: false,
})

function saveNotifications() {
  toast.success('Saved', 'Notification preferences updated.')
}

// ==============================
// SECURITY TAB
// ==============================
const security = reactive({
  twoFactor: true,
  biometric: false,
  loginAlerts: true,
  sessionTimeout: true,
  ipWhitelist: false,
})

const passwords = reactive({
  current: '',
  newPass: '',
  confirm: '',
})

// Active sessions display mode
const sessionMode = ref<string | number | boolean>('all')

function savePassword() {
  if (!passwords.current || !passwords.newPass) {
    toast.error('Error', 'Please fill in all password fields.')
    return
  }
  if (passwords.newPass !== passwords.confirm) {
    toast.error('Error', 'New passwords do not match.')
    return
  }
  toast.success('Saved', 'Password changed successfully.')
  passwords.current = ''
  passwords.newPass = ''
  passwords.confirm = ''
}

function saveSecurity() {
  toast.success('Saved', 'Security settings updated.')
}

// ==============================
// ADVANCED TAB
// ==============================
const advanced = reactive({
  cacheSize: 50,
  dataRetention: 'months_6' as string | number | null,
  debugMode: false,
  analyticsTracking: true,
  experimentalFeatures: false,
  autoUpdate: true,
})

const dataRetentionOptions = [
  { label: '30 ngay', value: 'months_1' },
  { label: '3 thang', value: 'months_3' },
  { label: '6 thang', value: 'months_6' },
  { label: '1 nam', value: 'year_1' },
  { label: 'Vinh vien', value: 'forever' },
]

// Option cards for theme mode
const themeMode = ref<string | number | boolean>('system')

// Accordion/collapsible FAQ
const expandedFaq = ref<number | null>(null)
const faqItems = [
  {
    question: 'Lam the nao de xoa tai khoan?',
    answer: 'Ban co the yeu cau xoa tai khoan bang cach lien he bo phan ho tro qua email support@wemake.vn. Quy trinh xu ly mat khoang 7 ngay lam viec. Tat ca du lieu se duoc xoa vinh vien sau khi xac nhan.',
  },
  {
    question: 'Du lieu cua toi duoc bao mat nhu the nao?',
    answer: 'Chung toi su dung ma hoa AES-256 cho du lieu luu tru va TLS 1.3 cho du lieu truyen tai. He thong duoc kiem tra bao mat dinh ky boi cac cong ty doc lap. Du lieu duoc sao luu tai 3 trung tam du lieu khac nhau.',
  },
  {
    question: 'Toi co the xuat du lieu khong?',
    answer: 'Co, ban co the xuat toan bo du lieu tai khoan o dinh dang CSV, JSON hoac PDF. Truy cap muc Cai dat > Nang cao > Xuat du lieu de bat dau. Qua trinh xuat co the mat vai phut tuy thuoc vao luong du lieu.',
  },
  {
    question: 'Gioi han luu tru la bao nhieu?',
    answer: 'Goi mien phi co gioi han 5GB luu tru. Goi Pro co 50GB va goi Enterprise co luu tru khong gioi han. Ban co the nang cap goi bat ky luc nao tu trang Billing.',
  },
]

function toggleFaq(index: number) {
  expandedFaq.value = expandedFaq.value === index ? null : index
}

function saveAdvanced() {
  toast.success('Saved', 'Advanced settings updated.')
}

function clearCache() {
  toast.success('Cache cleared', 'Application cache has been cleared successfully.')
}

function exportData() {
  toast.success('Exporting', 'Your data export has started. You will receive an email when ready.')
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Settings" description="Manage your account preferences, notifications, security, and advanced configurations." />

    <WxCard>
      <WxTabs v-model="activeTab" :items="tabs">
        <template #default="{ activeKey }">

          <!-- ===================== PROFILE TAB ===================== -->
          <div v-if="activeKey === 'profile'" class="space-y-6">
            <!-- Avatar section -->
            <div class="flex items-center gap-5 p-5 bg-surface-container rounded-xl">
              <div class="relative">
                <WxAvatar name="Nguyen Minh Tuan" size="xl" status="online" />
                <button
                  class="absolute -bottom-1 -right-1 w-7 h-7 rounded-full bg-primary text-white flex items-center justify-center shadow-lg hover:brightness-110 transition-all cursor-pointer"
                >
                  <Camera class="w-3.5 h-3.5" />
                </button>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-on-surface">{{ profile.fullName }}</h3>
                <p class="text-sm text-on-surface-variant">{{ profile.email }}</p>
                <p class="text-xs text-on-surface-variant mt-1">Member since March 2024</p>
              </div>
            </div>

            <!-- Profile form -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
              <div>
                <label class="flex items-center gap-2 text-sm font-medium text-on-surface mb-1.5">
                  <User class="w-3.5 h-3.5 text-on-surface-variant" />
                  Full Name
                </label>
                <WxInput v-model="profile.fullName" placeholder="Your full name" />
              </div>
              <div>
                <label class="flex items-center gap-2 text-sm font-medium text-on-surface mb-1.5">
                  <Mail class="w-3.5 h-3.5 text-on-surface-variant" />
                  Email Address
                </label>
                <WxInput v-model="profile.email" type="email" placeholder="you@wemake.vn" />
              </div>
              <div>
                <label class="flex items-center gap-2 text-sm font-medium text-on-surface mb-1.5">
                  <Smartphone class="w-3.5 h-3.5 text-on-surface-variant" />
                  Phone Number
                </label>
                <WxInput v-model="profile.phone" placeholder="+84 xxx xxx xxx" />
              </div>
              <div>
                <label class="flex items-center gap-2 text-sm font-medium text-on-surface mb-1.5">
                  <Globe class="w-3.5 h-3.5 text-on-surface-variant" />
                  Language
                </label>
                <WxSelect v-model="profile.language" :options="languageOptions" />
              </div>
              <div>
                <label class="flex items-center gap-2 text-sm font-medium text-on-surface mb-1.5">
                  <Clock class="w-3.5 h-3.5 text-on-surface-variant" />
                  Timezone
                </label>
                <WxSelect v-model="profile.timezone" :options="timezoneOptions" />
              </div>
            </div>

            <!-- Action buttons -->
            <div class="flex justify-end gap-3 pt-2">
              <WxButton variant="secondary" size="sm">Cancel</WxButton>
              <WxButton variant="primary" size="sm" @click="saveProfile">Save Changes</WxButton>
            </div>
          </div>

          <!-- ===================== NOTIFICATIONS TAB ===================== -->
          <div v-if="activeKey === 'notifications'" class="space-y-6">

            <!-- Auto-download settings section -->
            <div>
              <div class="flex items-center gap-2 mb-4">
                <Download class="w-4 h-4 text-primary" />
                <h4 class="text-sm font-semibold text-on-surface">Auto-download</h4>
              </div>
              <div class="space-y-1 rounded-xl border border-outline-variant overflow-hidden">
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-info/10 flex items-center justify-center">
                      <Download class="w-4 h-4 text-blue-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Auto-download hinh anh</p>
                      <p class="text-xs text-on-surface-variant">Tu dong tai hinh anh khi nhan duoc</p>
                    </div>
                  </div>
                  <WxSwitch v-model="autoDownload.images" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-purple-500/10 flex items-center justify-center">
                      <Download class="w-4 h-4 text-purple-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Auto-download video</p>
                      <p class="text-xs text-on-surface-variant">Tu dong tai video (co the ton nhieu dung luong)</p>
                    </div>
                  </div>
                  <WxSwitch v-model="autoDownload.videos" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-success/10 flex items-center justify-center">
                      <Download class="w-4 h-4 text-green-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Auto-download tai lieu</p>
                      <p class="text-xs text-on-surface-variant">Tu dong tai file PDF, Word, Excel</p>
                    </div>
                  </div>
                  <WxSwitch v-model="autoDownload.documents" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-amber-500/10 flex items-center justify-center">
                      <Database class="w-4 h-4 text-amber-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Auto-backup</p>
                      <p class="text-xs text-on-surface-variant">Tu dong sao luu du lieu hang ngay</p>
                    </div>
                  </div>
                  <WxSwitch v-model="autoDownload.autoBackup" />
                </div>
              </div>
            </div>

            <!-- Notification channels -->
            <div>
              <div class="flex items-center gap-2 mb-4">
                <Bell class="w-4 h-4 text-primary" />
                <h4 class="text-sm font-semibold text-on-surface">Kenh thong bao</h4>
              </div>
              <div class="space-y-1 rounded-xl border border-outline-variant overflow-hidden">
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-info/10 flex items-center justify-center">
                      <Mail class="w-4 h-4 text-info" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Email Notifications</p>
                      <p class="text-xs text-on-surface-variant">Nhan thong bao qua email</p>
                    </div>
                  </div>
                  <WxSwitch v-model="notifChannels.email" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-success/10 flex items-center justify-center">
                      <Bell class="w-4 h-4 text-success" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Push Notifications</p>
                      <p class="text-xs text-on-surface-variant">Thong bao day tren trinh duyet</p>
                    </div>
                  </div>
                  <WxSwitch v-model="notifChannels.push" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-warning/10 flex items-center justify-center">
                      <Smartphone class="w-4 h-4 text-warning" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">SMS Notifications</p>
                      <p class="text-xs text-on-surface-variant">Canh bao quan trong qua tin nhan SMS</p>
                    </div>
                  </div>
                  <WxSwitch v-model="notifChannels.sms" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-primary/10 flex items-center justify-center">
                      <MessageSquare class="w-4 h-4 text-primary" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">In-app Notifications</p>
                      <p class="text-xs text-on-surface-variant">Hien thi thong bao trong ung dung</p>
                    </div>
                  </div>
                  <WxSwitch v-model="notifChannels.inApp" />
                </div>
              </div>
            </div>

            <!-- Notification types -->
            <div>
              <div class="flex items-center gap-2 mb-4">
                <Settings class="w-4 h-4 text-primary" />
                <h4 class="text-sm font-semibold text-on-surface">Loai thong bao</h4>
              </div>
              <div class="space-y-1 rounded-xl border border-outline-variant overflow-hidden">
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-danger/10 flex items-center justify-center">
                      <AlertTriangle class="w-4 h-4 text-danger" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Campaign Alerts</p>
                      <p class="text-xs text-on-surface-variant">Thong bao khi chien dich bat dau, tam dung hoac ket thuc</p>
                    </div>
                  </div>
                  <WxSwitch v-model="notifTypes.campaignAlerts" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-amber-500/10 flex items-center justify-center">
                      <DollarSign class="w-4 h-4 text-amber-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Budget Alerts</p>
                      <p class="text-xs text-on-surface-variant">Canh bao khi ngan sach dat 80% gioi han</p>
                    </div>
                  </div>
                  <WxSwitch v-model="notifTypes.budgetAlerts" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-info/10 flex items-center justify-center">
                      <BarChart3 class="w-4 h-4 text-blue-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Weekly Digest</p>
                      <p class="text-xs text-on-surface-variant">Bao cao hieu suat hang tuan vao thu Hai</p>
                    </div>
                  </div>
                  <WxSwitch v-model="notifTypes.weeklyDigest" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-purple-500/10 flex items-center justify-center">
                      <Users class="w-4 h-4 text-purple-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Team Mentions</p>
                      <p class="text-xs text-on-surface-variant">Thong bao khi ai do nhac den ban</p>
                    </div>
                  </div>
                  <WxSwitch v-model="notifTypes.teamMentions" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-success/10 flex items-center justify-center">
                      <BarChart3 class="w-4 h-4 text-green-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Performance Reports</p>
                      <p class="text-xs text-on-surface-variant">Bao cao hieu suat chien dich chi tiet</p>
                    </div>
                  </div>
                  <WxSwitch v-model="notifTypes.performanceReports" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-cyan-500/10 flex items-center justify-center">
                      <Settings class="w-4 h-4 text-cyan-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">System Updates</p>
                      <p class="text-xs text-on-surface-variant">Thong bao ve cap nhat va bao tri he thong</p>
                    </div>
                  </div>
                  <WxSwitch v-model="notifTypes.systemUpdates" />
                </div>
              </div>
            </div>

            <div class="flex justify-end gap-3 pt-2">
              <WxButton variant="secondary" size="sm">Cancel</WxButton>
              <WxButton variant="primary" size="sm" @click="saveNotifications">Save Preferences</WxButton>
            </div>
          </div>

          <!-- ===================== SECURITY TAB ===================== -->
          <div v-if="activeKey === 'security'" class="space-y-6">

            <!-- Security toggle rows -->
            <div>
              <div class="flex items-center gap-2 mb-4">
                <Shield class="w-4 h-4 text-primary" />
                <h4 class="text-sm font-semibold text-on-surface">Cai dat bao mat</h4>
              </div>
              <div class="space-y-1 rounded-xl border border-outline-variant overflow-hidden">
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-primary/10 flex items-center justify-center">
                      <Key class="w-4 h-4 text-primary" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Xac thuc hai yeu to (2FA)</p>
                      <p class="text-xs text-on-surface-variant">Them lop bao mat khi dang nhap</p>
                    </div>
                  </div>
                  <WxSwitch v-model="security.twoFactor" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-success/10 flex items-center justify-center">
                      <Fingerprint class="w-4 h-4 text-success" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Dang nhap sinh trac hoc</p>
                      <p class="text-xs text-on-surface-variant">Su dung van tay hoac khuon mat de dang nhap</p>
                    </div>
                  </div>
                  <WxSwitch v-model="security.biometric" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-warning/10 flex items-center justify-center">
                      <Eye class="w-4 h-4 text-warning" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Canh bao dang nhap</p>
                      <p class="text-xs text-on-surface-variant">Thong bao khi co dang nhap tu thiet bi moi</p>
                    </div>
                  </div>
                  <WxSwitch v-model="security.loginAlerts" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-info/10 flex items-center justify-center">
                      <Clock class="w-4 h-4 text-info" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Tu dong het phien</p>
                      <p class="text-xs text-on-surface-variant">Dang xuat sau 30 phut khong hoat dong</p>
                    </div>
                  </div>
                  <WxSwitch v-model="security.sessionTimeout" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-danger/10 flex items-center justify-center">
                      <Lock class="w-4 h-4 text-danger" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">IP Whitelist</p>
                      <p class="text-xs text-on-surface-variant">Chi cho phep truy cap tu cac IP da dang ky</p>
                    </div>
                  </div>
                  <WxSwitch v-model="security.ipWhitelist" />
                </div>
              </div>
            </div>

            <!-- Option Cards: Session mode -->
            <div>
              <div class="flex items-center gap-2 mb-4">
                <Users class="w-4 h-4 text-primary" />
                <h4 class="text-sm font-semibold text-on-surface">Quan ly phien dang nhap</h4>
              </div>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
                <button
                  v-for="option in [
                    { value: 'all', title: 'Tat ca phien', desc: 'Hien thi toan bo phien dang hoat dong' },
                    { value: 'active', title: 'Phien hien tai', desc: 'Chi hien thi phien dang su dung' },
                    { value: 'recent', title: 'Gan day', desc: 'Phien dang nhap trong 7 ngay qua' },
                  ]"
                  :key="String(option.value)"
                  class="relative flex items-start gap-3 p-4 rounded-xl border-2 text-left transition-all cursor-pointer"
                  :class="sessionMode === option.value
                    ? 'border-primary bg-primary/5'
                    : 'border-outline-variant bg-surface-container/30 hover:border-outline hover:bg-surface-container/50'"
                  @click="sessionMode = option.value"
                >
                  <span
                    class="mt-0.5 flex items-center justify-center w-[18px] h-[18px] rounded-full border-2 shrink-0 transition-colors"
                    :class="sessionMode === option.value ? 'border-primary' : 'border-outline-variant'"
                  >
                    <span
                      v-if="sessionMode === option.value"
                      class="w-2.5 h-2.5 rounded-full bg-primary"
                    />
                  </span>
                  <div>
                    <p class="text-sm font-medium text-on-surface">{{ option.title }}</p>
                    <p class="text-xs text-on-surface-variant mt-0.5">{{ option.desc }}</p>
                  </div>
                </button>
              </div>
            </div>

            <!-- Change password -->
            <div>
              <div class="flex items-center gap-2 mb-4">
                <Lock class="w-4 h-4 text-primary" />
                <h4 class="text-sm font-semibold text-on-surface">Doi mat khau</h4>
              </div>
              <div class="space-y-4 max-w-md">
                <div>
                  <label class="block text-sm font-medium text-on-surface mb-1.5">Mat khau hien tai</label>
                  <WxPasswordInput v-model="passwords.current" placeholder="Nhap mat khau hien tai" />
                </div>
                <div>
                  <label class="block text-sm font-medium text-on-surface mb-1.5">Mat khau moi</label>
                  <WxPasswordInput v-model="passwords.newPass" placeholder="Nhap mat khau moi" />
                </div>
                <div>
                  <label class="block text-sm font-medium text-on-surface mb-1.5">Xac nhan mat khau moi</label>
                  <WxPasswordInput v-model="passwords.confirm" placeholder="Nhap lai mat khau moi" />
                </div>
              </div>
            </div>

            <div class="flex justify-end gap-3 pt-2">
              <WxButton variant="secondary" size="sm" @click="savePassword">Doi mat khau</WxButton>
              <WxButton variant="primary" size="sm" @click="saveSecurity">Luu cai dat</WxButton>
            </div>
          </div>

          <!-- ===================== ADVANCED TAB ===================== -->
          <div v-if="activeKey === 'advanced'" class="space-y-6">

            <!-- Option cards: Theme mode -->
            <div>
              <div class="flex items-center gap-2 mb-4">
                <Zap class="w-4 h-4 text-primary" />
                <h4 class="text-sm font-semibold text-on-surface">Che do giao dien</h4>
              </div>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
                <button
                  v-for="option in [
                    { value: 'light', title: 'Sang', desc: 'Giao dien sang phu hop lam viec ban ngay' },
                    { value: 'dark', title: 'Toi', desc: 'Giao dien toi giam moi mat ban dem' },
                    { value: 'system', title: 'He thong', desc: 'Tu dong theo cai dat he dieu hanh' },
                  ]"
                  :key="String(option.value)"
                  class="relative flex items-start gap-3 p-4 rounded-xl border-2 text-left transition-all cursor-pointer"
                  :class="themeMode === option.value
                    ? 'border-primary bg-primary/5'
                    : 'border-outline-variant bg-surface-container/30 hover:border-outline hover:bg-surface-container/50'"
                  @click="themeMode = option.value"
                >
                  <span
                    class="mt-0.5 flex items-center justify-center w-[18px] h-[18px] rounded-full border-2 shrink-0 transition-colors"
                    :class="themeMode === option.value ? 'border-primary' : 'border-outline-variant'"
                  >
                    <span
                      v-if="themeMode === option.value"
                      class="w-2.5 h-2.5 rounded-full bg-primary"
                    />
                  </span>
                  <div>
                    <p class="text-sm font-medium text-on-surface">{{ option.title }}</p>
                    <p class="text-xs text-on-surface-variant mt-0.5">{{ option.desc }}</p>
                  </div>
                </button>
              </div>
            </div>

            <!-- Advanced toggle settings -->
            <div>
              <div class="flex items-center gap-2 mb-4">
                <Settings class="w-4 h-4 text-primary" />
                <h4 class="text-sm font-semibold text-on-surface">Cai dat nang cao</h4>
              </div>
              <div class="space-y-1 rounded-xl border border-outline-variant overflow-hidden">
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-amber-500/10 flex items-center justify-center">
                      <Zap class="w-4 h-4 text-amber-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Che do debug</p>
                      <p class="text-xs text-on-surface-variant">Ghi log chi tiet cho muc dich kiem thu</p>
                    </div>
                  </div>
                  <WxSwitch v-model="advanced.debugMode" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-info/10 flex items-center justify-center">
                      <BarChart3 class="w-4 h-4 text-blue-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Analytics Tracking</p>
                      <p class="text-xs text-on-surface-variant">Thu thap du lieu su dung de cai thien san pham</p>
                    </div>
                  </div>
                  <WxSwitch v-model="advanced.analyticsTracking" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-purple-500/10 flex items-center justify-center">
                      <Zap class="w-4 h-4 text-purple-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Tinh nang thu nghiem</p>
                      <p class="text-xs text-on-surface-variant">Bat tinh nang dang phat trien (co the khong on dinh)</p>
                    </div>
                  </div>
                  <WxSwitch v-model="advanced.experimentalFeatures" />
                </div>
                <div class="flex items-center justify-between px-4 py-3 bg-surface-container/50">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-lg bg-success/10 flex items-center justify-center">
                      <Download class="w-4 h-4 text-green-500" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-on-surface">Tu dong cap nhat</p>
                      <p class="text-xs text-on-surface-variant">Tu dong tai va cai dat ban cap nhat moi</p>
                    </div>
                  </div>
                  <WxSwitch v-model="advanced.autoUpdate" />
                </div>
              </div>
            </div>

            <!-- Sliders for cache and data retention -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
              <div>
                <label class="block text-sm font-medium text-on-surface mb-1.5">
                  Kich thuoc cache: <span class="text-primary font-semibold">{{ advanced.cacheSize }} MB</span>
                </label>
                <WxSlider v-model="advanced.cacheSize" :min="10" :max="500" :step="10" show-value />
              </div>
              <div>
                <label class="block text-sm font-medium text-on-surface mb-1.5">Thoi gian luu tru du lieu</label>
                <WxSelect v-model="advanced.dataRetention" :options="dataRetentionOptions" />
              </div>
            </div>

            <!-- Action buttons row -->
            <div class="flex flex-wrap gap-3 p-4 bg-surface-container rounded-xl">
              <WxButton variant="secondary" size="sm" @click="clearCache">
                <template #icon><Database class="w-3.5 h-3.5" /></template>
                Xoa cache
              </WxButton>
              <WxButton variant="secondary" size="sm" @click="exportData">
                <template #icon><Download class="w-3.5 h-3.5" /></template>
                Xuat du lieu
              </WxButton>
              <WxButton variant="danger" size="sm">
                <template #icon><Trash2 class="w-3.5 h-3.5" /></template>
                Xoa tai khoan
              </WxButton>
            </div>

            <!-- FAQ Accordion -->
            <div>
              <div class="flex items-center gap-2 mb-4">
                <MessageSquare class="w-4 h-4 text-primary" />
                <h4 class="text-sm font-semibold text-on-surface">Cau hoi thuong gap</h4>
              </div>
              <div class="rounded-xl border border-outline-variant overflow-hidden divide-y divide-outline-variant">
                <div v-for="(faq, index) in faqItems" :key="index">
                  <button
                    class="w-full flex items-center justify-between px-4 py-3 text-left hover:bg-surface-container/50 transition-colors cursor-pointer"
                    @click="toggleFaq(index)"
                  >
                    <span class="text-sm font-medium text-on-surface pr-4">{{ faq.question }}</span>
                    <ChevronDown
                      class="w-4 h-4 text-on-surface-variant shrink-0 transition-transform duration-200"
                      :class="expandedFaq === index && 'rotate-180'"
                    />
                  </button>
                  <div
                    v-if="expandedFaq === index"
                    class="px-4 pb-4"
                  >
                    <p class="text-sm text-on-surface-variant leading-relaxed">{{ faq.answer }}</p>
                  </div>
                </div>
              </div>
            </div>

            <div class="flex justify-end gap-3 pt-2">
              <WxButton variant="secondary" size="sm">Cancel</WxButton>
              <WxButton variant="primary" size="sm" @click="saveAdvanced">Save Settings</WxButton>
            </div>
          </div>

        </template>
      </WxTabs>
    </WxCard>
  </div>
</template>
