<script setup lang="ts">
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import { LayoutDashboard, FileInput, Table, Layers } from 'lucide-vue-next'

const quickLinks = [
  {
    title: 'Dashboard',
    description: 'View analytics and key metrics at a glance',
    icon: LayoutDashboard,
    color: 'text-cyan-500 bg-cyan-500/10',
    route: '/',
  },
  {
    title: 'Forms',
    description: 'Input components and form validation examples',
    icon: FileInput,
    color: 'text-violet-500 bg-violet-500/10',
    route: '/forms',
  },
  {
    title: 'Tables',
    description: 'Data tables with sorting, filtering, and pagination',
    icon: Table,
    color: 'text-emerald-500 bg-emerald-500/10',
    route: '/tables',
  },
  {
    title: 'UI Elements',
    description: 'Buttons, badges, alerts, and other UI components',
    icon: Layers,
    color: 'text-amber-500 bg-amber-500/10',
    route: '/ui',
  },
]
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Starter Page"
      description="Use this as a starting point for new pages"
    />

    <WxCard>
      <div class="text-center py-10">
        <div class="w-16 h-16 mx-auto mb-4 rounded-full bg-surface-container flex items-center justify-center">
          <Layers :size="28" class="text-on-surface-variant" />
        </div>
        <h2 class="text-lg font-semibold text-on-surface">This is a starter page</h2>
        <p class="text-sm text-on-surface-variant mt-1 max-w-md mx-auto">
          Add your content here. Use the Wx component library and Tailwind CSS utility classes to build your interface.
        </p>
      </div>
    </WxCard>

    <!-- Quick links grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <router-link
        v-for="link in quickLinks"
        :key="link.title"
        :to="link.route"
        class="group bg-surface-container-lowest rounded-xl border border-outline-variant p-5 hover:border-primary/30 hover:shadow-md transition-all duration-200"
      >
        <div class="w-10 h-10 rounded-lg flex items-center justify-center mb-3" :class="link.color">
          <component :is="link.icon" :size="20" />
        </div>
        <h3 class="text-sm font-semibold text-on-surface group-hover:text-primary transition-colors">
          {{ link.title }}
        </h3>
        <p class="text-xs text-on-surface-variant mt-1 leading-relaxed">{{ link.description }}</p>
      </router-link>
    </div>
  </div>
</template>
