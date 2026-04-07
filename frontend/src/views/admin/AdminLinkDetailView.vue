<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { ArrowLeft, Link2, Ban, CheckCircle, ExternalLink } from 'lucide-vue-next'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const linkId = route.params.id as string
const link = ref<any>(null)
const error = ref('')
const loading = ref(true)
const actionLoading = ref(false)

async function load() {
  loading.value = true
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    link.value = await AdminService.getLinkDetail(authStore.accessToken, linkId)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải thông tin link.'
  } finally {
    loading.value = false
  }
}

async function toggleBlock() {
  if (!authStore.accessToken || !link.value) return
  const action: 'block' | 'unblock' = link.value.status === 'DisabledByAdmin' ? 'unblock' : 'block'
  if (!confirm(`${action === 'block' ? 'Vô hiệu hóa' : 'Kích hoạt lại'} link này?`)) return
  actionLoading.value = true
  try {
    await AdminService.toggleLinkBlock(authStore.accessToken, linkId, action)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi thao tác.')
  } finally {
    actionLoading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <div>
      <button @click="router.back()" class="flex items-center gap-1.5 text-sm text-on-surface-variant hover:text-on-surface transition-colors">
        <ArrowLeft :size="16" /> Quay lại
      </button>
    </div>

    <WxPageHeader title="Chi tiết Link" description="Thông tin chi tiết và quản trị đường dẫn rút gọn." />

    <div v-if="loading" class="text-center py-10 text-on-surface-variant">Đang tải...</div>
    <div v-else-if="error" class="text-danger bg-danger/10 rounded-xl p-4">{{ error }}</div>

    <template v-else-if="link">
      <!-- Link Card -->
      <div class="bg-surface-container rounded-2xl border border-outline p-6">
        <div class="flex items-start gap-4">
          <div class="w-12 h-12 rounded-xl bg-primary/10 flex items-center justify-center shrink-0">
            <Link2 :size="22" class="text-primary" />
          </div>
          <div class="flex-1 min-w-0">
            <div class="flex flex-wrap gap-2 mb-2">
              <span class="text-xs font-bold px-2.5 py-1 rounded-full" :class="link.status === 'Active' ? 'bg-success/10 text-success' : 'bg-danger/10 text-danger'">
                {{ link.status }}
              </span>
              <span v-if="link.hasPassword" class="text-xs font-bold px-2.5 py-1 rounded-full bg-warning/10 text-warning">🔒 Có mật khẩu</span>
              <span v-if="link.expiresAtUtc" class="text-xs font-bold px-2.5 py-1 rounded-full bg-surface-container-high text-on-surface-variant">⏱ Có hạn sử dụng</span>
            </div>
            <h2 class="text-base font-bold text-on-surface mb-1">{{ link.shortUrl }}</h2>
            <a :href="link.originalUrl" target="_blank" class="text-sm text-primary hover:underline flex items-center gap-1 truncate">
              {{ link.originalUrl }} <ExternalLink :size="12" class="shrink-0" />
            </a>
          </div>
        </div>
      </div>

      <!-- Stats -->
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-4">
        <div class="bg-surface-container rounded-xl border border-outline p-4 text-center">
          <p class="text-2xl font-extrabold text-on-surface">{{ link.totalClicks ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant mt-1">Tổng Clicks</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-4 text-center">
          <p class="text-2xl font-extrabold text-on-surface">{{ link.uniqueClicks ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant mt-1">Unique Clicks</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-4 text-center">
          <p class="text-sm font-bold text-on-surface">{{ link.createdAtUtc ? new Date(link.createdAtUtc).toLocaleDateString('vi-VN') : '-' }}</p>
          <p class="text-xs text-on-surface-variant mt-1">Ngày tạo</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-4 text-center">
          <p class="text-sm font-bold text-on-surface">{{ link.expiresAtUtc ? new Date(link.expiresAtUtc).toLocaleDateString('vi-VN') : 'Không' }}</p>
          <p class="text-xs text-on-surface-variant mt-1">Hết hạn</p>
        </div>
      </div>

      <!-- Detail info -->
      <div class="bg-surface-container rounded-2xl border border-outline p-6 grid grid-cols-1 sm:grid-cols-2 gap-4 text-sm">
        <div><span class="text-on-surface-variant">Chủ sở hữu:</span> <span class="font-semibold text-on-surface ml-1">{{ link.ownerEmail || '-' }}</span></div>
        <div><span class="text-on-surface-variant">Slug:</span> <span class="font-mono font-semibold text-on-surface ml-1">{{ link.slug }}</span></div>
        <div><span class="text-on-surface-variant">Domain:</span> <span class="font-semibold text-on-surface ml-1">{{ link.domain || 'sho.rt' }}</span></div>
        <div><span class="text-on-surface-variant">Tag:</span> <span class="font-semibold text-on-surface ml-1">{{ link.tag || '-' }}</span></div>
        <div><span class="text-on-surface-variant">Click limit:</span> <span class="font-semibold text-on-surface ml-1">{{ link.clickLimit ?? 'Không giới hạn' }}</span></div>
        <div><span class="text-on-surface-variant">Cập nhật:</span> <span class="font-semibold text-on-surface ml-1">{{ link.updatedAtUtc ? new Date(link.updatedAtUtc).toLocaleDateString('vi-VN') : '-' }}</span></div>
      </div>

      <!-- Admin action -->
      <div class="bg-surface-container rounded-2xl border border-outline p-6">
        <h3 class="font-semibold text-on-surface mb-4">Thao tác quản trị</h3>
        <button
          @click="toggleBlock"
          :disabled="actionLoading"
          class="flex items-center gap-2 px-4 py-2 text-sm rounded-lg border font-semibold transition-all"
          :class="link.status === 'DisabledByAdmin'
            ? 'border-success/50 text-success hover:bg-success/10'
            : 'border-danger/50 text-danger hover:bg-danger/10'"
        >
          <CheckCircle v-if="link.status === 'DisabledByAdmin'" :size="15" />
          <Ban v-else :size="15" />
          {{ link.status === 'DisabledByAdmin' ? 'Kích hoạt lại link' : 'Vô hiệu hóa link' }}
        </button>
      </div>
    </template>
  </div>
</template>
