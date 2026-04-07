<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { PanelLeftClose, PanelLeftOpen, Shrink, Expand } from 'lucide-vue-next'

const isCollapsed = ref(false)

function toggleSidebar() {
  isCollapsed.value = !isCollapsed.value
  document.documentElement.classList.toggle('sidebar-collapsed', isCollapsed.value)
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Collapsed Sidebar Layout"
      description="The sidebar can be collapsed to icon-only mode, giving more space to the main content area."
    >
      <template #actions>
        <WxButton @click="toggleSidebar" variant="secondary" size="sm">
          <component :is="isCollapsed ? PanelLeftOpen : PanelLeftClose" :size="16" class="mr-2" />
          {{ isCollapsed ? 'Expand Sidebar' : 'Collapse Sidebar' }}
        </WxButton>
      </template>
    </WxPageHeader>

    <div class="p-4 rounded-xl border-2 border-dashed border-outline-variant bg-surface-container-lowest">
      <div class="flex items-center gap-3 mb-3">
        <component :is="isCollapsed ? Shrink : Expand" :size="20" class="text-primary" />
        <span class="text-sm font-semibold text-on-surface">
          Sidebar is currently: <span class="text-primary">{{ isCollapsed ? 'Collapsed' : 'Expanded' }}</span>
        </span>
      </div>
      <p class="text-sm text-on-surface-variant">
        Click the button above to toggle the sidebar state. In collapsed mode, the sidebar shows only
        icons, freeing up horizontal space for the main content area.
      </p>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <WxCard>
        <div class="flex items-start gap-4">
          <div class="w-10 h-10 rounded-lg bg-violet-500/10 text-violet-500 flex items-center justify-center shrink-0">
            <PanelLeftClose :size="20" />
          </div>
          <div>
            <h3 class="text-sm font-semibold text-on-surface mb-1">Collapsed Mode</h3>
            <p class="text-xs text-on-surface-variant">
              When collapsed, the sidebar shrinks to show only navigation icons. Hovering over an icon
              can reveal a tooltip with the full label. This maximizes content space.
            </p>
          </div>
        </div>
      </WxCard>

      <WxCard>
        <div class="flex items-start gap-4">
          <div class="w-10 h-10 rounded-lg bg-cyan-500/10 text-cyan-500 flex items-center justify-center shrink-0">
            <PanelLeftOpen :size="20" />
          </div>
          <div>
            <h3 class="text-sm font-semibold text-on-surface mb-1">Expanded Mode</h3>
            <p class="text-xs text-on-surface-variant">
              In expanded mode, the sidebar displays full navigation labels, sub-menus, and section
              headers. This provides better discoverability for new users.
            </p>
          </div>
        </div>
      </WxCard>
    </div>

    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Implementation Notes</h3>
      </template>
      <div class="text-sm text-on-surface-variant space-y-3">
        <p>
          The collapsed sidebar pattern uses a CSS transition to animate the width change. Navigation
          items transition from icon+label to icon-only mode. The main content area adjusts its left
          margin accordingly.
        </p>
        <p>
          User preference for collapsed or expanded state is typically persisted in local storage,
          so the layout remembers the user's choice across sessions.
        </p>
      </div>
    </WxCard>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <WxCard v-for="n in 3" :key="n">
        <div class="h-20 flex items-center justify-center">
          <div class="text-center">
            <p class="text-xl font-bold text-on-surface">{{ (n * 234).toLocaleString() }}</p>
            <p class="text-xs text-on-surface-variant mt-1">Sample Metric {{ n }}</p>
          </div>
        </div>
      </WxCard>
    </div>
  </div>
</template>
