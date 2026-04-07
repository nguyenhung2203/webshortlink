<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { LinkService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { LinkDetail } from '@/types/api'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxEmptyState from '@/components/ui/WxEmptyState.vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const loading = ref(true)
const actionLoading = ref(false)
const error = ref('')
const message = ref('')
const detail = ref<LinkDetail | null>(null)

const linkId = computed(() => String(route.params.id ?? ''))
const summary = computed(() => detail.value?.summary ?? null)

async function load() {
  loading.value = true
  error.value = ''

  try {
    if (!authStore.token) {
      throw new Error('Chưa xác thực.')
    }

    detail.value = await LinkService.detail(authStore.token, linkId.value)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải chi tiết link.'
  } finally {
    loading.value = false
  }
}

async function toggleStatus() {
  if (!authStore.token || !summary.value) {
    return
  }

  actionLoading.value = true
  error.value = ''
  message.value = ''

  try {
    if (summary.value.status === 'Active') {
      await LinkService.pause(authStore.token, summary.value.id)
      message.value = 'Đã tạm dừng link.'
    } else {
      await LinkService.resume(authStore.token, summary.value.id)
      message.value = 'Đã kích hoạt lại link.'
    }

    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật trạng thái link.'
  } finally {
    actionLoading.value = false
  }
}

async function deleteLink() {
  if (!authStore.token || !summary.value) {
    return
  }

  actionLoading.value = true
  error.value = ''
  message.value = ''

  try {
    const response = await LinkService.delete(authStore.token, summary.value.id)
    message.value = response.message
    router.push('/app/links')
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể xóa link.'
  } finally {
    actionLoading.value = false
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
        <WxButton variant="secondary" @click="router.push('/app/links')">Quay lại danh sách</WxButton>
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
              <h2 class="text-xl font-bold text-primary">{{ detail.summary.shortUrl }}</h2>
              <WxBadge :variant="detail.summary.status === 'Active' ? 'success' : 'warning'">
                {{ detail.summary.status }}
              </WxBadge>
            </div>
            <p class="break-all text-sm text-on-surface-variant">{{ detail.summary.originalUrl }}</p>
            <div class="flex flex-wrap gap-4 text-sm text-on-surface-variant">
              <span>Slug: <strong class="text-on-surface">{{ detail.summary.slug }}</strong></span>
              <span>Tổng click: <strong class="text-on-surface">{{ detail.summary.totalClicks }}</strong></span>
              <span>Click duy nhất: <strong class="text-on-surface">{{ detail.summary.uniqueClicks }}</strong></span>
              <span>Bot: <strong class="text-on-surface">{{ detail.summary.botClicks }}</strong></span>
            </div>
          </div>

          <div class="flex flex-wrap gap-3">
            <WxButton
              :variant="detail.summary.status === 'Active' ? 'danger' : 'success'"
              :loading="actionLoading"
              @click="toggleStatus"
            >
              {{ detail.summary.status === 'Active' ? 'Tạm dừng Link' : 'Kích hoạt lại' }}
            </WxButton>
            <WxButton variant="ghost" @click="router.push(`/app/analytics?linkId=${detail.summary.id}`)">
              Xem analytics
            </WxButton>
            <WxButton variant="danger" :loading="actionLoading" @click="deleteLink">
              Xóa mềm
            </WxButton>
          </div>
        </div>
      </WxCard>

      <div class="grid gap-6 lg:grid-cols-2">
        <WxCard padding="md">
          <div class="space-y-4">
            <h3 class="text-lg font-semibold text-on-surface">Cấu hình Link</h3>
            <div class="grid gap-3 text-sm">
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Mật khẩu bảo vệ</span>
                <strong>{{ detail.settings.password ? 'Đã bật' : 'Không' }}</strong>
              </div>
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Hết hạn</span>
                <strong>{{ detail.settings.expiredAtUtc || 'Không giới hạn' }}</strong>
              </div>
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Click limit</span>
                <strong>{{ detail.settings.clickLimit ?? 'Không giới hạn' }}</strong>
              </div>
            </div>
          </div>
        </WxCard>

        <WxCard padding="md">
          <div class="space-y-4">
            <h3 class="text-lg font-semibold text-on-surface">Quyền theo gói</h3>
            <div class="grid gap-3 text-sm">
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Custom domain</span>
                <WxBadge :variant="detail.permissions.canUseCustomDomain ? 'success' : 'neutral'">
                  {{ detail.permissions.canUseCustomDomain ? 'Có' : 'Không' }}
                </WxBadge>
              </div>
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">Advanced analytics</span>
                <WxBadge :variant="detail.permissions.canUseAdvancedAnalytics ? 'success' : 'neutral'">
                  {{ detail.permissions.canUseAdvancedAnalytics ? 'Có' : 'Không' }}
                </WxBadge>
              </div>
              <div class="flex items-center justify-between gap-4">
                <span class="text-on-surface-variant">API access</span>
                <WxBadge :variant="detail.permissions.canUseApi ? 'success' : 'neutral'">
                  {{ detail.permissions.canUseApi ? 'Có' : 'Không' }}
                </WxBadge>
              </div>
            </div>
          </div>
        </WxCard>
      </div>
    </template>
  </div>
</template>
