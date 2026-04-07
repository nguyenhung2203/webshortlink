<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useToast } from '@/composables/useToast'
import {
  Palette,
  Blend,
  Type,
  Layers,
  Sparkles,
  ShieldCheck,
  Copy,
  SlidersHorizontal,
} from 'lucide-vue-next'

const toast = useToast()
const activeSection = ref('brand-colors')

const sections = [
  { id: 'brand-colors', label: 'Brand Colors', icon: Palette },
  { id: 'gradients', label: 'Gradients', icon: Blend },
  { id: 'typography', label: 'Typography', icon: Type },
  { id: 'surfaces', label: 'Foundation Surfaces', icon: Layers },
  { id: 'glass', label: 'Glass Effects', icon: Sparkles },
  { id: 'semantic', label: 'Semantic Colors', icon: ShieldCheck },
  { id: 'density-modes', label: 'Density Modes', icon: SlidersHorizontal },
]

// Brand colors
const brandColors = [
  { name: 'Primary', hex: '#00f2ff', class: 'bg-primary' },
  { name: 'Primary Dim', hex: '#00d8e6', class: 'bg-[#00d8e6]' },
  { name: 'Danger', hex: '#F43F5E', class: 'bg-danger' },
  { name: 'Success', hex: '#10B981', class: 'bg-success' },
  { name: 'Warning', hex: '#F59E0B', class: 'bg-warning' },
  { name: 'Info', hex: '#0EA5E9', class: 'bg-info' },
]

// Gradients
const gradients = [
  { name: 'Header Gradient', from: '#18CFEA', to: '#1E63F1', css: 'linear-gradient(135deg, #18CFEA, #1E63F1)' },
  { name: 'Button Gradient', from: '#00f2ff', to: '#0088ff', css: 'linear-gradient(135deg, #00f2ff, #0088ff)' },
  { name: 'Card Gradient A', from: '#00f2ff', to: '#7c3aed', css: 'linear-gradient(135deg, #00f2ff, #7c3aed)' },
  { name: 'Card Gradient B', from: '#10B981', to: '#0EA5E9', css: 'linear-gradient(135deg, #10B981, #0EA5E9)' },
]

// Typography sizes
const headingSizes = [
  { label: 'text-3xl', class: 'text-3xl font-bold', sample: 'The quick brown fox' },
  { label: 'text-2xl', class: 'text-2xl font-bold', sample: 'The quick brown fox' },
  { label: 'text-xl', class: 'text-xl font-semibold', sample: 'The quick brown fox' },
  { label: 'text-lg', class: 'text-lg font-semibold', sample: 'The quick brown fox' },
  { label: 'text-base', class: 'text-base font-medium', sample: 'The quick brown fox' },
  { label: 'text-sm', class: 'text-sm font-medium', sample: 'The quick brown fox' },
  { label: 'text-xs', class: 'text-xs font-medium', sample: 'The quick brown fox' },
]

const fontWeights = [
  { weight: 300, label: 'Light' },
  { weight: 400, label: 'Regular' },
  { weight: 500, label: 'Medium' },
  { weight: 600, label: 'SemiBold' },
  { weight: 700, label: 'Bold' },
  { weight: 800, label: 'ExtraBold' },
  { weight: 900, label: 'Black' },
]

// Surfaces
const surfaces = [
  { name: 'surface', class: 'bg-surface', desc: 'Base surface' },
  { name: 'surface-container', class: 'bg-surface-container', desc: 'Default container' },
  { name: 'surface-container-low', class: 'bg-surface-container-low', desc: 'Low emphasis' },
  { name: 'surface-container-high', class: 'bg-surface-container-high', desc: 'High emphasis' },
]

const textColors = [
  { name: 'on-surface', class: 'text-on-surface', bg: 'bg-surface-container' },
  { name: 'on-surface-variant', class: 'text-on-surface-variant', bg: 'bg-surface-container' },
]

// Glass levels
const glassLevels = [
  { name: 'glass-heavy', blur: 'backdrop-blur-xl', bg: 'bg-white/15', border: 'border-white/20' },
  { name: 'glass-medium', blur: 'backdrop-blur-md', bg: 'bg-white/10', border: 'border-white/15' },
  { name: 'glass-light', blur: 'backdrop-blur-sm', bg: 'bg-white/5', border: 'border-white/10' },
]

// Semantic
const semanticColors = [
  {
    name: 'Danger',
    base: 'bg-danger',
    light: 'bg-danger/10',
    text: 'text-danger',
    hex: '#F43F5E',
  },
  {
    name: 'Success',
    base: 'bg-success',
    light: 'bg-success/10',
    text: 'text-success',
    hex: '#10B981',
  },
  {
    name: 'Warning',
    base: 'bg-warning',
    light: 'bg-warning/10',
    text: 'text-warning',
    hex: '#F59E0B',
  },
  {
    name: 'Info',
    base: 'bg-info',
    light: 'bg-info/10',
    text: 'text-info',
    hex: '#0EA5E9',
  },
]

function copyToClipboard(value: string) {
  navigator.clipboard.writeText(value).then(() => {
    toast.success('Copied', `${value} copied to clipboard.`)
  }).catch(() => {
    toast.error('Error', 'Failed to copy to clipboard.')
  })
}

function scrollTo(id: string) {
  activeSection.value = id
  const el = document.getElementById(id)
  if (el) {
    el.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }
}

// Intersection observer for active section tracking
let observer: IntersectionObserver | null = null

onMounted(() => {
  observer = new IntersectionObserver(
    (entries) => {
      for (const entry of entries) {
        if (entry.isIntersecting) {
          activeSection.value = entry.target.id
        }
      }
    },
    { rootMargin: '-20% 0px -60% 0px' }
  )
  sections.forEach((s) => {
    const el = document.getElementById(s.id)
    if (el) observer!.observe(el)
  })
})

onUnmounted(() => {
  observer?.disconnect()
})
</script>

<template>
  <div class="flex gap-8 max-w-6xl mx-auto">
    <!-- Side Navigation -->
    <nav class="hidden lg:block w-48 shrink-0">
      <div class="sticky top-6 space-y-1">
        <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider mb-3 px-3">Tokens</p>
        <button
          v-for="section in sections"
          :key="section.id"
          class="flex items-center gap-2 w-full px-3 py-2 text-sm rounded-lg transition-colors duration-150 text-left cursor-pointer"
          :class="activeSection === section.id
            ? 'bg-primary/10 text-primary font-medium'
            : 'text-on-surface-variant hover:text-on-surface hover:bg-surface-container-high/50'"
          @click="scrollTo(section.id)"
        >
          <component :is="section.icon" :size="14" />
          {{ section.label }}
        </button>
      </div>
    </nav>

    <!-- Main Content -->
    <div class="flex-1 min-w-0 space-y-10">
      <!-- Page title -->
      <div>
        <h1 class="text-2xl font-bold text-on-surface tracking-tight">Design Tokens</h1>
        <p class="mt-1 text-sm text-on-surface-variant">Color palette, typography, surfaces, and effects for the Wemake design system.</p>
      </div>

      <!-- 1. Brand Colors -->
      <section :id="'brand-colors'" class="scroll-mt-6">
        <div class="flex items-center gap-2 mb-4">
          <Palette :size="18" class="text-primary" />
          <h2 class="text-lg font-semibold text-on-surface">Brand Colors</h2>
        </div>
        <div class="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm p-5">
          <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-6 gap-4">
            <div
              v-for="color in brandColors"
              :key="color.name"
              class="group cursor-pointer"
              @click="copyToClipboard(color.hex)"
            >
              <div
                :class="color.class"
                class="h-20 rounded-lg shadow-sm relative overflow-hidden transition-transform duration-150 group-hover:scale-[1.03]"
              >
                <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity bg-black/20">
                  <Copy :size="16" class="text-white" />
                </div>
              </div>
              <div class="mt-2">
                <p class="text-sm font-medium text-on-surface">{{ color.name }}</p>
                <p class="text-xs text-on-surface-variant font-mono">{{ color.hex }}</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- 2. Gradients -->
      <section :id="'gradients'" class="scroll-mt-6">
        <div class="flex items-center gap-2 mb-4">
          <Blend :size="18" class="text-primary" />
          <h2 class="text-lg font-semibold text-on-surface">Gradients</h2>
        </div>
        <div class="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm p-5">
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div
              v-for="grad in gradients"
              :key="grad.name"
              class="group cursor-pointer"
              @click="copyToClipboard(grad.css)"
            >
              <div
                class="h-24 rounded-lg shadow-sm relative overflow-hidden transition-transform duration-150 group-hover:scale-[1.02]"
                :style="{ background: grad.css }"
              >
                <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity bg-black/20">
                  <Copy :size="16" class="text-white" />
                </div>
              </div>
              <div class="mt-2">
                <p class="text-sm font-medium text-on-surface">{{ grad.name }}</p>
                <p class="text-xs text-on-surface-variant font-mono">{{ grad.from }} &rarr; {{ grad.to }}</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- 3. Typography -->
      <section :id="'typography'" class="scroll-mt-6">
        <div class="flex items-center gap-2 mb-4">
          <Type :size="18" class="text-primary" />
          <h2 class="text-lg font-semibold text-on-surface">Typography</h2>
        </div>
        <div class="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm p-5 space-y-8">
          <!-- Inter heading sizes -->
          <div>
            <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider mb-4">Inter (sans-serif) - Heading Scale</p>
            <div class="space-y-3">
              <div v-for="h in headingSizes" :key="h.label" class="flex items-baseline gap-4">
                <span class="text-xs text-on-surface-variant font-mono w-20 shrink-0 text-right">{{ h.label }}</span>
                <p :class="h.class" class="text-on-surface font-sans">{{ h.sample }}</p>
              </div>
            </div>
          </div>

          <div class="border-t border-outline-variant" />

          <!-- JetBrains Mono -->
          <div>
            <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider mb-4">JetBrains Mono (monospace) - Code Samples</p>
            <div class="bg-surface-container rounded-lg px-4 py-3 space-y-1">
              <p class="font-mono text-sm text-on-surface">const campaign = await api.getCampaign('C-001')</p>
              <p class="font-mono text-sm text-on-surface">interface TKQC { id: string; budget: number }</p>
              <p class="font-mono text-sm text-on-surface-variant">// Wemake Design System v2.0</p>
            </div>
          </div>

          <div class="border-t border-outline-variant" />

          <!-- Weight scale -->
          <div>
            <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider mb-4">Weight Scale (300 - 900)</p>
            <div class="grid grid-cols-2 sm:grid-cols-4 md:grid-cols-7 gap-3">
              <div v-for="w in fontWeights" :key="w.weight" class="text-center">
                <p class="text-2xl text-on-surface" :style="{ fontWeight: w.weight }">Aa</p>
                <p class="text-xs text-on-surface-variant mt-1">{{ w.weight }}</p>
                <p class="text-[10px] text-on-surface-variant">{{ w.label }}</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- 4. Foundation Surfaces -->
      <section :id="'surfaces'" class="scroll-mt-6">
        <div class="flex items-center gap-2 mb-4">
          <Layers :size="18" class="text-primary" />
          <h2 class="text-lg font-semibold text-on-surface">Foundation Surfaces</h2>
        </div>
        <div class="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm p-5 space-y-6">
          <!-- Surface swatches -->
          <div>
            <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider mb-4">Surface Levels</p>
            <div class="grid grid-cols-2 sm:grid-cols-4 gap-4">
              <div v-for="s in surfaces" :key="s.name" class="space-y-2">
                <div :class="s.class" class="h-20 rounded-lg border border-outline-variant" />
                <p class="text-sm font-medium text-on-surface">{{ s.name }}</p>
                <p class="text-xs text-on-surface-variant">{{ s.desc }}</p>
              </div>
            </div>
          </div>

          <div class="border-t border-outline-variant" />

          <!-- Text colors -->
          <div>
            <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider mb-4">Text Colors</p>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div v-for="tc in textColors" :key="tc.name" class="flex items-center gap-3">
                <div :class="[tc.bg]" class="h-14 w-full rounded-lg border border-outline-variant flex items-center justify-center">
                  <p :class="tc.class" class="text-sm font-medium">{{ tc.name }}</p>
                </div>
              </div>
            </div>
          </div>

          <div class="border-t border-outline-variant" />

          <!-- Light / Dark preview -->
          <div>
            <p class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider mb-4">Mode Preview</p>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div class="rounded-lg p-4 bg-white border border-gray-200">
                <p class="text-xs font-semibold text-gray-400 uppercase mb-2">Light Mode</p>
                <div class="space-y-1.5">
                  <div class="h-4 w-full rounded bg-gray-100" />
                  <div class="h-4 w-3/4 rounded bg-gray-200" />
                  <div class="h-4 w-1/2 rounded bg-gray-50" />
                </div>
              </div>
              <div class="rounded-lg p-4 bg-[#0f1117] border border-gray-800">
                <p class="text-xs font-semibold text-gray-500 uppercase mb-2">Dark Mode</p>
                <div class="space-y-1.5">
                  <div class="h-4 w-full rounded bg-gray-800" />
                  <div class="h-4 w-3/4 rounded bg-gray-700" />
                  <div class="h-4 w-1/2 rounded bg-gray-900" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- 5. Glass Effects -->
      <section :id="'glass'" class="scroll-mt-6">
        <div class="flex items-center gap-2 mb-4">
          <Sparkles :size="18" class="text-primary" />
          <h2 class="text-lg font-semibold text-on-surface">Glass Effects</h2>
        </div>
        <div class="rounded-xl border border-outline-variant shadow-sm overflow-hidden">
          <div class="bg-primary/10 p-6">
            <div
              class="relative bg-linear-to-br from-primary/30 via-info/20 to-primary/10 rounded-xl p-6 overflow-hidden"
            >
              <!-- Background decorative blobs -->
              <div class="absolute top-0 left-1/4 w-32 h-32 bg-primary/40 rounded-full blur-3xl" />
              <div class="absolute bottom-0 right-1/4 w-40 h-40 bg-info/30 rounded-full blur-3xl" />

              <div class="relative grid grid-cols-1 sm:grid-cols-3 gap-4">
                <div
                  v-for="gl in glassLevels"
                  :key="gl.name"
                  class="rounded-xl border p-5 transition-transform duration-150 hover:scale-[1.02]"
                  :class="[gl.blur, gl.bg, gl.border]"
                >
                  <p class="text-sm font-semibold text-on-surface mb-1">{{ gl.name }}</p>
                  <p class="text-xs text-on-surface-variant">
                    {{ gl.blur.replace('backdrop-blur-', 'blur: ') }}
                  </p>
                  <div class="mt-3 h-8 rounded-md" :class="gl.bg" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- 6. Semantic Colors -->
      <section :id="'semantic'" class="scroll-mt-6">
        <div class="flex items-center gap-2 mb-4">
          <ShieldCheck :size="18" class="text-primary" />
          <h2 class="text-lg font-semibold text-on-surface">Semantic Colors</h2>
        </div>
        <div class="bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm p-5">
          <div class="grid grid-cols-2 sm:grid-cols-4 gap-5">
            <div
              v-for="sc in semanticColors"
              :key="sc.name"
              class="group cursor-pointer"
              @click="copyToClipboard(sc.hex)"
            >
              <div class="space-y-2">
                <!-- Full color swatch -->
                <div
                  :class="sc.base"
                  class="h-16 rounded-lg shadow-sm relative overflow-hidden transition-transform duration-150 group-hover:scale-[1.03]"
                >
                  <div class="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity bg-black/20">
                    <Copy :size="16" class="text-white" />
                  </div>
                </div>
                <!-- Light variant -->
                <div :class="sc.light" class="h-10 rounded-lg flex items-center justify-center">
                  <span :class="sc.text" class="text-xs font-semibold">{{ sc.name }}/10</span>
                </div>
                <!-- Label -->
                <div>
                  <p class="text-sm font-medium text-on-surface">{{ sc.name }}</p>
                  <p class="text-xs text-on-surface-variant font-mono">{{ sc.hex }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Density Modes -->
      <section id="density-modes" class="space-y-6">
        <h2 class="text-xl font-bold text-on-surface">Density Modes</h2>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <!-- Compact card -->
          <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6 space-y-4">
            <div class="text-xs font-bold text-primary uppercase tracking-wider">Compact</div>
            <div class="space-y-1">
              <div class="h-6 bg-surface-container rounded flex items-center px-2 text-[10px] text-on-surface-variant">Row height: 32px</div>
              <div class="h-6 bg-surface-container rounded flex items-center px-2 text-[10px] text-on-surface-variant">Row height: 32px</div>
              <div class="h-6 bg-surface-container rounded flex items-center px-2 text-[10px] text-on-surface-variant">Row height: 32px</div>
            </div>
            <p class="text-xs text-on-surface-variant">Padding: 4px, Font: 12px</p>
          </div>
          <!-- Default card -->
          <div class="bg-surface-container-lowest rounded-xl border-2 border-primary p-6 space-y-4">
            <div class="text-xs font-bold text-primary uppercase tracking-wider">Default (Active)</div>
            <div class="space-y-2">
              <div class="h-8 bg-surface-container rounded flex items-center px-3 text-xs text-on-surface-variant">Row height: 40px</div>
              <div class="h-8 bg-surface-container rounded flex items-center px-3 text-xs text-on-surface-variant">Row height: 40px</div>
              <div class="h-8 bg-surface-container rounded flex items-center px-3 text-xs text-on-surface-variant">Row height: 40px</div>
            </div>
            <p class="text-xs text-on-surface-variant">Padding: 8px, Font: 14px</p>
          </div>
          <!-- Comfortable card -->
          <div class="bg-surface-container-lowest rounded-xl border border-outline-variant p-6 space-y-4">
            <div class="text-xs font-bold text-primary uppercase tracking-wider">Comfortable</div>
            <div class="space-y-3">
              <div class="h-10 bg-surface-container rounded flex items-center px-4 text-sm text-on-surface-variant">Row height: 48px</div>
              <div class="h-10 bg-surface-container rounded flex items-center px-4 text-sm text-on-surface-variant">Row height: 48px</div>
              <div class="h-10 bg-surface-container rounded flex items-center px-4 text-sm text-on-surface-variant">Row height: 48px</div>
            </div>
            <p class="text-xs text-on-surface-variant">Padding: 12px, Font: 16px</p>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>
