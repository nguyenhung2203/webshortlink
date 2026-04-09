<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { BarChart2, Users, Link2, MousePointerClick, TrendingUp, AlertCircle, RefreshCw, PieChart } from 'lucide-vue-next'

const authStore = useAuthStore()
const reports = ref<any>(null)
const error = ref('')
const loading = ref(true)

async function load() {
  loading.value = true
  error.value = ''
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    reports.value = await AdminService.getReports(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Dịch vụ phân tích báo cáo đang bảo trì.'
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
        <div class="ui-eyebrow"><BarChart2 :size="13" /> Hệ thống phân tích</div>
        <h1 class="ui-title">Phân tích & Báo cáo</h1>
        <p class="ui-subtitle">Dữ liệu thống kê tăng trưởng khách hàng và việc nâng cấp gói dịch vụ.</p>
      </div>
      <div>
        <button class="ui-btn ui-btn-outline" :disabled="loading" @click="load">
          <RefreshCw :size="14" :class="{ 'animate-spin': loading }" /> Làm mới
        </button>
      </div>
    </div>

    <div v-if="error" class="ui-alert ui-alert-warning" style="background: white;">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <div v-if="loading" style="display: flex; flex-direction: column; gap: 1.5rem;">
      <div class="ui-card-grid-4">
        <div v-for="i in 4" :key="`r-${i}`" class="ui-skeleton" style="height: 120px;" />
      </div>
      <div class="ui-skeleton" style="height: 250px; border-radius: 12px;" />
    </div>

    <template v-else-if="reports">
      <div style="display: flex; flex-direction: column; gap: 1.5rem;">
        <div class="ui-card-grid ui-card-grid-4">
          <div class="ui-panel" style="background: white;">
            <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
              <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                <span style="font-size: 0.8rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Người dùng</span>
                <Users :size="18" style="color: #3b82f6;" />
              </div>
              <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">{{ (reports.totalUsers ?? 0).toLocaleString() }}</div>
            </div>
          </div>

          <div class="ui-panel" style="background: white;">
            <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
              <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                <span style="font-size: 0.8rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Lượng URL</span>
                <Link2 :size="18" style="color: #10b981;" />
              </div>
              <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">{{ (reports.totalLinks ?? 0).toLocaleString() }}</div>
            </div>
          </div>

          <div class="ui-panel" style="background: white;">
            <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
              <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                <span style="font-size: 0.8rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Traffic ingest</span>
                <MousePointerClick :size="18" style="color: #8b5cf6;" />
              </div>
              <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">{{ (reports.totalClicks ?? 0).toLocaleString() }}</div>
            </div>
          </div>

          <div class="ui-panel" style="background: white; border-top: 3px solid #f59e0b;">
            <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
              <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                <span style="font-size: 0.8rem; font-weight: 700; color: #64748b; text-transform: uppercase;">Subscriptions</span>
                <TrendingUp :size="18" style="color: #f59e0b;" />
              </div>
              <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">{{ (reports.activeSubscriptions ?? 0).toLocaleString() }}</div>
              <div style="margin-top: 0.75rem; font-size: 0.8rem; color: #64748b; font-weight: 500;">
                Tỷ lệ phủ:
                <strong style="color: #0f172a;">{{ reports.totalUsers > 0 ? ((reports.activeSubscriptions / reports.totalUsers) * 100).toFixed(1) : 0 }}%</strong>
              </div>
            </div>
          </div>
        </div>

        <div v-if="reports.planBreakdown" class="ui-panel" style="background: white; padding: 0;">
          <div class="ui-panel-header" style="padding: 1.25rem 1.5rem; border-bottom: 1px solid #f1f5f9;">
            <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;">
              <PieChart :size="16" style="color: #3b82f6;" /> Phân bổ gói dịch vụ
            </h3>
          </div>
          <div class="ui-panel-body" style="padding: 1.5rem;">
            <div style="display: flex; flex-direction: column; gap: 1.25rem;">
              <div v-for="item in reports.planBreakdown" :key="item.planName" style="display: flex; align-items: center; gap: 1rem;">
                <span style="font-size: 0.85rem; font-weight: 700; color: #475569; width: 80px; flex-shrink: 0; text-transform: uppercase;">{{ item.planName }}</span>
                <div style="flex: 1; background: #e2e8f0; border-radius: 999px; height: 10px; overflow: hidden; display: flex;">
                  <div
                    :style="`width: ${Math.max(item.percent, 1)}%; background: ${item.planName.toLowerCase() === 'plus' ? '#f59e0b' : item.planName.toLowerCase() === 'pro' ? '#3b82f6' : '#94a3b8'};`"
                    style="height: 100%; border-radius: 999px; transition: width 1s ease-out;"
                  ></div>
                </div>
                <div style="display: flex; align-items: center; gap: 0.75rem; width: 100px; justify-content: flex-end;">
                  <span style="font-size: 0.95rem; font-weight: 800; color: #0f172a;">{{ item.count.toLocaleString() }}</span>
                  <span style="font-size: 0.75rem; font-weight: 600; color: #64748b; background: #f1f5f9; padding: 0.15rem 0.4rem; border-radius: 4px; min-width: 45px; text-align: center;">{{ item.percent.toFixed(1) }}%</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>
