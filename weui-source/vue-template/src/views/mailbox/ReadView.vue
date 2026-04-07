<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxTextarea from '@/components/ui/WxTextarea.vue'
import {
  Inbox, Send, FileText, Trash2, AlertOctagon, Pencil,
  Reply, Forward, Printer, Download, File, FileSpreadsheet,
} from 'lucide-vue-next'

const folders = [
  { label: 'Inbox', icon: Inbox, count: 13, active: true },
  { label: 'Sent', icon: Send, count: 0, active: false },
  { label: 'Drafts', icon: FileText, count: 2, active: false },
  { label: 'Trash', icon: Trash2, count: 0, active: false },
  { label: 'Junk', icon: AlertOctagon, count: 0, active: false },
]

const email = {
  sender: 'Nguyen Van Minh',
  email: 'minh.nguyen@techcorp.vn',
  date: '27 March 2026, 09:15 AM',
  subject: 'Bao cao doanh thu Q1 2026 - Can duyet truoc thu 6',
  body: [
    'Kinh gui anh/chi, em xin gui bao cao tong hop doanh thu Quy 1 nam 2026 cua bo phan kinh doanh. Tong doanh thu dat 4.8 ty VND, tang 18% so voi cung ky nam truoc. Cac chi tieu KPI co ban deu dat va vuot muc tieu de ra. Dac biet, mang san pham moi dong gop 32% vao tong doanh thu, cho thay chien luoc mo rong thi truong dang phat huy hieu qua.',
    'De nghi anh/chi xem xet va phe duyet bao cao truoc cuoi ngay thu Sau (28/03) de em co the trinh len Ban Giam doc trong cuoc hop dau tuan toi. Neu co bat ky dieu chinh hoac bo sung nao, xin anh/chi phan hoi som de em kip cap nhat. Tran trong cam on anh/chi da ho tro.',
  ],
  attachments: [
    { name: 'Bao_cao_Q1_2026.xlsx', size: '2.4 MB', icon: FileSpreadsheet },
    { name: 'Bieu_do_doanh_thu.pdf', size: '1.1 MB', icon: File },
  ],
}

const replyText = ref('')
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Read Message" description="View email details, attachments, and reply." />

    <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
      <!-- Sidebar -->
      <div class="lg:col-span-1 space-y-3">
        <WxButton class="w-full">
          <template #icon><Pencil :size="16" /></template>
          Compose
        </WxButton>

        <WxCard padding="sm">
          <nav class="space-y-0.5">
            <a
              v-for="f in folders"
              :key="f.label"
              href="#"
              class="flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition-colors"
              :class="f.active
                ? 'bg-primary/10 text-primary font-medium'
                : 'text-on-surface-variant hover:bg-surface-container-high'"
              @click.prevent
            >
              <component :is="f.icon" :size="16" />
              <span class="flex-1">{{ f.label }}</span>
              <WxBadge v-if="f.count" variant="neutral" size="sm">{{ f.count }}</WxBadge>
            </a>
          </nav>
        </WxCard>
      </div>

      <!-- Email Detail -->
      <div class="lg:col-span-3 space-y-5">
        <WxCard>
          <!-- Email Header -->
          <div class="flex items-start gap-4 mb-5">
            <WxAvatar :name="email.sender" size="lg" />
            <div class="flex-1 min-w-0">
              <div class="flex items-start justify-between gap-4 flex-wrap">
                <div>
                  <h3 class="text-base font-semibold text-on-surface">{{ email.sender }}</h3>
                  <p class="text-xs text-on-surface-variant">{{ email.email }}</p>
                </div>
                <span class="text-xs text-on-surface-variant whitespace-nowrap">{{ email.date }}</span>
              </div>
              <h2 class="text-lg font-bold text-on-surface mt-2">{{ email.subject }}</h2>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex items-center gap-2 mb-5 pb-5 border-b border-outline-variant">
            <WxButton variant="secondary" size="sm">
              <template #icon><Reply :size="14" /></template>
              Reply
            </WxButton>
            <WxButton variant="secondary" size="sm">
              <template #icon><Forward :size="14" /></template>
              Forward
            </WxButton>
            <WxButton variant="ghost" size="sm">
              <template #icon><Printer :size="14" /></template>
              Print
            </WxButton>
            <div class="flex-1" />
            <WxButton variant="danger" size="sm">
              <template #icon><Trash2 :size="14" /></template>
              Delete
            </WxButton>
          </div>

          <!-- Email Body -->
          <div class="space-y-4 text-sm text-on-surface leading-relaxed">
            <p v-for="(para, idx) in email.body" :key="idx">{{ para }}</p>
          </div>

          <!-- Attachments -->
          <div class="mt-6 pt-5 border-t border-outline-variant">
            <h4 class="text-sm font-semibold text-on-surface mb-3">
              Attachments ({{ email.attachments.length }})
            </h4>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
              <div
                v-for="att in email.attachments"
                :key="att.name"
                class="flex items-center gap-3 p-3 rounded-lg border border-outline-variant bg-surface-container-low/30 hover:bg-surface-container-high/50 transition-colors"
              >
                <div class="w-10 h-10 rounded-lg bg-primary/10 flex items-center justify-center shrink-0">
                  <component :is="att.icon" :size="20" class="text-primary" />
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm font-medium text-on-surface truncate">{{ att.name }}</p>
                  <p class="text-xs text-on-surface-variant">{{ att.size }}</p>
                </div>
                <WxButton variant="ghost" size="sm">
                  <Download :size="14" />
                </WxButton>
              </div>
            </div>
          </div>
        </WxCard>

        <!-- Reply Box -->
        <WxCard>
          <template #header>
            <h4 class="text-sm font-semibold text-on-surface">Quick Reply</h4>
          </template>
          <div class="space-y-3">
            <WxTextarea v-model="replyText" placeholder="Type your reply..." :rows="4" />
            <div class="flex items-center gap-3">
              <WxButton size="sm">
                <template #icon><Send :size="14" /></template>
                Send Reply
              </WxButton>
              <WxButton variant="secondary" size="sm">Discard</WxButton>
            </div>
          </div>
        </WxCard>
      </div>
    </div>
  </div>
</template>
