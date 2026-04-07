<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxProgressBar from '@/components/ui/WxProgressBar.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { Plus, Calendar } from 'lucide-vue-next'

interface Project {
  name: string
  description: string
  progress: number
  team: string[]
  deadline: string
  status: 'active' | 'completed' | 'on-hold' | 'at-risk'
}

const projects = ref<Project[]>([
  {
    name: 'Website Redesign',
    description: 'Complete overhaul of the corporate website with new branding and improved user experience.',
    progress: 72,
    team: ['Alice Wang', 'Bob Smith', 'Carol Davis'],
    deadline: 'Apr 15, 2026',
    status: 'active',
  },
  {
    name: 'Mobile App',
    description: 'Native iOS and Android application for customer self-service and account management.',
    progress: 45,
    team: ['David Kim', 'Eva Martinez', 'Frank Lee'],
    deadline: 'Jun 30, 2026',
    status: 'active',
  },
  {
    name: 'API Integration',
    description: 'Third-party payment and logistics API integration for the e-commerce platform.',
    progress: 90,
    team: ['Grace Chen', 'Henry Park', 'Ivy Scott'],
    deadline: 'Mar 31, 2026',
    status: 'at-risk',
  },
  {
    name: 'Database Migration',
    description: 'Migrate legacy PostgreSQL databases to new cloud-hosted infrastructure with zero downtime.',
    progress: 100,
    team: ['Jack Turner', 'Kate Brown', 'Leo Adams'],
    deadline: 'Mar 10, 2026',
    status: 'completed',
  },
  {
    name: 'Marketing Campaign',
    description: 'Q2 digital marketing campaign across social media, email, and search advertising channels.',
    progress: 30,
    team: ['Mia Johnson', 'Noah Wilson', 'Olivia Hart'],
    deadline: 'May 20, 2026',
    status: 'on-hold',
  },
  {
    name: 'Security Audit',
    description: 'Comprehensive security audit including penetration testing and compliance verification.',
    progress: 60,
    team: ['Paul Young', 'Quinn Reed', 'Rachel Fox'],
    deadline: 'Apr 28, 2026',
    status: 'active',
  },
])

const statusConfig: Record<string, { variant: 'success' | 'info' | 'warning' | 'error'; label: string }> = {
  active: { variant: 'info', label: 'Active' },
  completed: { variant: 'success', label: 'Completed' },
  'on-hold': { variant: 'warning', label: 'On Hold' },
  'at-risk': { variant: 'error', label: 'At Risk' },
}

function progressVariant(val: number): 'primary' | 'success' | 'warning' | 'danger' {
  if (val >= 100) return 'success'
  if (val >= 60) return 'primary'
  if (val >= 30) return 'warning'
  return 'danger'
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Projects" description="Manage and track your team projects">
      <template #actions>
        <WxButton size="sm">
          <template #icon><Plus :size="16" /></template>
          New Project
        </WxButton>
      </template>
    </WxPageHeader>

    <div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
      <WxCard v-for="project in projects" :key="project.name">
        <div class="flex items-start justify-between gap-2 mb-3">
          <h3 class="text-base font-semibold text-on-surface">{{ project.name }}</h3>
          <WxBadge :variant="statusConfig[project.status].variant" size="sm">
            {{ statusConfig[project.status].label }}
          </WxBadge>
        </div>

        <p class="text-sm text-on-surface-variant line-clamp-2 mb-4">{{ project.description }}</p>

        <WxProgressBar :value="project.progress" :variant="progressVariant(project.progress)" show-value size="sm">
          <span class="text-xs text-on-surface-variant">Progress</span>
        </WxProgressBar>

        <div class="flex items-center justify-between mt-4 pt-4 border-t border-outline-variant">
          <!-- Team Avatars -->
          <div class="flex -space-x-2">
            <div
              v-for="(member, i) in project.team"
              :key="i"
              class="ring-2 ring-surface-container-lowest rounded-full"
            >
              <WxAvatar :name="member" size="sm" />
            </div>
          </div>

          <!-- Deadline -->
          <div class="flex items-center gap-1 text-xs text-on-surface-variant">
            <Calendar :size="12" />
            <span>{{ project.deadline }}</span>
          </div>
        </div>
      </WxCard>
    </div>
  </div>
</template>
