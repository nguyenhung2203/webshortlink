<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxTextarea from '@/components/ui/WxTextarea.vue'
import WxFileUpload from '@/components/ui/WxFileUpload.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import {
  Inbox, Send, FileText, Trash2, AlertOctagon, Pencil,
  Bold, Italic, Underline, List, ChevronDown, ChevronUp,
} from 'lucide-vue-next'

const folders = [
  { label: 'Inbox', icon: Inbox, count: 13, active: false },
  { label: 'Sent', icon: Send, count: 0, active: false },
  { label: 'Drafts', icon: FileText, count: 2, active: false },
  { label: 'Trash', icon: Trash2, count: 0, active: false },
  { label: 'Junk', icon: AlertOctagon, count: 0, active: false },
]

const to = ref('')
const cc = ref('')
const bcc = ref('')
const subject = ref('')
const body = ref('')
const showCcBcc = ref(false)

const toolbarButtons = [
  { icon: Bold, label: 'Bold' },
  { icon: Italic, label: 'Italic' },
  { icon: Underline, label: 'Underline' },
  { icon: List, label: 'List' },
]

function handleUpload(files: File[]) {
  console.log('Attached files:', files)
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Compose" description="Write and send a new email message." />

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

      <!-- Compose Form -->
      <div class="lg:col-span-3">
        <WxCard>
          <div class="space-y-4">
            <!-- To -->
            <div class="flex items-center gap-3">
              <label class="text-sm font-medium text-on-surface-variant w-16 shrink-0">To</label>
              <div class="flex-1">
                <WxInput v-model="to" placeholder="recipient@example.com" type="email" />
              </div>
              <WxButton
                variant="ghost"
                size="sm"
                @click="showCcBcc = !showCcBcc"
              >
                CC/BCC
                <component :is="showCcBcc ? ChevronUp : ChevronDown" :size="14" class="ml-1" />
              </WxButton>
            </div>

            <!-- CC / BCC (collapsible) -->
            <template v-if="showCcBcc">
              <div class="flex items-center gap-3">
                <label class="text-sm font-medium text-on-surface-variant w-16 shrink-0">CC</label>
                <div class="flex-1">
                  <WxInput v-model="cc" placeholder="cc@example.com" type="email" />
                </div>
              </div>
              <div class="flex items-center gap-3">
                <label class="text-sm font-medium text-on-surface-variant w-16 shrink-0">BCC</label>
                <div class="flex-1">
                  <WxInput v-model="bcc" placeholder="bcc@example.com" type="email" />
                </div>
              </div>
            </template>

            <!-- Subject -->
            <div class="flex items-center gap-3">
              <label class="text-sm font-medium text-on-surface-variant w-16 shrink-0">Subject</label>
              <div class="flex-1">
                <WxInput v-model="subject" placeholder="Email subject" />
              </div>
            </div>

            <!-- Formatting Toolbar -->
            <div class="flex items-center gap-1 border border-outline-variant rounded-lg p-1 w-fit">
              <button
                v-for="btn in toolbarButtons"
                :key="btn.label"
                class="p-2 rounded-md text-on-surface-variant hover:bg-surface-container-high hover:text-on-surface transition-colors cursor-pointer"
                :title="btn.label"
              >
                <component :is="btn.icon" :size="16" />
              </button>
            </div>

            <!-- Body -->
            <WxTextarea v-model="body" placeholder="Write your message..." :rows="10" />

            <!-- Attachment -->
            <div>
              <p class="text-sm font-medium text-on-surface-variant mb-2">Attachments</p>
              <WxFileUpload accept="*" :multiple="true" :max-size="25" @upload="handleUpload" />
            </div>

            <!-- Actions -->
            <div class="flex items-center gap-3 pt-2 border-t border-outline-variant">
              <WxButton>
                <template #icon><Send :size="16" /></template>
                Send
              </WxButton>
              <WxButton variant="secondary">
                <template #icon><FileText :size="16" /></template>
                Save Draft
              </WxButton>
              <div class="flex-1" />
              <WxButton variant="danger" size="sm">
                <template #icon><Trash2 :size="16" /></template>
                Discard
              </WxButton>
            </div>
          </div>
        </WxCard>
      </div>
    </div>
  </div>
</template>
