<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { ShieldCheck, Activity, AlertTriangle } from 'lucide-vue-next'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const authStore = useAuthStore()
const security = ref<any>(null)
const error = ref('')
const loading = ref(true)

async function load() {
  loading.value = true
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    security.value = await AdminService.getSecurity(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải thông tin bảo mật.'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader title="Bảo mật & Vận hành" description="Giám sát sức khỏe hệ thống và các chỉ số bảo mật." />

    <div v-if="loading" class="text-center py-10 text-on-surface-variant">Đang tải dữ liệu bảo mật...</div>

    <div v-else-if="error" class="bg-surface-container rounded-2xl border border-outline p-8 text-center">
      <ShieldCheck :size="40" class="text-on-surface-variant mx-auto mb-3 opacity-40" />
      <p class="text-on-surface-variant text-sm">Dữ liệu bảo mật đang được phát triển.</p>
    </div>

    <template v-else-if="security">
      <!-- Status tiles -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div class="bg-surface-container rounded-xl border border-outline p-5 flex flex-col gap-2">
          <Activity :size="20" class="text-success" />
          <p class="text-2xl font-extrabold text-on-surface">{{ security.failedLoginsToday ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant">Đăng nhập thất bại hôm nay</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-5 flex flex-col gap-2">
          <AlertTriangle :size="20" class="text-warning" />
          <p class="text-2xl font-extrabold text-on-surface">{{ security.suspiciousClicks ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant">Click bot phát hiện</p>
        </div>
        <div class="bg-surface-container rounded-xl border border-outline p-5 flex flex-col gap-2">
          <ShieldCheck :size="20" class="text-primary" />
          <p class="text-2xl font-extrabold text-on-surface">{{ security.lockedAccounts ?? 0 }}</p>
          <p class="text-xs text-on-surface-variant">Tài khoản bị khóa</p>
        </div>
      </div>

      <!-- System health table -->
      <div v-if="security.systemHealth" class="bg-surface-container rounded-2xl border border-outline p-6">
        <h3 class="font-semibold text-on-surface mb-4">Sức khỏe hệ thống</h3>
        <div class="flex flex-col gap-2.5">
          <div v-for="item in security.systemHealth" :key="item.name" class="flex items-center justify-between py-2 border-b border-outline last:border-0">
            <span class="text-sm text-on-surface-variant">{{ item.name }}</span>
            <span class="text-sm font-bold text-on-surface flex items-center gap-2">
              <span class="w-2 h-2 rounded-full" :class="item.status === 'OK' ? 'bg-success' : 'bg-warning'"></span>
              {{ item.value }}
            </span>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>
