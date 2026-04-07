<script setup lang="ts">
import { ref } from 'vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxBreadcrumb from '@/components/ui/WxBreadcrumb.vue'
import WxStepper from '@/components/ui/WxStepper.vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxFileUpload from '@/components/ui/WxFileUpload.vue'
import WxDatePicker from '@/components/ui/WxDatePicker.vue'
import WxSlider from '@/components/ui/WxSlider.vue'
import WxTimeline from '@/components/ui/WxTimeline.vue'
import WxTreeView from '@/components/ui/WxTreeView.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxAlert from '@/components/ui/WxAlert.vue'
import WxProgressBar from '@/components/ui/WxProgressBar.vue'
import WxSkeleton from '@/components/ui/WxSkeleton.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'
import { useToast } from '@/composables/useToast'
import type { TreeNode } from '@/types'
import {
  Signpost,
  ListOrdered,
  PanelTop,
  Upload,
  CalendarRange,
  Clock,
  FolderTree,
  Bell,
  ShieldAlert,
  BarChart3,
  Loader,
  SearchX,
} from 'lucide-vue-next'

const toast = useToast()

// Stepper state
const currentStep = ref(1)
const steps = [
  { label: 'Thong tin', description: 'Nhap thong tin' },
  { label: 'Cau hinh', description: 'Cai dat' },
  { label: 'Xac nhan', description: 'Kiem tra' },
  { label: 'Hoan tat', description: 'Ket thuc' },
]

function nextStep() {
  if (currentStep.value < steps.length - 1) currentStep.value++
}
function prevStep() {
  if (currentStep.value > 0) currentStep.value--
}

// Breadcrumb items
const breadcrumbItems = [
  { label: 'Home', to: '/' },
  { label: 'Quan ly TKQC', to: '/' },
  { label: 'Campaign_A' },
]

// DatePicker & Slider state
const selectedDate = ref('')
const sliderValue = ref(40)

// Timeline items
const timelineItems = [
  { title: 'Dang nhap he thong', time: '08:45', color: 'info' },
  { title: 'Tao chien dich moi', time: '09:12', color: 'success' },
  { title: 'Cap nhat ngan sach', time: '10:30', color: 'warning' },
  { title: 'Xuat bao cao', time: '14:00', color: 'info' },
  { title: 'Xoa TKQC test', time: '16:45', color: 'danger' },
]

// Tree nodes
const treeNodes: TreeNode[] = [
  {
    id: 'bm-001',
    label: 'BM-001',
    children: [
      { id: 'camp-a', label: 'TKQC_Campaign_A' },
      { id: 'camp-b', label: 'TKQC_Campaign_B' },
    ],
  },
  {
    id: 'bm-002',
    label: 'BM-002',
    children: [
      { id: 'retarget-01', label: 'TKQC_Retarget_01' },
    ],
  },
  {
    id: 'bm-003',
    label: 'BM-003',
    children: [
      { id: 'lookalike-vn', label: 'TKQC_Lookalike_VN' },
      { id: 'brand-winter', label: 'TKQC_Brand_Winter' },
    ],
  },
]
</script>

<template>
  <div class="space-y-8 max-w-5xl mx-auto">
    <!-- Page title -->
    <div>
      <h1 class="text-2xl font-bold text-on-surface tracking-tight">Advanced Components</h1>
      <p class="mt-1 text-sm text-on-surface-variant">Navigation, upload, timeline, tree, and feedback components.</p>
    </div>

    <!-- 1. Breadcrumb -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <Signpost :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxBreadcrumb</h2>
      </div>
      <WxCard>
        <WxBreadcrumb :items="breadcrumbItems" />
      </WxCard>
    </section>

    <!-- 2. Stepper -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <ListOrdered :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxStepper</h2>
      </div>
      <WxCard>
        <div class="space-y-6">
          <WxStepper :steps="steps" :current-step="currentStep" />
          <div class="flex items-center justify-center gap-3">
            <WxButton variant="secondary" size="sm" :disabled="currentStep === 0" @click="prevStep">
              Previous
            </WxButton>
            <span class="text-xs text-on-surface-variant tabular-nums">
              Step {{ currentStep + 1 }} / {{ steps.length }}
            </span>
            <WxButton variant="primary" size="sm" :disabled="currentStep === steps.length - 1" @click="nextStep">
              Next
            </WxButton>
          </div>
        </div>
      </WxCard>
    </section>

    <!-- 3. PageHeader -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <PanelTop :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxPageHeader</h2>
      </div>
      <WxCard>
        <WxPageHeader
          title="Quan ly Chien dich"
          description="Tao va quan ly cac chien dich quang cao tren nen tang Wemake."
        >
          <template #breadcrumb>
            <WxBreadcrumb :items="[{ label: 'Home', to: '/' }, { label: 'Campaigns' }]" />
          </template>
          <template #actions>
            <WxButton variant="secondary" size="sm">Export</WxButton>
            <WxButton variant="primary" size="sm">+ New Campaign</WxButton>
          </template>
        </WxPageHeader>
      </WxCard>
    </section>

    <!-- 4. FileUpload -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <Upload :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxFileUpload</h2>
      </div>
      <WxCard>
        <WxFileUpload accept="image/*,.pdf" :multiple="true" :max-size="5" />
      </WxCard>
    </section>

    <!-- 5. DatePicker & Slider -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <CalendarRange :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxDatePicker &amp; WxSlider</h2>
      </div>
      <WxCard>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="space-y-2">
            <label class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider">Date Picker</label>
            <WxDatePicker v-model="selectedDate" placeholder="Chon ngay bat dau" />
            <p class="text-xs text-on-surface-variant">
              Selected: {{ selectedDate || 'None' }}
            </p>
          </div>
          <div class="space-y-2">
            <label class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider">Slider</label>
            <WxSlider v-model="sliderValue" :min="0" :max="100" :step="1" show-value />
          </div>
        </div>
      </WxCard>
    </section>

    <!-- 6. Timeline -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <Clock :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxTimeline</h2>
      </div>
      <WxCard>
        <WxTimeline :items="timelineItems" />
      </WxCard>
    </section>

    <!-- 7. TreeView -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <FolderTree :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxTreeView</h2>
      </div>
      <WxCard>
        <WxTreeView :nodes="treeNodes" />
      </WxCard>
    </section>

    <!-- 8. Toast -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <Bell :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxToast</h2>
      </div>
      <WxCard>
        <div class="flex flex-wrap gap-3">
          <WxButton variant="secondary" size="sm" @click="toast.info('Info', 'Bao cao dang duoc tao.')">
            Info Toast
          </WxButton>
          <WxButton variant="success" size="sm" @click="toast.success('Thanh cong', 'Chien dich da duoc luu.')">
            Success Toast
          </WxButton>
          <WxButton variant="secondary" size="sm" @click="toast.warning('Canh bao', 'API rate limit dat 80%.')">
            Warning Toast
          </WxButton>
          <WxButton variant="danger" size="sm" @click="toast.error('Loi', 'Khong the ket noi analytics server.')">
            Error Toast
          </WxButton>
        </div>
      </WxCard>
    </section>

    <!-- 9. Alert -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <ShieldAlert :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxAlert</h2>
      </div>
      <WxCard>
        <div class="space-y-3">
          <WxAlert type="info" title="Thong bao he thong">
            Bao tri he thong vao ngay 30/03/2026 tu 02:00 - 04:00 (ICT).
          </WxAlert>
          <WxAlert type="success" title="Chien dich da xuat ban">
            "Khuyen mai Tet" dang hoat dong va phan phoi den 3 khu vuc.
          </WxAlert>
          <WxAlert type="warning" title="Canh bao ngan sach">
            Chien dich "Flash Deal Weekend" da su dung 85% ngan sach trong ngay.
          </WxAlert>
          <WxAlert type="error" title="Thanh toan that bai">
            Khong the xu ly thanh toan cho hoa don #WM-2026-0342.
          </WxAlert>
        </div>
      </WxCard>
    </section>

    <!-- 10. ProgressBar -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <BarChart3 :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxProgressBar</h2>
      </div>
      <WxCard>
        <div class="space-y-5">
          <WxProgressBar :value="65" variant="primary" show-value>
            <span class="text-xs text-on-surface-variant">Upload</span>
          </WxProgressBar>
          <WxProgressBar :value="100" variant="success" show-value>
            <span class="text-xs text-on-surface-variant">Success</span>
          </WxProgressBar>
          <WxProgressBar :value="30" variant="danger" show-value>
            <span class="text-xs text-on-surface-variant">Error</span>
          </WxProgressBar>
          <WxProgressBar :value="50" variant="warning" show-value>
            <span class="text-xs text-on-surface-variant">Processing</span>
          </WxProgressBar>
        </div>
      </WxCard>
    </section>

    <!-- 11. Skeleton -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <Loader :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxSkeleton</h2>
      </div>
      <WxCard>
        <div class="space-y-6">
          <div class="flex items-center gap-4">
            <WxSkeleton variant="circular" width="3rem" />
            <div class="flex-1 space-y-2">
              <WxSkeleton variant="text" width="40%" />
              <WxSkeleton variant="text" width="70%" />
            </div>
          </div>
          <WxSkeleton variant="rectangular" height="6rem" />
          <WxSkeleton variant="text" :lines="3" />
        </div>
      </WxCard>
    </section>

    <!-- 12. EmptyState -->
    <section>
      <div class="flex items-center gap-2 mb-3">
        <SearchX :size="18" class="text-primary" />
        <h2 class="text-sm font-semibold text-primary">WxEmptyState</h2>
      </div>
      <WxCard>
        <WxEmptyState
          title="Khong tim thay chien dich"
          description="Thu dieu chinh bo loc tim kiem hoac tao chien dich moi de bat dau."
        >
          <template #icon><SearchX :size="48" :stroke-width="1.5" /></template>
          <template #action>
            <WxButton variant="primary" size="sm">Tao chien dich</WxButton>
          </template>
        </WxEmptyState>
      </WxCard>
    </section>
  </div>
</template>
