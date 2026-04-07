<script setup lang="ts">
import {
  Rocket, CheckCircle2, AlertTriangle, Code, Users,
  Package, Star, Bug, GitBranch, Server, Shield, Zap,
} from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxTimeline from '@/components/ui/WxTimeline.vue'

const timelineItems = [
  { title: 'Project Kickoff', description: 'Initial planning meeting with stakeholders. Defined MVP scope and assigned core team members.', time: 'Jan 15, 2026', color: 'primary' },
  { title: 'Design Phase Complete', description: 'UI/UX wireframes approved. Design system tokens finalized for development handoff.', time: 'Feb 2, 2026', color: 'success' },
  { title: 'Backend API v1 Ready', description: 'Core REST endpoints deployed to staging. Authentication and authorization fully implemented.', time: 'Feb 20, 2026', color: 'info' },
  { title: 'Security Audit Flagged', description: 'Penetration testing revealed 2 medium-severity vulnerabilities in session management.', time: 'Mar 1, 2026', color: 'warning' },
  { title: 'Bug Fix Sprint', description: 'Resolved 23 critical bugs including the session vulnerability. All tests passing.', time: 'Mar 8, 2026', color: 'danger' },
  { title: 'Beta Release', description: 'Launched to 500 beta testers. Collecting feedback via in-app surveys and analytics.', time: 'Mar 15, 2026', color: 'primary' },
  { title: 'Performance Optimization', description: 'Reduced initial load time by 40%. Implemented lazy loading and code splitting.', time: 'Mar 20, 2026', color: 'success' },
  { title: 'Production Launch', description: 'Successfully deployed to production. 99.9% uptime SLA active. Monitoring dashboards live.', time: 'Mar 27, 2026', color: 'success' },
]

const cardEvents = [
  { icon: Rocket, color: 'text-primary', bg: 'bg-primary/10', title: 'Sprint 12 Started', date: 'Mar 18, 2026', desc: 'New sprint kicked off with 34 story points across 12 user stories. Focus area: payment integration module.', tags: ['Sprint', 'Planning'] },
  { icon: Code, color: 'text-info', bg: 'bg-info/10', title: 'PR #847 Merged', date: 'Mar 19, 2026', desc: 'Refactored authentication middleware to support OAuth 2.0 providers. Added Google and GitHub SSO.', tags: ['Code', 'Auth'] },
  { icon: Bug, color: 'text-danger', bg: 'bg-danger/10', title: 'Critical Bug Reported', date: 'Mar 20, 2026', desc: 'Users experiencing intermittent 500 errors on checkout. Root cause: database connection pool exhaustion.', tags: ['Bug', 'P0'] },
  { icon: Shield, color: 'text-success', bg: 'bg-success/10', title: 'Hotfix Deployed', date: 'Mar 20, 2026', desc: 'Connection pool size increased and leak fixed. Monitoring confirms zero errors in the last 4 hours.', tags: ['Fix', 'Deploy'] },
  { icon: Users, color: 'text-primary', bg: 'bg-primary/10', title: 'Team Standup', date: 'Mar 21, 2026', desc: 'All team members on track. Design review for the new dashboard scheduled for Thursday afternoon.', tags: ['Meeting'] },
]

const horizontalEvents = [
  { icon: GitBranch, label: 'Branched', date: 'Mar 1', color: 'bg-primary' },
  { icon: Code, label: 'Development', date: 'Mar 5', color: 'bg-info' },
  { icon: CheckCircle2, label: 'Code Review', date: 'Mar 12', color: 'bg-warning' },
  { icon: Server, label: 'Staging', date: 'Mar 18', color: 'bg-primary' },
  { icon: Shield, label: 'QA Testing', date: 'Mar 22', color: 'bg-warning' },
  { icon: Zap, label: 'Production', date: 'Mar 27', color: 'bg-success' },
]
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Timeline"
      description="Vertical and horizontal timelines for displaying chronological events and project milestones."
    />

    <!-- Basic Timeline -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Project Timeline</h3>
      </template>
      <WxTimeline :items="timelineItems" />
    </WxCard>

    <!-- Timeline with Cards -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Timeline with Cards</h3>
      </template>
      <div class="relative pl-8">
        <div class="absolute left-[0.9375rem] top-2 bottom-2 w-0.5 bg-surface-container-high" />
        <div v-for="(event, index) in cardEvents" :key="index" class="relative pb-6 last:pb-0">
          <div class="absolute -left-8 top-3 w-8 h-8 rounded-full flex items-center justify-center border-2 border-surface-container-lowest z-10" :class="event.bg">
            <component :is="event.icon" :size="14" :class="event.color" />
          </div>
          <div class="ml-4 bg-surface-container-lowest rounded-xl border border-outline-variant shadow-sm p-4">
            <div class="flex items-center justify-between mb-2">
              <h4 class="text-sm font-semibold text-on-surface">{{ event.title }}</h4>
              <span class="text-xs text-on-surface-variant">{{ event.date }}</span>
            </div>
            <p class="text-sm text-on-surface-variant mb-3">{{ event.desc }}</p>
            <div class="flex gap-1.5">
              <span v-for="tag in event.tags" :key="tag"
                class="px-2 py-0.5 text-[10px] font-medium rounded-full bg-surface-container-high text-on-surface-variant">
                {{ tag }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </WxCard>

    <!-- Horizontal Timeline -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Horizontal Timeline</h3>
      </template>
      <div class="overflow-x-auto pb-2">
        <div class="flex items-start min-w-[640px]">
          <div v-for="(event, index) in horizontalEvents" :key="index" class="flex-1 relative">
            <!-- Connector line -->
            <div class="flex items-center">
              <div v-if="index > 0" class="flex-1 h-0.5 bg-surface-container-high" />
              <div v-else class="flex-1" />
              <div class="w-10 h-10 rounded-full flex items-center justify-center shrink-0 z-10 border-2 border-surface-container-lowest" :class="event.color">
                <component :is="event.icon" :size="16" class="text-white" />
              </div>
              <div v-if="index < horizontalEvents.length - 1" class="flex-1 h-0.5 bg-surface-container-high" />
              <div v-else class="flex-1" />
            </div>
            <!-- Label -->
            <div class="text-center mt-3">
              <p class="text-xs font-semibold text-on-surface">{{ event.label }}</p>
              <p class="text-[10px] text-on-surface-variant mt-0.5">{{ event.date }}</p>
            </div>
          </div>
        </div>
      </div>
    </WxCard>
  </div>
</template>
