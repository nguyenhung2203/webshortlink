<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { LinkService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { ShortLink } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'
import { Search, ArrowUpDown, Link2, MousePointerClick, Copy, ExternalLink } from 'lucide-vue-next'

const authStore = useAuthStore()
const links = ref<ShortLink[]>([])
const loading = ref(true)
const error = ref('')
const search = ref('')
const sortKey = ref<'createdAt' | 'clicks'>('createdAt')
const copySuccessId = ref<string | null>(null)

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    links.value = await LinkService.list(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách link.'
  } finally {
    loading.value = false
  }
}

async function toggleStatus(link: ShortLink) {
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực')
    await LinkService.updateStatus(authStore.accessToken, link.id, link.status !== 'Active')
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật trạng thái.'
  }
}

async function copyUrl(link: ShortLink) {
  try {
    await navigator.clipboard.writeText(link.shortUrl)
    copySuccessId.value = link.id
    setTimeout(() => (copySuccessId.value = null), 1800)
  } catch {
    // ignore
  }
}

const filtered = computed(() => {
  let list = links.value
  if (search.value.trim()) {
    const q = search.value.trim().toLowerCase()
    list = list.filter(
      (l) => l.slug.toLowerCase().includes(q) || l.originalUrl.toLowerCase().includes(q) || (l.shortUrl || '').toLowerCase().includes(q)
    )
  }
  if (sortKey.value === 'clicks') {
    list = [...list].sort((a, b) => (b.totalClicks ?? 0) - (a.totalClicks ?? 0))
  } else {
    list = [...list].sort(
      (a, b) => new Date(b.createdAtUtc ?? 0).getTime() - new Date(a.createdAtUtc ?? 0).getTime()
    )
  }
  return list
})

function toggleSort() {
  sortKey.value = sortKey.value === 'createdAt' ? 'clicks' : 'createdAt'
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader title="Danh sách Link" subtitle="Quản lý shortlink và trạng thái chuyển hướng.">
      <template #actions>
        <WxButton variant="primary" to="/app/links/create">Tạo Link mới</WxButton>
      </template>
    </WxPageHeader>

    <p v-if="error" class="text-danger text-sm">{{ error }}</p>

    <!-- Search + Sort bar -->
    <div v-if="!loading && links.length > 0" class="flex flex-col sm:flex-row gap-3">
      <div class="relative flex-1">
        <Search :size="16" class="absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant" />
        <input
          v-model="search"
          type="text"
          placeholder="Tìm theo slug, URL..."
          class="w-full h-10 pl-9 pr-4 rounded-xl border border-outline bg-surface-container text-sm focus:outline-none focus:border-primary"
        />
      </div>
      <button
        @click="toggleSort"
        class="flex items-center gap-2 px-4 h-10 rounded-xl border border-outline bg-surface-container text-sm font-medium text-on-surface hover:bg-surface-container-high transition-colors"
      >
        <ArrowUpDown :size="15" />
        {{ sortKey === 'createdAt' ? 'Mới nhất' : 'Nhiều click nhất' }}
      </button>
    </div>

    <div v-if="loading" class="text-on-surface-variant text-center py-10">Đang tải danh sách link...</div>

    <WxEmptyState
      v-else-if="links.length === 0"
      title="Chưa có shortlink nào"
      description="Bạn chưa tạo link nào. Hãy bắt đầu bằng cách rút gọn URL đầu tiên của bạn."
    >
      <template #action>
        <WxButton variant="primary" to="/app/links/create">Tạo Link ngay</WxButton>
      </template>
    </WxEmptyState>

    <div v-else-if="filtered.length === 0" class="bg-surface-container rounded-2xl border border-outline p-8 text-center">
      <Search :size="32" class="text-on-surface-variant mx-auto mb-2 opacity-40" />
      <p class="text-on-surface-variant text-sm">Không tìm thấy link nào khớp với "<strong>{{ search }}</strong>"</p>
    </div>

    <div v-else class="grid gap-4">
      <WxCard v-for="link in filtered" :key="link.id" class="p-4 flex flex-col gap-3 hover:border-primary/30 transition-colors group">
        <!-- Header row -->
        <div class="flex items-start justify-between gap-3">
          <div class="flex items-center gap-2 min-w-0">
            <Link2 :size="16" class="text-primary shrink-0" />
            <a :href="link.shortUrl" target="_blank" class="font-bold text-primary hover:underline truncate text-sm">
              {{ link.shortUrl }}
            </a>
          </div>
          <WxBadge :variant="link.status === 'Active' ? 'success' : 'warning'" class="shrink-0">
            {{ link.status === 'Active' ? 'Active' : link.status === 'Paused' ? 'Tạm dừng' : link.status }}
          </WxBadge>
        </div>

        <!-- Destination -->
        <p class="text-xs text-on-surface-variant truncate">
          → {{ link.originalUrl }}
        </p>

        <!-- Stats -->
        <div class="flex items-center gap-4 text-xs text-on-surface-variant">
          <span class="flex items-center gap-1"><MousePointerClick :size="12" /> {{ link.totalClicks ?? 0 }} clicks</span>
          <span>{{ link.uniqueClicks ?? 0 }} unique</span>
          <span v-if="link.createdAtUtc" class="ml-auto">{{ new Date(link.createdAtUtc).toLocaleDateString('vi-VN') }}</span>
        </div>

        <!-- Actions -->
        <div class="flex flex-wrap gap-2 pt-1 border-t border-outline">
          <WxButton variant="ghost" size="sm" :to="`/app/links/${link.id}`">Chi tiết</WxButton>
          <button
            @click="copyUrl(link)"
            class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium text-on-surface-variant hover:bg-surface-container-high border border-outline transition-colors"
          >
            <Copy :size="12" />
            {{ copySuccessId === link.id ? '✓ Đã sao chép' : 'Sao chép link' }}
          </button>
          <a :href="link.shortUrl" target="_blank"
            class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium text-on-surface-variant hover:bg-surface-container-high border border-outline transition-colors">
            <ExternalLink :size="12" /> Mở thử
          </a>
          <WxButton
            :variant="link.status === 'Active' ? 'danger' : 'success'"
            size="sm"
            @click="toggleStatus(link)"
          >
            {{ link.status === 'Active' ? 'Tạm dừng' : 'Kích hoạt' }}
          </WxButton>
        </div>
      </WxCard>
    </div>
  </div>
</template>
