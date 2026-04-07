<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxTabs from '@/components/ui/WxTabs.vue'
import WxProgressBar from '@/components/ui/WxProgressBar.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxCheckbox from '@/components/ui/WxCheckbox.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import { Calendar, DollarSign, Users, Building2, Flag } from 'lucide-vue-next'

const activeTab = ref('overview')
const tabItems = [
  { key: 'overview', label: 'Overview' },
  { key: 'tasks', label: 'Tasks' },
  { key: 'files', label: 'Files' },
  { key: 'activity', label: 'Activity' },
]

const project = ref({
  name: 'Website Redesign',
  status: 'active' as const,
  priority: 'high' as const,
  progress: 72,
  client: 'Acme Corporation',
  startDate: 'Jan 15, 2026',
  endDate: 'Apr 15, 2026',
  budget: '$45,000',
  team: ['Alice Wang', 'Bob Smith', 'Carol Davis', 'David Kim'],
  description:
    'Complete overhaul of the corporate website with new branding, improved user experience, and modern technology stack. The project includes a full redesign of all public-facing pages, integration with the new CMS, performance optimization, and accessibility compliance to WCAG 2.1 AA standards.',
})

const infoGrid = [
  { icon: Building2, label: 'Client', value: project.value.client },
  { icon: Calendar, label: 'Start Date', value: project.value.startDate },
  { icon: Calendar, label: 'End Date', value: project.value.endDate },
  { icon: DollarSign, label: 'Budget', value: project.value.budget },
]

const metrics = [
  { label: 'Tasks Completed', value: '36 / 50' },
  { label: 'Hours Logged', value: '284h' },
  { label: 'Issues Open', value: '7' },
  { label: 'Days Remaining', value: '19' },
]

interface Task {
  id: number
  title: string
  assignee: string
  dueDate: string
  done: boolean
}

const tasks = ref<Task[]>([
  { id: 1, title: 'Finalize homepage wireframes', assignee: 'Alice Wang', dueDate: 'Mar 28', done: true },
  { id: 2, title: 'Implement responsive navigation', assignee: 'Bob Smith', dueDate: 'Mar 30', done: true },
  { id: 3, title: 'Design product listing page', assignee: 'Carol Davis', dueDate: 'Apr 02', done: false },
  { id: 4, title: 'Set up CI/CD pipeline', assignee: 'David Kim', dueDate: 'Apr 05', done: false },
  { id: 5, title: 'Accessibility audit and fixes', assignee: 'Alice Wang', dueDate: 'Apr 10', done: false },
])

const statusVariant: Record<string, 'info' | 'success' | 'warning' | 'error'> = {
  active: 'info',
  completed: 'success',
  'on-hold': 'warning',
}

const priorityVariant: Record<string, 'error' | 'warning' | 'info' | 'success'> = {
  critical: 'error',
  high: 'error',
  medium: 'warning',
  low: 'info',
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader :title="project.name" description="Project details and task management">
      <template #actions>
        <WxBadge :variant="statusVariant[project.status]" size="lg">{{ project.status }}</WxBadge>
        <WxBadge :variant="priorityVariant[project.priority]" size="lg">{{ project.priority }} priority</WxBadge>
      </template>
    </WxPageHeader>

    <!-- Progress -->
    <WxCard>
      <WxProgressBar :value="project.progress" variant="primary" show-value>
        <span class="text-sm font-medium text-on-surface">Overall Progress</span>
      </WxProgressBar>
    </WxCard>

    <!-- Info Grid -->
    <div class="grid grid-cols-2 lg:grid-cols-4 gap-4">
      <WxCard v-for="info in infoGrid" :key="info.label">
        <div class="flex items-center gap-3">
          <div class="p-2 rounded-lg bg-primary/10 text-primary shrink-0">
            <component :is="info.icon" :size="18" />
          </div>
          <div>
            <p class="text-xs text-on-surface-variant">{{ info.label }}</p>
            <p class="text-sm font-semibold text-on-surface">{{ info.value }}</p>
          </div>
        </div>
      </WxCard>
    </div>

    <!-- Team -->
    <WxCard>
      <div class="flex items-center gap-2 mb-3">
        <Users :size="16" class="text-on-surface-variant" />
        <h3 class="text-sm font-semibold text-on-surface">Team Members</h3>
      </div>
      <div class="flex flex-wrap gap-3">
        <div v-for="member in project.team" :key="member" class="flex items-center gap-2 bg-surface-container rounded-full pl-1 pr-3 py-1">
          <WxAvatar :name="member" size="sm" />
          <span class="text-sm text-on-surface">{{ member }}</span>
        </div>
      </div>
    </WxCard>

    <!-- Tabs -->
    <WxCard>
      <WxTabs v-model="activeTab" :items="tabItems">
        <template #default="{ activeKey }">
          <!-- Overview -->
          <div v-if="activeKey === 'overview'" class="space-y-6">
            <div>
              <h3 class="text-sm font-semibold text-on-surface mb-2">Description</h3>
              <p class="text-sm text-on-surface-variant leading-relaxed">{{ project.description }}</p>
            </div>
            <div>
              <h3 class="text-sm font-semibold text-on-surface mb-3">Key Metrics</h3>
              <div class="grid grid-cols-2 sm:grid-cols-4 gap-4">
                <div v-for="metric in metrics" :key="metric.label" class="bg-surface-container rounded-lg p-3 text-center">
                  <p class="text-lg font-bold text-on-surface">{{ metric.value }}</p>
                  <p class="text-xs text-on-surface-variant mt-1">{{ metric.label }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Tasks -->
          <div v-if="activeKey === 'tasks'" class="space-y-1">
            <div
              v-for="task in tasks"
              :key="task.id"
              class="flex items-center gap-3 p-3 rounded-lg hover:bg-surface-container/50 transition-colors"
            >
              <WxCheckbox v-model="task.done" />
              <div class="flex-1 min-w-0">
                <p class="text-sm text-on-surface" :class="task.done && 'line-through opacity-60'">
                  {{ task.title }}
                </p>
              </div>
              <div class="flex items-center gap-3 shrink-0">
                <div class="flex items-center gap-1.5">
                  <WxAvatar :name="task.assignee" size="sm" />
                  <span class="text-xs text-on-surface-variant hidden sm:inline">{{ task.assignee }}</span>
                </div>
                <span class="text-xs text-on-surface-variant flex items-center gap-1">
                  <Calendar :size="12" />
                  {{ task.dueDate }}
                </span>
              </div>
            </div>
          </div>

          <!-- Files -->
          <div v-if="activeKey === 'files'" class="text-center py-8">
            <Flag :size="40" class="mx-auto text-on-surface-variant/30 mb-3" />
            <p class="text-sm text-on-surface-variant">No files uploaded yet.</p>
          </div>

          <!-- Activity -->
          <div v-if="activeKey === 'activity'" class="text-center py-8">
            <Flag :size="40" class="mx-auto text-on-surface-variant/30 mb-3" />
            <p class="text-sm text-on-surface-variant">Activity log coming soon.</p>
          </div>
        </template>
      </WxTabs>
    </WxCard>
  </div>
</template>
