<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminAuditLog } from '@/types/api'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const authStore = useAuthStore()
const logs = ref<AdminAuditLog[]>([])
const loading = ref(true)
const error = ref('')

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    logs.value = await AdminService.getAuditLogs(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải audit logs.'
  } finally {
    loading.value = false
  }
}

function formatDate(dateStr: string) {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleString('vi-VN')
}

onMounted(load)
</script>

<template>
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Audit Logs"
      subtitle="Theo dõi hành động quản trị và thay đổi quan trọng trong hệ thống."
    />

    <p v-if="error" class="text-red-500 bg-red-50 p-4 rounded-md">{{ error }}</p>

    <div v-if="loading" class="text-gray-500 font-medium">
      Đang tải danh sách nhật ký...
    </div>

    <div v-else-if="!error" class="flex flex-col gap-4">
      <p v-if="logs.length === 0" class="text-gray-500 text-center py-8">
        Chưa có dữ liệu nhật ký hệ thống.
      </p>

      <div 
        v-for="log in logs" 
        :key="log.id" 
        class="bg-white p-4 rounded-xl border border-gray-100 shadow-sm flex flex-col md:flex-row md:items-center justify-between gap-4 transition-shadow hover:shadow-md"
      >
        <div class="flex gap-4">
          <div>
            <strong class="text-gray-900 block">{{ log.action }}</strong>
            <p class="text-sm text-gray-500 mt-1">
              {{ log.resourceType }} &bull; {{ log.resourceId }} &bull; Bởi: {{ log.actorType }}
            </p>
          </div>
        </div>
        <div class="text-right shrink-0">
          <span class="text-sm text-gray-500 bg-gray-50 px-3 py-1 rounded-full border border-gray-100">
            {{ formatDate(log.createdAtUtc) }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>
