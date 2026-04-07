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
import WxSwitch from '@/components/ui/WxSwitch.vue'
import WxTabs from '@/components/ui/WxTabs.vue'
import WxSlider from '@/components/ui/WxSlider.vue'
import { useToast } from '@/composables/useToast'
import {
  Info, CheckCircle2, XCircle, AlertTriangle, Trash2, PanelRight,
  Settings, HelpCircle, FileText,
} from 'lucide-vue-next'

const toast = useToast()

// Message modals
const showInfo = ref(false)
const showSuccess = ref(false)
const showError = ref(false)
const showQuestion = ref(false)

// Confirm dialogs
const showDeleteConfirm = ref(false)
const showWarningConfirm = ref(false)

// Standard modal
const showBasicModal = ref(false)

// Sized modals
const showSmModal = ref(false)
const showLgModal = ref(false)
const showXlModal = ref(false)

// Tabbed config modal
const showConfigModal = ref(false)
const configTab = ref<string | number>('general')
const configTabs = [
  { key: 'general', label: 'General' },
  { key: 'notifications', label: 'Notifications' },
  { key: 'advanced', label: 'Advanced' },
]
const generalSettings = reactive({ siteName: 'My Application', language: 'en' as string | number | null, cacheTimeout: 60 })
const langOptions = [
  { label: 'English', value: 'en' },
  { label: 'Vietnamese', value: 'vi' },
  { label: 'Japanese', value: 'ja' },
]
const notifySettings = reactive({ emailAlerts: true, pushNotifs: false, weeklyDigest: true })

// Drawers
const showRightDrawer = ref(false)
const showLeftDrawer = ref(false)
const drawerForm = reactive({ name: '', email: '', notes: '' })

function handleDelete() { toast.success('Deleted', 'The item has been permanently removed.') }
function handleWarning() { toast.info('Acknowledged', 'Warning has been acknowledged.') }
function handleSaveConfig() { showConfigModal.value = false; toast.success('Saved', 'Settings updated.') }
function handleDrawerSave() { showRightDrawer.value = false; toast.success('Saved', 'Form data saved.') }
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Modals & Dialogs"
      description="Message modals, confirm dialogs, tabbed modals, sized modals, and drawer panels."
    />

    <!-- Message Modals -->
    <WxCard padding="lg">
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Message Modals</h3>
      </template>
      <p class="text-sm text-on-surface-variant mb-5">Click buttons to open different types of message dialogs.</p>
      <div class="flex flex-wrap gap-4">
        <WxButton variant="primary" @click="showInfo = true">
          <template #icon><Info :size="16" /></template>
          Message (Info)
        </WxButton>
        <WxButton variant="success" @click="showSuccess = true">
          <template #icon><CheckCircle2 :size="16" /></template>
          Message (Success)
        </WxButton>
        <WxButton variant="danger" @click="showError = true">
          <template #icon><XCircle :size="16" /></template>
          Message (Error)
        </WxButton>
        <WxButton variant="primary" @click="showQuestion = true">
          <template #icon><HelpCircle :size="16" /></template>
          Message (Question)
        </WxButton>
      </div>
    </WxCard>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Confirm Dialogs -->
      <WxCard padding="lg">
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">Confirm Dialogs</h3>
        </template>
        <p class="text-sm text-on-surface-variant mb-5">Confirmation prompts before destructive or important actions.</p>
        <div class="flex flex-wrap gap-4">
          <WxButton variant="danger" @click="showDeleteConfirm = true">
            <template #icon><Trash2 :size="16" /></template>
            Delete Confirm
          </WxButton>
          <WxButton variant="secondary" @click="showWarningConfirm = true">
            <template #icon><AlertTriangle :size="16" /></template>
            Warning Confirm
          </WxButton>
        </div>
      </WxCard>

      <!-- Tabbed Config Modal -->
      <WxCard padding="lg">
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">Tabbed Config Modal</h3>
        </template>
        <p class="text-sm text-on-surface-variant mb-5">Complex modal with tabbed content for settings and configuration.</p>
        <WxButton variant="primary" @click="showConfigModal = true">
          <template #icon><Settings :size="16" /></template>
          Open Settings
        </WxButton>
      </WxCard>
    </div>

    <!-- Modal Sizes -->
    <WxCard padding="lg">
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Modal Sizes</h3>
      </template>
      <p class="text-sm text-on-surface-variant mb-5">Modals come in 4 sizes to fit different content needs.</p>
      <div class="flex flex-wrap gap-4">
        <WxButton variant="secondary" @click="showSmModal = true">Small (max-w-md)</WxButton>
        <WxButton variant="secondary" @click="showBasicModal = true">Medium (max-w-lg)</WxButton>
        <WxButton variant="secondary" @click="showLgModal = true">Large (max-w-2xl)</WxButton>
        <WxButton variant="secondary" @click="showXlModal = true">Extra Large (max-w-4xl)</WxButton>
      </div>
    </WxCard>

    <!-- Drawers -->
    <WxCard padding="lg">
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Drawer Panels</h3>
      </template>
      <p class="text-sm text-on-surface-variant mb-5">Slide-in panels from left or right for editing without leaving context.</p>
      <div class="flex flex-wrap gap-4">
        <WxButton variant="secondary" @click="showRightDrawer = true">
          <template #icon><PanelRight :size="16" /></template>
          Right Drawer
        </WxButton>
        <WxButton variant="secondary" @click="showLeftDrawer = true">
          <template #icon><FileText :size="16" /></template>
          Left Drawer
        </WxButton>
      </div>
    </WxCard>

    <!-- All Modals -->
    <WxModal v-model="showInfo" title="Information" size="sm">
      <div class="flex flex-col items-center text-center gap-4">
        <div class="w-14 h-14 rounded-full bg-info/10 flex items-center justify-center">
          <Info class="w-7 h-7 text-info" />
        </div>
        <p class="text-sm text-on-surface-variant">The system has been updated to the latest version. All features are working normally.</p>
      </div>
      <template #footer><WxButton variant="primary" size="sm" @click="showInfo = false">OK</WxButton></template>
    </WxModal>

    <WxModal v-model="showSuccess" title="Success" size="sm">
      <div class="flex flex-col items-center text-center gap-4">
        <div class="w-14 h-14 rounded-full bg-success/10 flex items-center justify-center">
          <CheckCircle2 class="w-7 h-7 text-success" />
        </div>
        <p class="text-sm text-on-surface-variant">Your changes have been saved successfully and are now live in production.</p>
      </div>
      <template #footer><WxButton variant="success" size="sm" @click="showSuccess = false">Great</WxButton></template>
    </WxModal>

    <WxModal v-model="showError" title="Error" size="sm">
      <div class="flex flex-col items-center text-center gap-4">
        <div class="w-14 h-14 rounded-full bg-danger/10 flex items-center justify-center">
          <XCircle class="w-7 h-7 text-danger" />
        </div>
        <div>
          <p class="text-sm text-on-surface-variant">Unable to connect to the server. Please check your network connection.</p>
          <p class="text-xs text-danger/70 mt-2 font-mono">ERR_CONNECTION_TIMEOUT</p>
        </div>
      </div>
      <template #footer><WxButton variant="danger" size="sm" @click="showError = false">Close</WxButton></template>
    </WxModal>

    <WxModal v-model="showQuestion" title="Confirm Action" size="sm">
      <div class="flex flex-col items-center text-center gap-4">
        <div class="w-14 h-14 rounded-full bg-info/10 flex items-center justify-center">
          <HelpCircle class="w-7 h-7 text-info" />
        </div>
        <p class="text-sm text-on-surface-variant">This will change the current configuration. Are you sure you want to proceed?</p>
      </div>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="showQuestion = false">Cancel</WxButton>
        <WxButton variant="primary" size="sm" @click="showQuestion = false; toast.success('Confirmed', 'Action completed.')">Confirm</WxButton>
      </template>
    </WxModal>

    <WxConfirmDialog v-model="showDeleteConfirm" type="danger" title="Delete Item"
      message="Are you sure you want to delete this item? This action cannot be undone and all related data will be permanently removed."
      confirm-text="Delete" cancel-text="Cancel" @confirm="handleDelete" />

    <WxConfirmDialog v-model="showWarningConfirm" type="warning" title="Budget Warning"
      message="Campaign budget has reached 92%. The campaign will automatically pause within 24 hours if not adjusted."
      confirm-text="Understood" cancel-text="View Details" @confirm="handleWarning" />

    <WxModal v-model="showSmModal" title="Small Modal" size="sm">
      <p class="text-sm text-on-surface-variant">This is a small-sized modal, suitable for simple confirmations or brief messages.</p>
      <template #footer><WxButton variant="secondary" size="sm" @click="showSmModal = false">Close</WxButton></template>
    </WxModal>

    <WxModal v-model="showBasicModal" title="Medium Modal" size="md">
      <p class="text-sm text-on-surface-variant">This is the default medium-sized modal. It provides enough space for forms and detailed content while remaining focused.</p>
      <template #footer><WxButton variant="secondary" size="sm" @click="showBasicModal = false">Close</WxButton></template>
    </WxModal>

    <WxModal v-model="showLgModal" title="Large Modal" size="lg">
      <p class="text-sm text-on-surface-variant">A large modal is suitable for complex forms, data tables, or multi-step wizards that need more horizontal space.</p>
      <template #footer><WxButton variant="secondary" size="sm" @click="showLgModal = false">Close</WxButton></template>
    </WxModal>

    <WxModal v-model="showXlModal" title="Extra Large Modal" size="xl">
      <p class="text-sm text-on-surface-variant">The extra large modal provides maximum content area for dashboards, comparison views, or detailed reports embedded within an overlay.</p>
      <template #footer><WxButton variant="secondary" size="sm" @click="showXlModal = false">Close</WxButton></template>
    </WxModal>

    <!-- Config Modal -->
    <WxModal v-model="showConfigModal" title="Application Settings" size="lg">
      <WxTabs v-model="configTab" :items="configTabs">
        <template #default="{ activeKey }">
          <div v-if="activeKey === 'general'" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1.5">Site Name</label>
              <WxInput v-model="generalSettings.siteName" placeholder="Enter site name" />
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1.5">Language</label>
              <WxSelect v-model="generalSettings.language" :options="langOptions" />
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1.5">Cache Timeout (seconds)</label>
              <WxSlider v-model="generalSettings.cacheTimeout" :min="10" :max="600" :step="10" show-value />
            </div>
          </div>
          <div v-if="activeKey === 'notifications'" class="space-y-4">
            <div class="flex items-center justify-between py-3 border-b border-outline-variant">
              <div>
                <p class="text-sm font-medium text-on-surface">Email Alerts</p>
                <p class="text-xs text-on-surface-variant">Receive critical alerts via email</p>
              </div>
              <WxSwitch v-model="notifySettings.emailAlerts" />
            </div>
            <div class="flex items-center justify-between py-3 border-b border-outline-variant">
              <div>
                <p class="text-sm font-medium text-on-surface">Push Notifications</p>
                <p class="text-xs text-on-surface-variant">Browser push notifications</p>
              </div>
              <WxSwitch v-model="notifySettings.pushNotifs" />
            </div>
            <div class="flex items-center justify-between py-3">
              <div>
                <p class="text-sm font-medium text-on-surface">Weekly Digest</p>
                <p class="text-xs text-on-surface-variant">Summary email every Monday</p>
              </div>
              <WxSwitch v-model="notifySettings.weeklyDigest" />
            </div>
          </div>
          <div v-if="activeKey === 'advanced'" class="space-y-3">
            <p class="text-sm text-on-surface-variant">Advanced settings are for power users. Changes here may affect system stability.</p>
            <div class="bg-warning/5 border border-warning/20 rounded-lg p-3">
              <p class="text-xs text-warning font-medium">Proceed with caution when modifying advanced settings.</p>
            </div>
          </div>
        </template>
      </WxTabs>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="showConfigModal = false">Cancel</WxButton>
        <WxButton variant="primary" size="sm" @click="handleSaveConfig">Save Settings</WxButton>
      </template>
    </WxModal>

    <!-- Drawers -->
    <WxDrawer v-model="showRightDrawer" title="Edit Details" size="md">
      <div class="space-y-5">
        <p class="text-sm text-on-surface-variant">A right-side drawer panel for quick editing without leaving the current context.</p>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1.5">Full Name</label>
          <WxInput v-model="drawerForm.name" placeholder="John Doe" />
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1.5">Email</label>
          <WxInput v-model="drawerForm.email" type="email" placeholder="john@example.com" />
        </div>
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1.5">Notes</label>
          <WxTextarea v-model="drawerForm.notes" placeholder="Additional notes..." :rows="4" />
        </div>
      </div>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="showRightDrawer = false">Cancel</WxButton>
        <WxButton variant="primary" size="sm" @click="handleDrawerSave">Save</WxButton>
      </template>
    </WxDrawer>

    <WxDrawer v-model="showLeftDrawer" title="Navigation" side="left" size="sm">
      <nav class="space-y-1">
        <a v-for="item in ['Dashboard', 'Analytics', 'Reports', 'Users', 'Settings']" :key="item"
          class="block px-3 py-2 text-sm rounded-lg text-on-surface-variant hover:text-on-surface hover:bg-surface-container-high transition-colors cursor-pointer">
          {{ item }}
        </a>
      </nav>
    </WxDrawer>
  </div>
</template>
