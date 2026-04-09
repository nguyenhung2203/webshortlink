<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminAuditLog } from '@/types/api'
import { ClipboardList, AlertCircle, RefreshCw, Activity, Terminal } from 'lucide-vue-next'

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
  <div class="ui-root">
    
    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><ClipboardList :size="13" /> Nhật ký Quản trị</div>
        <h1 class="ui-title">Audit Logs</h1>
        <p class="ui-subtitle">Truy vết hành động can thiệp hệ thống và các thay đổi dữ liệu cấu hình quan trọng.</p>
      </div>
      <div>
        <button class="ui-btn ui-btn-outline" @click="load" :disabled="loading" style="color: #0f172a;">
          <RefreshCw :size="14" :class="{'animate-spin': loading}" /> Khôi phục Logs
        </button>
      </div>
    </div>

    <div v-if="error" class="ui-alert ui-alert-error" style="background: white;">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <div v-if="loading" class="ui-skeleton" style="height: 400px; border-radius: 12px;" />

    <div v-else-if="logs.length === 0" class="ui-empty" style="background: white;">
      <div class="ui-empty-icon" style="opacity: 0.5;"><Terminal :size="48" /></div>
      <h3 class="ui-empty-title">Trống nhật ký</h3>
      <p class="ui-empty-desc">Chưa có hoạt động quản trị nào được ghi nhận trên hệ thống.</p>
    </div>

    <div v-else class="ui-panel" style="overflow: hidden; padding: 0;">
      
      <div class="ui-panel-header" style="padding: 1.25rem 1.5rem; border-bottom: 1px solid #f1f5f9; background: #fafafa;">
        <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;"><Activity :size="16" style="color: #475569;" /> Dữ liệu truy vết hệ thống</h3>
      </div>

      <div style="overflow-x: auto;">
        <table style="width: 100%; border-collapse: collapse; min-width: 800px; text-align: left;">
          <thead style="background: #f8fafc; border-bottom: 1px solid #e2e8f0;">
            <tr>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Mã Hành động / Tác nhân</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Mục tiêu (Resource)</th>
              <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; text-align: right;">Ghi nhận lúc</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="log in logs" :key="log.id" style="border-bottom: 1px solid #f1f5f9; transition: background 0.15s;" class="hover:bg-slate-50">
              
              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; flex-direction: column; gap: 0.25rem;">
                  <span style="font-family: monospace; font-weight: 800; font-size: 0.9rem; color: #1e293b;">{{ log.action }}</span>
                  <div style="display: flex; items-center; gap: 0.5rem;">
                    <span style="font-size: 0.75rem; font-weight: 700; color: #475569; background: #f1f5f9; padding: 0.15rem 0.4rem; border-radius: 4px;">{{ log.actorType.toUpperCase() }}</span>
                  </div>
                </div>
              </td>
              
              <td style="padding: 1rem 1.5rem;">
                <div style="display: flex; flex-direction: column; gap: 0.15rem;">
                  <span style="font-size: 0.85rem; font-weight: 600; color: #0f172a;">{{ log.resourceType || 'Hệ thống' }}</span>
                  <span style="font-family: monospace; font-size: 0.8rem; color: #94a3b8;" :title="log.resourceId ?? undefined">{{ log.resourceId ? log.resourceId.substring(0, 15) + '...' : 'N/A' }}</span>
                </div>
              </td>
              
              <td style="padding: 1rem 1.5rem; text-align: right;">
                <span style="font-size: 0.85rem; font-weight: 500; color: #64748b; font-variant-numeric: tabular-nums;">
                  {{ formatDate(log.createdAtUtc) }}
                </span>
                <div style="font-size: 0.7rem; color: #cbd5e1; font-family: monospace; margin-top: 0.2rem;">
                  REF-{{ log.id.substring(0,6) }}
                </div>
              </td>

            </tr>
          </tbody>
        </table>
      </div>

    </div>
  </div>
</template>
