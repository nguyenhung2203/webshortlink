<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { ShieldCheck, Activity, AlertTriangle, ShieldAlert, Lock, Server, RefreshCw } from 'lucide-vue-next'

const authStore = useAuthStore()
const security = ref<any>(null)
const error = ref('')
const loading = ref(true)

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    security.value = await AdminService.getSecurity(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Module bảo mật tạm thời không phản hồi.'
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="ui-root">
    
    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><ShieldCheck :size="13" /> SOC Dashboard</div>
        <h1 class="ui-title">Phòng thủ & Bảo mật</h1>
        <p class="ui-subtitle">Giám sát các chỉ số an toàn hệ thống, xâm nhập bất hợp pháp và tình trạng máy chủ.</p>
      </div>
      <div>
        <button class="ui-btn ui-btn-outline" @click="load" :disabled="loading" style="color: #0f172a;">
          <RefreshCw :size="14" :class="{'animate-spin': loading}" /> Resync SOC
        </button>
      </div>
    </div>

    <div v-if="error" class="ui-alert ui-alert-error" style="background: white;">
      <ShieldAlert :size="16" /> {{ error }}
    </div>

    <div v-if="loading" style="display: flex; flex-direction: column; gap: 1.5rem;">
      <div class="ui-card-grid-3">
        <div class="ui-skeleton" style="height: 120px;" v-for="i in 3" :key="`s-${i}`" />
      </div>
      <div class="ui-skeleton" style="height: 250px; border-radius: 12px;" />
    </div>

    <template v-else-if="security">
      
      <div style="display: flex; flex-direction: column; gap: 1.5rem;">
        
        <!-- Danger metrics -->
        <div class="ui-card-grid ui-card-grid-3">
          
          <div class="ui-panel" style="background: white; border-top: 3px solid #ef4444;">
            <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
              <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                <span style="font-size: 0.8rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Failed Logins</span>
                <AlertTriangle :size="18" style="color: #ef4444;" />
              </div>
              <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">{{ (security.failedLoginsToday ?? 0).toLocaleString() }}</div>
            </div>
          </div>

          <div class="ui-panel" style="background: white; border-top: 3px solid #f59e0b;">
            <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
              <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                <span style="font-size: 0.8rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Suspicious Traffic</span>
                <Activity :size="18" style="color: #f59e0b;" />
              </div>
              <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">{{ (security.suspiciousClicks ?? 0).toLocaleString() }}</div>
            </div>
          </div>

          <div class="ui-panel" style="background: white; border-top: 3px solid #64748b;">
            <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
              <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                <span style="font-size: 0.8rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Locked Identities</span>
                <Lock :size="18" style="color: #64748b;" />
              </div>
              <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">{{ (security.lockedAccounts ?? 0).toLocaleString() }}</div>
            </div>
          </div>

        </div>

        <!-- Infrastructure Health -->
        <div v-if="security.systemHealth" class="ui-panel" style="background: white; padding: 0;">
          <div class="ui-panel-header" style="padding: 1.25rem 1.5rem; border-bottom: 1px solid #f1f5f9;">
            <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;"><Server :size="16" style="color: #10b981;" /> Sức khỏe Máy chủ & Dịch vụ</h3>
          </div>
          <div class="ui-panel-body" style="padding: 0;">
            <table style="width: 100%; border-collapse: collapse; text-align: left;">
              <thead style="background: #f8fafc; border-bottom: 1px solid #e2e8f0;">
                <tr>
                  <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Node / Service Component</th>
                  <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Chỉ số Hiện Tại</th>
                  <th style="padding: 1rem 1.5rem; font-size: 0.75rem; font-weight: 700; color: #64748b; text-transform: uppercase; text-align: right;">Trạng thái (Health)</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, idx) in security.systemHealth" :key="idx" style="border-bottom: 1px solid #f1f5f9;">
                  <td style="padding: 1rem 1.5rem; font-size: 0.9rem; font-weight: 600; color: #0f172a;">{{ item.name }}</td>
                  <td style="padding: 1rem 1.5rem;">
                    <span style="font-family: monospace; font-size: 0.85rem; background: #f1f5f9; padding: 0.2rem 0.5rem; border-radius: 4px; color: #475569;">{{ item.value }}</span>
                  </td>
                  <td style="padding: 1rem 1.5rem; text-align: right;">
                    <div style="display: flex; align-items: center; justify-content: flex-end; gap: 0.5rem;">
                      <span v-if="item.status === 'OK'" class="ui-badge ui-badge-success" style="font-weight: 700;">OK</span>
                      <span v-else class="ui-badge ui-badge-warning" style="font-weight: 700;">{{ item.status }}</span>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

      </div>
    </template>
  </div>
</template>
