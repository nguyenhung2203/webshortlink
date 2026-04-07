<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminUser } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxButton from '@/components/ui/WxButton.vue'

const authStore = useAuthStore()
const users = ref<AdminUser[]>([])
const error = ref('')

async function load() {
  try {
    if (!authStore.token) throw new Error('Chưa xác thực')
    users.value = await AdminService.getUsers(authStore.token)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải users.'
  }
}

async function toggle(user: AdminUser) {
  const action = user.status === 'Active' ? 'lock' : 'unlock'

  try {
    if (!authStore.token) throw new Error('Chưa xác thực')
    await AdminService.toggleUserLock(authStore.token, user.id, action)
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể cập nhật user.'
  }
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Khách hàng"
      subtitle="Quản lý tài khoản, gói dịch vụ và trạng thái kích hoạt."
    />

    <p v-if="error" class="text-danger text-sm">{{ error }}</p>

    <div class="grid gap-4">
      <WxCard v-for="user in users" :key="user.id" class="p-5">
        <div class="flex flex-col md:flex-row justify-between md:items-center gap-4">
          <div class="flex flex-col gap-1">
            <strong class="text-lg text-on-surface">{{ user.email }}</strong>
            <p class="text-sm text-on-surface-variant">{{ user.fullName }} • <span class="font-bold text-primary">{{ user.plan }}</span></p>
            <p class="text-xs text-on-surface-variant mt-1">Sở hữu {{ user.links }} links • Thu hút {{ user.totalClicks }} clicks</p>
          </div>
          <div class="flex gap-4 items-center">
            <WxBadge :variant="user.status === 'Active' ? 'success' : 'warning'">
              {{ user.status === 'Active' ? 'Hoạt động' : 'Đã khóa' }}
            </WxBadge>
            <WxButton 
              :variant="user.status === 'Active' ? 'danger' : 'success'" 
              @click="toggle(user)"
              size="sm"
            >
              {{ user.status === 'Active' ? 'Khóa tài khoản' : 'Mở khóa' }}
            </WxButton>
          </div>
        </div>
      </WxCard>
    </div>
  </div>
</template>
