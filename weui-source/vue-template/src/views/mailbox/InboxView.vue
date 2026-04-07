<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxCheckbox from '@/components/ui/WxCheckbox.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import {
  Inbox, Send, FileText, Trash2, AlertOctagon,
  RefreshCw, Filter, Star, Pencil,
  ChevronLeft, ChevronRight,
} from 'lucide-vue-next'

interface Email {
  id: number
  sender: string
  subject: string
  preview: string
  date: string
  read: boolean
  starred: boolean
  selected: boolean
}

const folders = [
  { label: 'Inbox', icon: Inbox, count: 13, active: true },
  { label: 'Sent', icon: Send, count: 0, active: false },
  { label: 'Drafts', icon: FileText, count: 2, active: false },
  { label: 'Trash', icon: Trash2, count: 0, active: false },
  { label: 'Junk', icon: AlertOctagon, count: 0, active: false },
]

const emails = ref<Email[]>([
  { id: 1, sender: 'Nguyen Van Minh', subject: 'Bao cao doanh thu Q1 2026 - Can duyet truoc thu 6', preview: 'Chao anh, em gui bao cao doanh thu quy 1...', date: 'Mar 27', read: false, starred: true, selected: false },
  { id: 2, sender: 'Tran Thi Lan Anh', subject: 'Re: Hop dong doi tac chien luoc - ban cap nhat', preview: 'Da cap nhat cac dieu khoan theo yeu cau...', date: 'Mar 27', read: false, starred: false, selected: false },
  { id: 3, sender: 'Le Quoc Hung', subject: 'Lich hop tuan toi - thay doi phong hop', preview: 'Phong hop A3 dang bao tri, chuyen sang B2...', date: 'Mar 26', read: true, starred: false, selected: false },
  { id: 4, sender: 'Pham Ngoc Bich', subject: 'De xuat ngan sach Marketing Q2', preview: 'Gui anh chi ban de xuat chi tiet...', date: 'Mar 26', read: true, starred: true, selected: false },
  { id: 5, sender: 'Vo Thanh Son', subject: 'Cap nhat tien do du an ERP - Sprint 14', preview: 'Sprint 14 hoan thanh 85%, con 3 task...', date: 'Mar 25', read: true, starred: false, selected: false },
  { id: 6, sender: 'Hoang Mai Linh', subject: 'Tuyen dung: Ung vien Senior Developer', preview: 'Co 3 ung vien tiem nang can phong van...', date: 'Mar 25', read: false, starred: false, selected: false },
  { id: 7, sender: 'Do Minh Quang', subject: 'Bao gia server moi - AWS vs Azure', preview: 'Sau khi so sanh 2 nha cung cap...', date: 'Mar 24', read: true, starred: false, selected: false },
  { id: 8, sender: 'Bui Thi Hong Nhung', subject: 'Team building thang 4 - Khao sat dia diem', preview: 'Moi anh chi dien khao sat chon dia diem...', date: 'Mar 24', read: true, starred: true, selected: false },
])

const selectAll = ref(false)

function toggleSelectAll() {
  selectAll.value = !selectAll.value
  emails.value.forEach(e => (e.selected = selectAll.value))
}

function toggleStar(email: Email) {
  email.starred = !email.starred
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Inbox" description="Manage your email messages and conversations." />

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

      <!-- Email List -->
      <div class="lg:col-span-3">
        <WxCard padding="none">
          <!-- Toolbar -->
          <div class="flex items-center justify-between px-4 py-2.5 border-b border-outline-variant">
            <div class="flex items-center gap-3">
              <WxCheckbox :model-value="selectAll" @update:model-value="toggleSelectAll" />
              <WxButton variant="ghost" size="sm"><RefreshCw :size="14" /></WxButton>
              <WxButton variant="ghost" size="sm"><Trash2 :size="14" /></WxButton>
              <WxButton variant="ghost" size="sm"><Filter :size="14" /></WxButton>
            </div>
            <div class="flex items-center gap-2 text-xs text-on-surface-variant">
              <span>1-8 of 13</span>
              <WxButton variant="ghost" size="sm"><ChevronLeft :size="14" /></WxButton>
              <WxButton variant="ghost" size="sm"><ChevronRight :size="14" /></WxButton>
            </div>
          </div>

          <!-- Email Rows -->
          <div class="divide-y divide-outline-variant">
            <div
              v-for="email in emails"
              :key="email.id"
              class="flex items-center gap-3 px-4 py-3 hover:bg-surface-container-high/50 transition-colors cursor-pointer"
              :class="!email.read ? 'bg-primary/[0.03]' : ''"
            >
              <WxCheckbox v-model="email.selected" />
              <button
                class="shrink-0 transition-colors cursor-pointer"
                :class="email.starred ? 'text-amber-400' : 'text-on-surface-variant/30 hover:text-amber-400'"
                @click.stop="toggleStar(email)"
              >
                <Star :size="16" :fill="email.starred ? 'currentColor' : 'none'" />
              </button>
              <span
                class="w-36 shrink-0 text-sm truncate"
                :class="!email.read ? 'font-semibold text-on-surface' : 'text-on-surface-variant'"
              >
                {{ email.sender }}
              </span>
              <div class="flex-1 min-w-0">
                <span
                  class="text-sm truncate"
                  :class="!email.read ? 'font-semibold text-on-surface' : 'text-on-surface'"
                >
                  {{ email.subject }}
                </span>
                <span class="text-sm text-on-surface-variant"> - {{ email.preview }}</span>
              </div>
              <span class="shrink-0 text-xs text-on-surface-variant whitespace-nowrap">{{ email.date }}</span>
            </div>
          </div>
        </WxCard>
      </div>
    </div>
  </div>
</template>
