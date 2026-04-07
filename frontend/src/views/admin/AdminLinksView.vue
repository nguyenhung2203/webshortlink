<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminLink } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import { Search, Link2, ChevronRight, Ban, CheckCircle, MousePointerClick } from 'lucide-vue-next'
import { RouterLink } from 'vue-router'

const authStore = useAuthStore()
const data = ref<AdminLink[]>([])
const error = ref('')
const isActionLoading = ref(false)
const search = ref('')

async function load() {
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    data.value = await AdminService.getLinks(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách links.'
  }
}

async function toggleLinkStatus(link: AdminLink) {
  if (isActionLoading.value || !authStore.accessToken) return
  const action = link.status === 'Active' ? 'block' : 'unblock'
  if (!confirm(`${action === 'block' ? 'Vô hiệu hóa' : 'Kích hoạt lại'} link /${link.slug}?`)) return
  isActionLoading.value = true
  try {
    await AdminService.toggleLinkBlock(authStore.accessToken, link.id, action)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi thay đổi trạng thái link.')
  } finally {
    isActionLoading.value = false
  }
}

const filtered = computed(() => {
  if (!search.value.trim()) return data.value
  const q = search.value.trim().toLowerCase()
  return data.value.filter(l =>
    (l.slug || '').toLowerCase().includes(q) ||
    (l.userEmail || '').toLowerCase().includes(q) ||
    (l.shortUrl || '').toLowerCase().includes(q)
  )
})

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader title="Quản lý Links" description="Giám sát và quản lý toàn bộ link của hệ thống." />

    <div v-if="error" class="text-danger bg-danger/10 rounded-xl p-4 text-sm">{{ error }}</div>

    <!-- Search -->
    <div class="relative">
      <Search :size="16" class="absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant" />
      <input
        v-model="search"
        type="text"
        placeholder="Tìm theo slug, email chủ sở hữu..."
        class="w-full h-10 pl-9 pr-4 rounded-xl border border-outline bg-surface-container text-sm focus:outline-none focus:border-primary"
      />
    </div>

    <!-- Desktop table -->
    <div class="hidden md:block bg-surface-container rounded-xl border border-outline overflow-hidden relative">
      <div v-if="isActionLoading" class="absolute inset-0 bg-surface/50 z-10 flex items-center justify-center">
        <span class="text-on-surface-variant text-sm font-medium">Đang xử lý...</span>
      </div>
      <table class="w-full text-left text-sm">
        <thead class="bg-surface-container-high text-on-surface-variant border-b border-outline">
          <tr>
            <th class="p-4 font-semibold">Slug</th>
            <th class="p-4 font-semibold">Chủ sở hữu</th>
            <th class="p-4 font-semibold">Clicks</th>
            <th class="p-4 font-semibold">Bot</th>
            <th class="p-4 font-semibold">Risk</th>
            <th class="p-4 font-semibold">Trạng thái</th>
            <th class="p-4 font-semibold">Ngày tạo</th>
            <th class="p-4 font-semibold">Thao tác</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-outline">
          <tr v-for="link in filtered" :key="link.id" class="hover:bg-surface-container-low">
            <td class="p-4">
              <a :href="link.shortUrl" target="_blank" class="text-primary font-mono hover:underline">{{ link.slug }}</a>
            </td>
            <td class="p-4 text-on-surface-variant">{{ link.userEmail }}</td>
            <td class="p-4 text-on-surface">{{ link.totalClicks }}</td>
            <td class="p-4 text-on-surface-variant">{{ link.botClicks }}</td>
            <td class="p-4">
              <span v-if="link.highestRiskScore" :class="link.highestRiskScore > 70 ? 'text-danger' : 'text-warning'" class="font-bold">
                {{ link.highestRiskScore }}
              </span>
              <span v-else class="text-on-surface-variant">—</span>
            </td>
            <td class="p-4">
              <span :class="link.status === 'Active' ? 'text-success bg-success/10' : 'text-on-surface-variant bg-surface-container-high'" class="px-2 py-0.5 rounded-full text-xs font-bold">
                {{ link.status }}
              </span>
            </td>
            <td class="p-4 text-on-surface-variant">{{ new Date(link.createdAtUtc).toLocaleDateString('vi-VN') }}</td>
            <td class="p-4">
              <div class="flex items-center gap-3">
                <RouterLink :to="{ name: 'admin-link-detail', params: { id: link.id } }" class="text-primary font-semibold text-sm hover:underline">Chi tiết</RouterLink>
                <button
                  @click="toggleLinkStatus(link)"
                  :disabled="isActionLoading"
                  :class="link.status === 'Active' ? 'text-danger' : 'text-success'"
                  class="font-semibold text-sm transition-colors"
                >
                  {{ link.status === 'Active' ? 'Khóa' : 'Mở' }}
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="filtered.length === 0">
            <td colspan="8" class="p-8 text-center text-on-surface-variant">{{ search ? 'Không tìm thấy kết quả.' : 'Chưa có dữ liệu link.' }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Mobile card list -->
    <div class="flex flex-col gap-3 md:hidden">
      <div v-if="filtered.length === 0" class="text-center text-on-surface-variant text-sm py-6">
        {{ search ? 'Không tìm thấy kết quả.' : 'Chưa có dữ liệu link.' }}
      </div>
      <div v-for="link in filtered" :key="link.id + '-mobile'"
        class="bg-surface-container rounded-2xl border border-outline p-4 flex flex-col gap-3"
      >
        <div class="flex items-start justify-between gap-2">
          <div class="flex items-center gap-2 min-w-0">
            <div class="w-9 h-9 rounded-xl bg-primary/10 flex items-center justify-center shrink-0">
              <Link2 :size="16" class="text-primary" />
            </div>
            <div class="min-w-0">
              <a :href="link.shortUrl" target="_blank" class="font-mono font-bold text-primary text-sm hover:underline">{{ link.slug }}</a>
              <p class="text-xs text-on-surface-variant truncate">{{ link.userEmail }}</p>
            </div>
          </div>
          <span :class="link.status === 'Active' ? 'text-success bg-success/10' : 'text-on-surface-variant bg-surface-container-high'" class="px-2 py-0.5 rounded-full text-xs font-bold shrink-0">
            {{ link.status }}
          </span>
        </div>
        <div class="flex items-center gap-4 text-xs text-on-surface-variant">
          <span class="flex items-center gap-1"><MousePointerClick :size="12" /> {{ link.totalClicks }} clicks</span>
          <span>Bot: {{ link.botClicks }}</span>
          <span v-if="link.highestRiskScore" :class="link.highestRiskScore > 70 ? 'text-danger' : 'text-warning'" class="font-bold">Risk: {{ link.highestRiskScore }}</span>
        </div>
        <div class="flex items-center gap-2 pt-1 border-t border-outline">
          <RouterLink :to="{ name: 'admin-link-detail', params: { id: link.id } }" class="flex items-center gap-1 text-primary font-semibold text-sm">
            Chi tiết <ChevronRight :size="14" />
          </RouterLink>
          <button
            @click="toggleLinkStatus(link)"
            :disabled="isActionLoading"
            class="flex items-center gap-1 text-sm font-semibold ml-auto"
            :class="link.status === 'Active' ? 'text-danger' : 'text-success'"
          >
            <Ban v-if="link.status === 'Active'" :size="13" />
            <CheckCircle v-else :size="13" />
            {{ link.status === 'Active' ? 'Vô hiệu hóa' : 'Kích hoạt' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
