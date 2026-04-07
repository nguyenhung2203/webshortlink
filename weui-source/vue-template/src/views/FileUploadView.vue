<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxFileUpload from '@/components/ui/WxFileUpload.vue'
import WxProgressBar from '@/components/ui/WxProgressBar.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { FileText, Image, FileSpreadsheet, Trash2 } from 'lucide-vue-next'
import { useToast } from '@/composables/useToast'

const toast = useToast()

const uploadedFiles = ref([
  { name: 'campaign-report-q1.pdf', size: '2.4 MB', progress: 100, type: 'pdf' },
  { name: 'banner-1200x628.png', size: '856 KB', progress: 100, type: 'image' },
  { name: 'audience-segments.xlsx', size: '1.1 MB', progress: 100, type: 'spreadsheet' },
  { name: 'creative-brief-tet.pdf', size: '540 KB', progress: 72, type: 'pdf' },
  { name: 'product-catalog.csv', size: '3.8 MB', progress: 45, type: 'spreadsheet' },
])

const fileIcons: Record<string, typeof FileText> = {
  pdf: FileText,
  image: Image,
  spreadsheet: FileSpreadsheet,
}

function handleUpload(files: File[]) {
  for (const file of files) {
    uploadedFiles.value.unshift({
      name: file.name,
      size: formatSize(file.size),
      progress: 0,
      type: getFileType(file.name),
    })
  }
  toast.success('Upload Started', `${files.length} file(s) added to queue.`)
}

function formatSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(0)} KB`
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`
}

function getFileType(name: string): string {
  const ext = name.split('.').pop()?.toLowerCase() ?? ''
  if (['png', 'jpg', 'jpeg', 'gif', 'webp', 'svg'].includes(ext)) return 'image'
  if (['xlsx', 'xls', 'csv'].includes(ext)) return 'spreadsheet'
  return 'pdf'
}

function removeFile(index: number) {
  uploadedFiles.value.splice(index, 1)
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="File Upload" description="Upload campaign assets, reports, and data files." />

    <WxCard>
      <template #header><h3 class="text-sm font-semibold text-on-surface">Upload Files</h3></template>
      <WxFileUpload
        accept=".pdf,.png,.jpg,.jpeg,.xlsx,.csv"
        :multiple="true"
        :max-size="10"
        @upload="handleUpload"
      />
    </WxCard>

    <WxCard>
      <template #header>
        <div class="flex items-center justify-between">
          <h3 class="text-sm font-semibold text-on-surface">Uploaded Files</h3>
          <span class="text-xs text-on-surface-variant">{{ uploadedFiles.length }} files</span>
        </div>
      </template>
      <div class="divide-y divide-outline-variant">
        <div
          v-for="(file, i) in uploadedFiles"
          :key="i"
          class="flex items-center gap-4 py-3 first:pt-0 last:pb-0"
        >
          <div class="w-10 h-10 rounded-lg bg-surface-container flex items-center justify-center shrink-0">
            <component :is="fileIcons[file.type] || FileText" :size="20" class="text-on-surface-variant" />
          </div>
          <div class="flex-1 min-w-0">
            <div class="flex items-center gap-2 mb-1">
              <p class="text-sm font-medium text-on-surface truncate">{{ file.name }}</p>
              <WxBadge v-if="file.progress === 100" variant="success" size="sm">Complete</WxBadge>
              <WxBadge v-else variant="info" size="sm">Uploading</WxBadge>
            </div>
            <div class="flex items-center gap-3">
              <span class="text-xs text-on-surface-variant shrink-0">{{ file.size }}</span>
              <div class="flex-1">
                <WxProgressBar
                  :value="file.progress"
                  :variant="file.progress === 100 ? 'success' : 'primary'"
                  size="sm"
                />
              </div>
            </div>
          </div>
          <button
            class="p-2 rounded-lg text-on-surface-variant hover:text-danger hover:bg-danger/10 transition-colors cursor-pointer shrink-0"
            @click="removeFile(i)"
          >
            <Trash2 :size="16" />
          </button>
        </div>
      </div>
    </WxCard>
  </div>
</template>
