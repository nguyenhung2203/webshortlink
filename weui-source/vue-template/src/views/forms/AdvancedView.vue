<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  Calendar, Tags, Palette, Upload, Phone, DollarSign,
  SlidersHorizontal, ChevronRight, ChevronLeft, Check,
} from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxDatePicker from '@/components/ui/WxDatePicker.vue'
import WxMultiSelect from '@/components/ui/WxMultiSelect.vue'
import WxFileUpload from '@/components/ui/WxFileUpload.vue'
import WxSlider from '@/components/ui/WxSlider.vue'
import WxStepper from '@/components/ui/WxStepper.vue'
import WxTextarea from '@/components/ui/WxTextarea.vue'
import WxSelect from '@/components/ui/WxSelect.vue'

// Date Pickers
const startDate = ref('')
const endDate = ref('')

// Multi Select with tags
const selectedTags = ref<(string | number)[]>(['vue', 'typescript'])
const tagOptions = [
  { label: 'Vue.js', value: 'vue' },
  { label: 'TypeScript', value: 'typescript' },
  { label: 'Tailwind CSS', value: 'tailwind' },
  { label: 'React', value: 'react' },
  { label: 'Node.js', value: 'node' },
  { label: 'Python', value: 'python' },
]

// Color picker
const colors = ['#3B82F6', '#EF4444', '#10B981', '#F59E0B', '#8B5CF6', '#EC4899']
const selectedColor = ref('#3B82F6')

// File upload
const uploadedFiles = ref<string[]>([])
function handleUpload(files: File[]) {
  uploadedFiles.value = files.map(f => f.name)
}

// Input masks
const phoneVal = ref('')
const currencyVal = ref('')

function formatPhone(e: Event) {
  const input = e.target as HTMLInputElement
  let val = input.value.replace(/\D/g, '').slice(0, 10)
  if (val.length > 6) val = `(${val.slice(0, 3)}) ${val.slice(3, 6)}-${val.slice(6)}`
  else if (val.length > 3) val = `(${val.slice(0, 3)}) ${val.slice(3)}`
  else if (val.length > 0) val = `(${val}`
  phoneVal.value = val
}

function formatCurrency(e: Event) {
  const input = e.target as HTMLInputElement
  const num = input.value.replace(/[^\d]/g, '')
  currencyVal.value = num ? `$${Number(num).toLocaleString()}` : ''
}

// Range sliders
const rangeA = ref(30)
const rangeB = ref(70)
const brightness = ref(50)

// Stepper
const currentStep = ref(0)
const steps = [
  { label: 'Account', description: 'Basic info' },
  { label: 'Details', description: 'Additional details' },
  { label: 'Review', description: 'Confirm & submit' },
]
const stepName = ref('')
const stepEmail = ref('')
const stepRole = ref<string | number | null>(null)
const stepBio = ref('')

const roleOptions = [
  { label: 'Developer', value: 'developer' },
  { label: 'Designer', value: 'designer' },
  { label: 'Manager', value: 'manager' },
]

function nextStep() {
  if (currentStep.value < steps.length - 1) currentStep.value++
}
function prevStep() {
  if (currentStep.value > 0) currentStep.value--
}

const isLastStep = computed(() => currentStep.value === steps.length - 1)
</script>

<template>
  <div class="space-y-10">
    <WxPageHeader
      title="Advanced Form Elements"
      description="Date pickers, multi-select tags, color swatches, file upload, input masks, range sliders, and step wizards."
    />

    <!-- ==================== Date Picker ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Calendar class="w-5 h-5" />
        <span>Date Picker</span>
      </div>
      <WxCard>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Start Date</label>
            <WxDatePicker v-model="startDate" placeholder="Select start date" />
          </div>
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">End Date</label>
            <WxDatePicker v-model="endDate" placeholder="Select end date" />
          </div>
        </div>
        <p class="text-xs text-on-surface-variant mt-4">
          Selected range: {{ startDate || '...' }} to {{ endDate || '...' }}
        </p>
      </WxCard>
    </section>

    <!-- ==================== Multi Select with Tags ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Tags class="w-5 h-5" />
        <span>Multi Select with Tags</span>
      </div>
      <WxCard>
        <div class="max-w-md space-y-3">
          <label class="block text-sm font-medium text-on-surface">Select Technologies</label>
          <WxMultiSelect v-model="selectedTags" :options="tagOptions" placeholder="Choose tags..." />
          <p class="text-xs text-on-surface-variant">{{ selectedTags.length }} tag(s) selected</p>
        </div>
      </WxCard>
    </section>

    <!-- ==================== Color Picker ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Palette class="w-5 h-5" />
        <span>Color Picker</span>
      </div>
      <WxCard>
        <div class="space-y-4">
          <label class="block text-sm font-medium text-on-surface">Choose a color</label>
          <div class="flex items-center gap-3">
            <button
              v-for="color in colors"
              :key="color"
              class="w-10 h-10 rounded-full border-2 transition-all duration-200 cursor-pointer flex items-center justify-center"
              :class="selectedColor === color ? 'border-on-surface scale-110 shadow-lg' : 'border-transparent hover:scale-105'"
              :style="{ backgroundColor: color }"
              @click="selectedColor = color"
            >
              <Check v-if="selectedColor === color" class="w-5 h-5 text-white" />
            </button>
          </div>
          <div class="flex items-center gap-2">
            <div class="w-6 h-6 rounded" :style="{ backgroundColor: selectedColor }" />
            <span class="text-sm font-mono text-on-surface-variant">{{ selectedColor }}</span>
          </div>
        </div>
      </WxCard>
    </section>

    <!-- ==================== File Upload ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Upload class="w-5 h-5" />
        <span>File Upload</span>
      </div>
      <WxCard>
        <div class="max-w-lg space-y-3">
          <WxFileUpload accept="image/*,.pdf" :multiple="true" :max-size="5" @upload="handleUpload" />
          <div v-if="uploadedFiles.length" class="space-y-1">
            <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider">Uploaded Files</p>
            <p v-for="f in uploadedFiles" :key="f" class="text-sm text-on-surface">{{ f }}</p>
          </div>
        </div>
      </WxCard>
    </section>

    <!-- ==================== Input Masks ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Phone class="w-5 h-5" />
        <span>Input Masks</span>
      </div>
      <WxCard>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Phone Number</label>
            <WxInput :model-value="phoneVal" placeholder="(012) 345-6789" @input="formatPhone">
              <template #prefix><Phone class="w-4 h-4" /></template>
            </WxInput>
            <p class="text-xs text-on-surface-variant">Format: (XXX) XXX-XXXX</p>
          </div>
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">Currency</label>
            <WxInput :model-value="currencyVal" placeholder="$0" @input="formatCurrency">
              <template #prefix><DollarSign class="w-4 h-4" /></template>
            </WxInput>
            <p class="text-xs text-on-surface-variant">Automatically formats as USD currency</p>
          </div>
        </div>
      </WxCard>
    </section>

    <!-- ==================== Range Sliders ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <SlidersHorizontal class="w-5 h-5" />
        <span>Range Sliders</span>
      </div>
      <WxCard>
        <div class="space-y-8 max-w-lg">
          <div class="space-y-2">
            <label class="block text-sm font-medium text-on-surface">Price Range Min ({{ rangeA }}%)</label>
            <WxSlider v-model="rangeA" :min="0" :max="100" show-value />
          </div>
          <div class="space-y-2">
            <label class="block text-sm font-medium text-on-surface">Price Range Max ({{ rangeB }}%)</label>
            <WxSlider v-model="rangeB" :min="0" :max="100" show-value />
          </div>
          <div class="space-y-2">
            <label class="block text-sm font-medium text-on-surface">Brightness ({{ brightness }}%)</label>
            <WxSlider v-model="brightness" :min="0" :max="100" show-value />
          </div>
        </div>
      </WxCard>
    </section>

    <!-- ==================== Step Wizard ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <ChevronRight class="w-5 h-5" />
        <span>Step Wizard</span>
      </div>
      <WxCard>
        <div class="space-y-8">
          <WxStepper :steps="steps" :current-step="currentStep" />

          <!-- Step content -->
          <div class="min-h-[160px]">
            <!-- Step 0: Account -->
            <div v-if="currentStep === 0" class="space-y-4 max-w-md">
              <div class="space-y-1.5">
                <label class="block text-sm font-medium text-on-surface">Full Name</label>
                <WxInput v-model="stepName" placeholder="Enter your name" />
              </div>
              <div class="space-y-1.5">
                <label class="block text-sm font-medium text-on-surface">Email</label>
                <WxInput v-model="stepEmail" type="email" placeholder="name@example.com" />
              </div>
            </div>

            <!-- Step 1: Details -->
            <div v-if="currentStep === 1" class="space-y-4 max-w-md">
              <div class="space-y-1.5">
                <label class="block text-sm font-medium text-on-surface">Role</label>
                <WxSelect v-model="stepRole" :options="roleOptions" placeholder="Select role..." />
              </div>
              <div class="space-y-1.5">
                <label class="block text-sm font-medium text-on-surface">Bio</label>
                <WxTextarea v-model="stepBio" placeholder="Tell us about yourself..." :rows="3" />
              </div>
            </div>

            <!-- Step 2: Review -->
            <div v-if="currentStep === 2" class="space-y-3">
              <h4 class="text-sm font-semibold text-on-surface">Review your information</h4>
              <div class="bg-surface-container rounded-lg p-4 space-y-2 text-sm">
                <div class="flex justify-between">
                  <span class="text-on-surface-variant">Name</span>
                  <span class="text-on-surface font-medium">{{ stepName || '--' }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-on-surface-variant">Email</span>
                  <span class="text-on-surface font-medium">{{ stepEmail || '--' }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-on-surface-variant">Role</span>
                  <span class="text-on-surface font-medium">{{ stepRole || '--' }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-on-surface-variant">Bio</span>
                  <span class="text-on-surface font-medium">{{ stepBio || '--' }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Navigation -->
          <div class="flex justify-between pt-4 border-t border-outline-variant">
            <WxButton variant="secondary" size="sm" :disabled="currentStep === 0" @click="prevStep">
              <template #icon><ChevronLeft class="w-4 h-4" /></template>
              Previous
            </WxButton>
            <WxButton :variant="isLastStep ? 'success' : 'primary'" size="sm" @click="nextStep">
              {{ isLastStep ? 'Submit' : 'Next' }}
              <template v-if="!isLastStep" #icon><ChevronRight class="w-4 h-4" /></template>
              <template v-else #icon><Check class="w-4 h-4" /></template>
            </WxButton>
          </div>
        </div>
      </WxCard>
    </section>
  </div>
</template>
