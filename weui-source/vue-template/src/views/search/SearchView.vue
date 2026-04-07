<script setup lang="ts">
import { ref } from 'vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxTabs from '@/components/ui/WxTabs.vue'
import WxPagination from '@/components/ui/WxPagination.vue'
import { Search, FileText, Users, FolderKanban, Clock } from 'lucide-vue-next'

const query = ref('campaign')
const activeTab = ref<string | number>('all')
const currentPage = ref(1)

const tabs = [
  { key: 'all', label: 'All' },
  { key: 'pages', label: 'Pages' },
  { key: 'users', label: 'Users' },
  { key: 'projects', label: 'Projects' },
]

interface SearchResult {
  id: number
  type: 'page' | 'user' | 'project'
  title: string
  url: string
  description: string
  date: string
}

const results: SearchResult[] = [
  {
    id: 1,
    type: 'page',
    title: 'Email Campaign Analytics Dashboard',
    url: 'Dashboard / Marketing / Campaign Analytics',
    description: 'View detailed analytics for all email campaigns including open rates, click-through rates, and conversion metrics for Q4 planning.',
    date: 'Mar 15, 2026',
  },
  {
    id: 2,
    type: 'user',
    title: 'Sarah Campaign Manager',
    url: 'Users / Marketing Team',
    description: 'Marketing lead responsible for campaign strategy and execution across digital channels. Active contributor to quarterly campaigns.',
    date: 'Mar 12, 2026',
  },
  {
    id: 3,
    type: 'project',
    title: 'Q1 Campaign Automation Project',
    url: 'Projects / Marketing / Automation',
    description: 'Automated campaign workflow project integrating CRM data with email and social media platforms for targeted outreach.',
    date: 'Mar 10, 2026',
  },
  {
    id: 4,
    type: 'page',
    title: 'Campaign Template Builder',
    url: 'Tools / Email / Template Builder',
    description: 'Drag-and-drop campaign template builder with reusable blocks, A/B testing variants, and responsive email preview.',
    date: 'Feb 28, 2026',
  },
  {
    id: 5,
    type: 'project',
    title: 'Social Media Campaign Tracker',
    url: 'Projects / Social / Campaign Tracker',
    description: 'Cross-platform social media campaign performance tracker with real-time engagement metrics and ROI calculations.',
    date: 'Feb 20, 2026',
  },
  {
    id: 6,
    type: 'user',
    title: 'Alex Rivera - Campaign Analyst',
    url: 'Users / Analytics Team',
    description: 'Data analyst specializing in campaign performance metrics, attribution modeling, and marketing funnel optimization.',
    date: 'Feb 15, 2026',
  },
]

const typeIcons = {
  page: FileText,
  user: Users,
  project: FolderKanban,
}

const typeColors = {
  page: 'text-primary bg-primary/10',
  user: 'text-violet-500 bg-violet-500/10',
  project: 'text-emerald-500 bg-emerald-500/10',
}

function highlightKeyword(text: string): string {
  if (!query.value) return text
  const regex = new RegExp(`(${query.value})`, 'gi')
  return text.replace(regex, '<mark class="bg-yellow-300/40 text-on-surface rounded px-0.5">$1</mark>')
}
</script>

<template>
  <div class="space-y-6">
    <!-- Search bar -->
    <div class="max-w-2xl">
      <WxInput v-model="query" type="search" placeholder="Search pages, users, projects..." size="lg">
        <template #prefix>
          <Search :size="20" class="text-on-surface-variant" />
        </template>
      </WxInput>
    </div>

    <!-- Tabs -->
    <WxTabs v-model="activeTab" :items="tabs">
      <template #default>
        <!-- Results count -->
        <p class="text-sm text-on-surface-variant mb-4">
          Found <span class="font-semibold text-on-surface">24</span> results for
          '<span class="font-medium text-primary">{{ query }}</span>'
        </p>

        <!-- Results list -->
        <div class="space-y-4">
          <div
            v-for="result in results"
            :key="result.id"
            class="group bg-surface-container-lowest rounded-xl border border-outline-variant p-4 hover:border-primary/30 hover:shadow-sm transition-all duration-200 cursor-pointer"
          >
            <div class="flex items-start gap-3">
              <!-- Type icon -->
              <div class="shrink-0 w-9 h-9 rounded-lg flex items-center justify-center" :class="typeColors[result.type]">
                <component :is="typeIcons[result.type]" :size="18" />
              </div>

              <div class="flex-1 min-w-0">
                <!-- Title -->
                <h3
                  class="text-sm font-semibold text-on-surface group-hover:text-primary transition-colors"
                  v-html="highlightKeyword(result.title)"
                />
                <!-- URL breadcrumb -->
                <p class="text-xs text-primary/70 mt-0.5">{{ result.url }}</p>
                <!-- Description -->
                <p
                  class="text-sm text-on-surface-variant mt-1 line-clamp-2"
                  v-html="highlightKeyword(result.description)"
                />
                <!-- Date -->
                <div class="flex items-center gap-1 mt-2 text-xs text-on-surface-variant/60">
                  <Clock :size="12" />
                  <span>{{ result.date }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Pagination -->
        <div class="mt-6">
          <WxPagination
            v-model:current-page="currentPage"
            :total-pages="4"
            :total-items="24"
            :page-size="6"
          />
        </div>
      </template>
    </WxTabs>
  </div>
</template>
