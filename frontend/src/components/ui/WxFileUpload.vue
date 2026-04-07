<script setup lang="ts">
import { ref } from 'vue'
import { CloudUpload } from 'lucide-vue-next'

const props = withDefaults(defineProps<{
  accept?: string
  multiple?: boolean
  maxSize?: number
}>(), {
  accept: '*',
  multiple: false,
  maxSize: 10,
})

const emit = defineEmits<{
  upload: [files: File[]]
}>()

const fileInput = ref<HTMLInputElement | null>(null)
const isDragging = ref(false)
const error = ref('')

function triggerInput() {
  fileInput.value?.click()
}

function validateAndEmit(files: FileList | null) {
  if (!files || files.length === 0) return

  error.value = ''
  const valid: File[] = []
  const maxBytes = props.maxSize * 1024 * 1024

  for (const file of Array.from(files)) {
    if (file.size > maxBytes) {
      error.value = `"${file.name}" exceeds ${props.maxSize}MB limit`
      return
    }
    valid.push(file)
  }

  emit('upload', valid)

  // Reset input so same file can be re-selected
  if (fileInput.value) fileInput.value.value = ''
}

function onFileChange(e: Event) {
  const target = e.target as HTMLInputElement
  validateAndEmit(target.files)
}

function onDrop(e: DragEvent) {
  isDragging.value = false
  validateAndEmit(e.dataTransfer?.files ?? null)
}

function onDragOver() {
  isDragging.value = true
}

function onDragLeave() {
  isDragging.value = false
}
</script>

<template>
  <div>
    <div
      class="relative flex flex-col items-center justify-center gap-3 px-6 py-10 rounded-xl border-2 border-dashed transition-colors duration-200 cursor-pointer select-none"
      :class="isDragging
        ? 'border-primary bg-primary/5'
        : 'border-outline-variant bg-surface-container-lowest hover:border-on-surface-variant hover:bg-surface-container-low/30'"
      @click="triggerInput"
      @drop.prevent="onDrop"
      @dragover.prevent="onDragOver"
      @dragleave.prevent="onDragLeave"
    >
      <div class="w-12 h-12 rounded-full bg-surface-container-high/50 flex items-center justify-center">
        <CloudUpload
          class="w-6 h-6 transition-colors"
          :class="isDragging ? 'text-primary' : 'text-on-surface-variant'"
        />
      </div>
      <div class="text-center">
        <p class="text-sm font-medium text-on-surface">
          Drag files here or click to upload
        </p>
        <p class="text-xs text-on-surface-variant mt-1">
          Max {{ maxSize }}MB per file
        </p>
      </div>

      <input
        ref="fileInput"
        type="file"
        :accept="accept"
        :multiple="multiple"
        class="hidden"
        @change="onFileChange"
      />
    </div>

    <p v-if="error" class="text-danger text-xs mt-2">{{ error }}</p>
  </div>
</template>
