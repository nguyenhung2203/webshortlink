<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { LinkService, UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { LinkDetail } from '@/types/api'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import LinkRulesPanel from '@/components/user/LinkRulesPanel.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const loading = ref(true)
const actionLoading = ref(false)
const error = ref('')
const message = ref('')
const detail = ref<LinkDetail | null>(null)
const currentPlanId = ref(1)

const linkId = computed(() => String(route.params.id ?? ''))
const isPro = computed(() => currentPlanId.value >= 2)

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    const [linkData, subscription] = await Promise.all([
      LinkService.detail(authStore.accessToken, linkId.value),
      UserService.getSubscription(authStore.accessToken),
    ])
    detail.value = linkData
    currentPlanId.value = (subscription as any)?.planId ?? 1
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải chi tiết link.'
  } finally {
    loading.value = false
  }
}

async function toggleStatus() {
  if (!authStore.accessToken || !detail.value) return
  actionLoading.value = true
  error.value = ''
  message.value = ''
  try {
    const newStatus = detail.value.status !== 'Active'
    await LinkService.updateStatus(authStore.accessToken, detail.value.id, newStatus)
    message.value = newStatus ? 'Đã kích hoạt lại link.' : 'Đã tạm dừng link.'
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật trạng thái link.'
  } finally {
    actionLoading.value = false
  }
}

async function deleteLink() {
  if (!authStore.accessToken || !detail.value) return
  actionLoading.value = true
  error.value = ''
  message.value = ''
  try {
    const response = await LinkService.delete(authStore.accessToken, detail.value.id)
    message.value = response.message
    router.push('/app/links')
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể xóa link.'
  } finally {
    actionLoading.value = false
  }
}

const linkHost = computed(() => {
  if (!detail.value) return ''
  try {
    return new URL(detail.value.shortUrl).host
  } catch {
    return 'Liên kết không hợp lệ'
  }
})

const exportLoading = ref(false)
async function exportCsv() {
  if (!authStore.accessToken || !detail.value) return
  exportLoading.value = true
  try {
    await UserService.exportLinkAnalyticsCsv(authStore.accessToken, detail.value.id)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể xuất dữ liệu.'
  } finally {
    exportLoading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Chi tiết Link"
      description="Xem cấu hình, giới hạn và quyền sử dụng của shortlink."
    >
      <template #actions>
        <WxButton v-if="isPro" variant="secondary" :disabled="exportLoading" @click="exportCsv">
          {{ exportLoading ? 'Đang xuất...' : 'Export CSV' }}
        </WxButton>
        <WxButton variant="secondary" @click="router.push('/app/links')">Quậy lại danh sách</WxButton>
      </template>
    </WxPageHeader>

    <p v-if="message" class="rounded-lg border border-success/20 bg-success/10 p-3 text-sm font-medium text-success">
      {{ message }}
    </p>
    <p v-if="error && !loading" class="text-sm text-danger">{{ error }}</p>

    <div v-if="loading" class="text-sm text-on-surface-variant">Đang tải chi tiết link...</div>

    <WxEmptyState
      v-else-if="!detail"
      title="Không tìm thấy link"
      description="Link này có thể đã bị xóa hoặc bạn không còn quyền truy cập."
    >
      <template #action>
        <WxButton variant="primary" @click="router.push('/app/links')">Về trang Links</WxButton>
      </template>
    </WxEmptyState>

    <template v-else>
      <WxCard padding="md">
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <div class="space-y-2">
            <div class="flex items-center gap-3">
              <h2 class="text-xl font-bold text-primary">{{ detail.shortUrl }}</h2>
              <WxBadge :variant="detail.status === 'Active' ? 'success' : 'warning'">
                {{ detail.status }}
              </WxBadge>
            </div>
            <p class="break-all text-sm text-on-surface-variant">{{ detail.originalUrl }}</p>
            <div class="flex flex-wrap gap-4 text-sm text-on-surface-variant">
              <span>Slug: <strong class="text-on-surface">{{ detail.slug }}</strong></span>
              <span>Tổng click: <strong class="text-on-surface">{{ detail.totalClicks }}</strong></span>
              <span>Click duy nhất: <strong class="text-on-surface">{{ detail.uniqueClicks }}</strong></span>
            </div>
          </div>

          <div class="flex flex-wrap gap-3">
            <WxButton
              :variant="detail.status === 'Active' ? 'danger' : 'success'"
              :loading="actionLoading"
              @click="toggleStatus"
            >
              {{ detail.status === 'Active' ? 'Tạm dừng Link' : 'Kích hoạt lại' }}
            </WxButton>
            <WxButton variant="ghost" @click="router.push(`/app/analytics?linkId=${detail.id}`)">
              Xem analytics
            </WxButton>
            <WxButton variant="danger" :loading="actionLoading" @click="deleteLink">
              Xóa mềm
            </WxButton>
          </div>
        </div>
      </WxCard>

      <div class="grid gap-6 lg:grid-cols-2">
        <!-- Cấu hình -->
        <WxCard padding="md">
          <div class="space-y-4">
            <h3 class="text-lg font-semibold text-on-surface">Cấu hình Link</h3>
            <div class="grid gap-3 text-sm">
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Mật khẩu bảo vệ</span>
                <strong>{{ detail.hasPassword ? 'Đã bật' : 'Không' }}</strong>
              </div>
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Hết hạn</span>
                <strong>{{ detail.expiresAtUtc || 'Không giới hạn' }}</strong>
              </div>
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Click limit</span>
                <strong>{{ detail.clickLimit ?? 'Không giới hạn' }}</strong>
              </div>
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Tag</span>
                <strong>{{ detail.tag || '—' }}</strong>
              </div>
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Mô tả</span>
                <strong>{{ detail.description || '—' }}</strong>
              </div>
            </div>
          </div>
        </WxCard>

        <!-- Thông tin host -->
        <WxCard padding="md">
          <div class="space-y-4">
            <h3 class="text-lg font-semibold text-on-surface">Thông tin bổ sung</h3>
            <div class="grid gap-3 text-sm">
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Host chuyển hướng</span>
                <strong>{{ linkHost }}</strong>
              </div>
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Ngày tạo</span>
                <strong>{{ new Date(detail.createdAtUtc).toLocaleDateString('vi-VN') }}</strong>
              </div>
              <div v-if="detail.updatedAtUtc" class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Cập nhật lần cuối</span>
                <strong>{{ new Date(detail.updatedAtUtc).toLocaleDateString('vi-VN') }}</strong>
              </div>
            </div>
          </div>
        </WxCard>
      </div>
        <!-- Link Rules -->
        <LinkRulesPanel
          :link-id="detail.id"
          :is-pro="isPro"
        />
    </template>
  </div>
</template>
