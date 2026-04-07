<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  Home, User, Settings, Search, Bell, Mail, Heart, Star, Calendar, Clock,
  Camera, Cloud, Download, Upload, FileText, Folder, Globe, Lock, Unlock,
  Eye, EyeOff, Check, X, Plus, Minus, ChevronDown, ChevronUp, ChevronLeft,
  ChevronRight, ArrowUp, ArrowDown, ArrowLeft, ArrowRight, Edit, Trash2,
  Copy, Clipboard, Share2, Link, ExternalLink, Filter, BarChart3, PieChart,
  TrendingUp, Activity, Zap, AlertCircle, AlertTriangle, Info, HelpCircle,
  MessageSquare, Phone, MapPin, Package, ShoppingCart, CreditCard, DollarSign,
  Bookmark, Tag, Hash, AtSign, Wifi, Bluetooth, Monitor, Smartphone, Tablet,
} from 'lucide-vue-next'
import { useToast } from '@/composables/useToast'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxInput from '@/components/ui/WxInput.vue'

const toast = useToast()
const searchQuery = ref('')

const icons = [
  { name: 'Home', component: Home },
  { name: 'User', component: User },
  { name: 'Settings', component: Settings },
  { name: 'Search', component: Search },
  { name: 'Bell', component: Bell },
  { name: 'Mail', component: Mail },
  { name: 'Heart', component: Heart },
  { name: 'Star', component: Star },
  { name: 'Calendar', component: Calendar },
  { name: 'Clock', component: Clock },
  { name: 'Camera', component: Camera },
  { name: 'Cloud', component: Cloud },
  { name: 'Download', component: Download },
  { name: 'Upload', component: Upload },
  { name: 'FileText', component: FileText },
  { name: 'Folder', component: Folder },
  { name: 'Globe', component: Globe },
  { name: 'Lock', component: Lock },
  { name: 'Unlock', component: Unlock },
  { name: 'Eye', component: Eye },
  { name: 'EyeOff', component: EyeOff },
  { name: 'Check', component: Check },
  { name: 'X', component: X },
  { name: 'Plus', component: Plus },
  { name: 'Minus', component: Minus },
  { name: 'ChevronDown', component: ChevronDown },
  { name: 'ChevronUp', component: ChevronUp },
  { name: 'ChevronLeft', component: ChevronLeft },
  { name: 'ChevronRight', component: ChevronRight },
  { name: 'ArrowUp', component: ArrowUp },
  { name: 'ArrowDown', component: ArrowDown },
  { name: 'ArrowLeft', component: ArrowLeft },
  { name: 'ArrowRight', component: ArrowRight },
  { name: 'Edit', component: Edit },
  { name: 'Trash2', component: Trash2 },
  { name: 'Copy', component: Copy },
  { name: 'Clipboard', component: Clipboard },
  { name: 'Share2', component: Share2 },
  { name: 'Link', component: Link },
  { name: 'ExternalLink', component: ExternalLink },
  { name: 'Filter', component: Filter },
  { name: 'BarChart3', component: BarChart3 },
  { name: 'PieChart', component: PieChart },
  { name: 'TrendingUp', component: TrendingUp },
  { name: 'Activity', component: Activity },
  { name: 'Zap', component: Zap },
  { name: 'AlertCircle', component: AlertCircle },
  { name: 'AlertTriangle', component: AlertTriangle },
  { name: 'Info', component: Info },
  { name: 'HelpCircle', component: HelpCircle },
  { name: 'MessageSquare', component: MessageSquare },
  { name: 'Phone', component: Phone },
  { name: 'MapPin', component: MapPin },
  { name: 'Package', component: Package },
  { name: 'ShoppingCart', component: ShoppingCart },
  { name: 'CreditCard', component: CreditCard },
  { name: 'DollarSign', component: DollarSign },
  { name: 'Bookmark', component: Bookmark },
  { name: 'Tag', component: Tag },
  { name: 'Hash', component: Hash },
  { name: 'AtSign', component: AtSign },
  { name: 'Wifi', component: Wifi },
  { name: 'Bluetooth', component: Bluetooth },
  { name: 'Monitor', component: Monitor },
  { name: 'Smartphone', component: Smartphone },
  { name: 'Tablet', component: Tablet },
]

const filteredIcons = computed(() => {
  if (!searchQuery.value) return icons
  const q = searchQuery.value.toLowerCase()
  return icons.filter(i => i.name.toLowerCase().includes(q))
})

async function copyName(name: string) {
  try {
    await navigator.clipboard.writeText(name)
    toast.success('Copied', `"${name}" copied to clipboard.`)
  } catch {
    toast.info('Icon Name', name)
  }
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Icon Gallery"
      description="Browse and search 65+ Lucide icons. Click any icon to copy its name."
    />

    <WxCard>
      <template #header>
        <div class="flex items-center justify-between gap-4">
          <h3 class="text-sm font-semibold text-on-surface">Lucide Icons</h3>
          <span class="text-xs text-on-surface-variant">{{ filteredIcons.length }} icons</span>
        </div>
      </template>
      <div class="mb-5">
        <WxInput v-model="searchQuery" placeholder="Search icons..." />
      </div>
      <div class="grid grid-cols-4 sm:grid-cols-6 md:grid-cols-8 lg:grid-cols-10 gap-2">
        <button
          v-for="icon in filteredIcons"
          :key="icon.name"
          class="flex flex-col items-center justify-center gap-2 p-3 rounded-lg border border-transparent
            hover:border-outline-variant hover:bg-surface-container transition-all cursor-pointer group"
          @click="copyName(icon.name)"
        >
          <component :is="icon.component" :size="22" class="text-on-surface-variant group-hover:text-primary transition-colors" />
          <span class="text-[10px] text-on-surface-variant group-hover:text-on-surface truncate w-full text-center">
            {{ icon.name }}
          </span>
        </button>
      </div>
      <div v-if="filteredIcons.length === 0" class="py-12 text-center">
        <Search :size="32" class="mx-auto text-on-surface-variant/50 mb-2" />
        <p class="text-sm text-on-surface-variant">No icons found for "{{ searchQuery }}"</p>
      </div>
    </WxCard>
  </div>
</template>
