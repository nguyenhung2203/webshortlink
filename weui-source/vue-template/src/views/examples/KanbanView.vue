<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { Plus, MoreHorizontal, MessageSquare, Paperclip } from 'lucide-vue-next'

type Priority = 'high' | 'medium' | 'low'
type ColumnId = 'todo' | 'progress' | 'review' | 'done'

interface KanbanCard {
  id: number
  title: string
  description: string
  assignee: string
  priority: Priority
  comments: number
  attachments: number
}

interface KanbanColumn {
  id: ColumnId
  title: string
  color: string
  headerBg: string
  cards: KanbanCard[]
}

const priorityVariant: Record<Priority, 'error' | 'warning' | 'info'> = {
  high: 'error',
  medium: 'warning',
  low: 'info',
}

const columns = ref<KanbanColumn[]>([
  {
    id: 'todo',
    title: 'To Do',
    color: 'bg-slate-400',
    headerBg: 'bg-slate-500/10',
    cards: [
      { id: 1, title: 'Design new landing page', description: 'Create wireframes and high-fidelity mockups for the marketing site redesign.', assignee: 'Nguyen Minh Tuan', priority: 'high', comments: 3, attachments: 2 },
      { id: 2, title: 'Write API documentation', description: 'Document all REST endpoints including auth, users, and orders modules.', assignee: 'Tran Thi Mai', priority: 'medium', comments: 1, attachments: 0 },
      { id: 3, title: 'Update user manual', description: 'Revise the user guide to cover new dashboard features released in v2.4.', assignee: 'Le Van Hung', priority: 'low', comments: 0, attachments: 1 },
    ],
  },
  {
    id: 'progress',
    title: 'In Progress',
    color: 'bg-blue-500',
    headerBg: 'bg-blue-500/10',
    cards: [
      { id: 4, title: 'Implement auth flow', description: 'Build OAuth2 login with Google and GitHub providers, including token refresh.', assignee: 'Pham Ngoc Linh', priority: 'high', comments: 5, attachments: 1 },
      { id: 5, title: 'Build dashboard widgets', description: 'Create reusable stat cards, chart components, and activity feed widgets.', assignee: 'Vo Thanh Dat', priority: 'medium', comments: 2, attachments: 3 },
    ],
  },
  {
    id: 'review',
    title: 'Review',
    color: 'bg-amber-500',
    headerBg: 'bg-amber-500/10',
    cards: [
      { id: 6, title: 'Code review: payment module', description: 'Review Stripe integration, webhook handlers, and subscription management logic.', assignee: 'Hoang Mai Anh', priority: 'high', comments: 8, attachments: 4 },
    ],
  },
  {
    id: 'done',
    title: 'Done',
    color: 'bg-emerald-500',
    headerBg: 'bg-emerald-500/10',
    cards: [
      { id: 7, title: 'Setup CI/CD pipeline', description: 'Configured GitHub Actions for automated testing, linting, and deployment.', assignee: 'Do Minh Khoi', priority: 'medium', comments: 4, attachments: 2 },
      { id: 8, title: 'Database migration', description: 'Migrated PostgreSQL schema from v1 to v2 with zero-downtime strategy.', assignee: 'Bui Ngoc Lan', priority: 'high', comments: 6, attachments: 1 },
    ],
  },
])
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Kanban Board" description="Manage tasks across workflow stages with drag-and-drop cards.">
      <template #actions>
        <WxButton size="sm">
          <template #icon><Plus :size="16" /></template>
          Add Task
        </WxButton>
      </template>
    </WxPageHeader>

    <!-- Kanban Columns -->
    <div class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-5">
      <div
        v-for="col in columns"
        :key="col.id"
        class="flex flex-col rounded-xl border border-outline-variant bg-surface-container-lowest"
      >
        <!-- Column Header -->
        <div class="flex items-center justify-between px-4 py-3 border-b border-outline-variant" :class="col.headerBg">
          <div class="flex items-center gap-2">
            <span class="w-2.5 h-2.5 rounded-full" :class="col.color" />
            <h3 class="text-sm font-semibold text-on-surface">{{ col.title }}</h3>
            <WxBadge variant="neutral" size="sm">{{ col.cards.length }}</WxBadge>
          </div>
          <button class="p-1 rounded-md hover:bg-surface-container-high text-on-surface-variant transition-colors cursor-pointer">
            <MoreHorizontal :size="16" />
          </button>
        </div>

        <!-- Cards List -->
        <div class="flex-1 p-3 space-y-3 min-h-[120px]">
          <div
            v-for="card in col.cards"
            :key="card.id"
            class="p-3.5 rounded-lg border border-outline-variant bg-surface-container-lowest shadow-sm hover:shadow-md transition-shadow cursor-pointer"
          >
            <div class="flex items-start justify-between gap-2 mb-2">
              <h4 class="text-sm font-medium text-on-surface leading-snug">{{ card.title }}</h4>
              <WxBadge :variant="priorityVariant[card.priority]" size="sm">{{ card.priority }}</WxBadge>
            </div>
            <p class="text-xs text-on-surface-variant line-clamp-2 mb-3">{{ card.description }}</p>
            <div class="flex items-center justify-between">
              <WxAvatar :name="card.assignee" size="sm" />
              <div class="flex items-center gap-3 text-on-surface-variant">
                <span v-if="card.comments" class="flex items-center gap-1 text-xs">
                  <MessageSquare :size="12" />
                  {{ card.comments }}
                </span>
                <span v-if="card.attachments" class="flex items-center gap-1 text-xs">
                  <Paperclip :size="12" />
                  {{ card.attachments }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Add Card Button -->
        <div class="p-3 pt-0">
          <button class="w-full flex items-center justify-center gap-1.5 py-2 text-xs font-medium text-on-surface-variant rounded-lg border border-dashed border-outline-variant hover:bg-surface-container-high hover:text-on-surface transition-colors cursor-pointer">
            <Plus :size="14" />
            Add card
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
