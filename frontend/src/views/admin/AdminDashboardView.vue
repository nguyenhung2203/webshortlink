<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { AdminService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import type { AdminDashboardDashboard } from '@/types/api'
import { Users, Link2, MousePointerClick, TrendingUp, ShieldAlert, Zap, ServerCog, Activity, AlertCircle } from 'lucide-vue-next'
import { RouterLink } from 'vue-router'

const authStore = useAuthStore()
const data = ref<AdminDashboardDashboard | null>(null)
const error = ref('')
const loading = ref(true)

async function load() {
  loading.value = true
  try {
    if (!authStore.accessToken) throw new Error('Chưa xác thực.')
    data.value = await AdminService.getDashboard(authStore.accessToken)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải dashboard admin.'
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
        <div class="ui-eyebrow"><Activity :size="13" /> Overview Dashboard</div>
        <h1 class="ui-title">Cổng tổng quan</h1>
        <p class="ui-subtitle">Giám sát các chỉ số kinh doanh, sản phẩm và vận hành lõi của hệ thống.</p>
      </div>
    </div>

    <div v-if="error" class="ui-alert ui-alert-error">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <!-- Loading Skeleton -->
    <div v-if="loading" style="display: flex; flex-direction: column; gap: 1.5rem;">
      <div class="ui-card-grid-4">
        <div class="ui-skeleton" style="height: 120px;" v-for="i in 4" :key="`s1-${i}`" />
      </div>
      <div class="ui-card-grid-3">
        <div class="ui-skeleton" style="height: 250px;" v-for="i in 3" :key="`s2-${i}`" />
      </div>
    </div>

    <template v-else-if="data">
      
      <div style="display: flex; flex-direction: column; gap: 2rem;">
        
        <!-- Primary KPI Cards -->
        <section>
          <div style="margin-bottom: 0.75rem; font-size: 0.85rem; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: 0.5px;">Chỉ số Cốt lõi (KPIs)</div>
          <div class="ui-card-grid ui-card-grid-4">
            
            <div class="ui-panel" style="border-top: 4px solid #3b82f6;">
              <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
                <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                  <span style="font-size: 0.85rem; font-weight: 600; color: #64748b; text-transform: uppercase;">Người dùng</span>
                  <div style="padding: 0.5rem; background: #eff6ff; color: #3b82f6; border-radius: 8px;"><Users :size="18" /></div>
                </div>
                <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">{{ data.business.totalUsers.toLocaleString() }}</div>
                <div style="margin-top: 0.75rem; font-size: 0.8rem; color: #64748b; font-weight: 500;">
                  <span style="color: #10b981; font-weight: 700;">{{ data.business.paidUsers }}</span> trả phí
                </div>
              </div>
            </div>

            <div class="ui-panel" style="border-top: 4px solid #f59e0b;">
              <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
                <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                  <span style="font-size: 0.85rem; font-weight: 600; color: #64748b; text-transform: uppercase;">Doanh thu Hàng tháng</span>
                  <div style="padding: 0.5rem; background: #fffbeb; color: #f59e0b; border-radius: 8px;"><TrendingUp :size="18" /></div>
                </div>
                <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">${{ (data.business.monthlyRevenue ?? 0).toLocaleString() }}</div>
                <div style="margin-top: 0.75rem; font-size: 0.8rem; color: #64748b; font-weight: 500;">
                  Tỷ lệ chuyển đổi: <strong style="color: #0f172a;">{{ data.business.conversionRate }}%</strong>
                </div>
              </div>
            </div>

            <div class="ui-panel" style="border-top: 4px solid #10b981;">
              <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
                <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                  <span style="font-size: 0.85rem; font-weight: 600; color: #64748b; text-transform: uppercase;">Tổng Links</span>
                  <div style="padding: 0.5rem; background: #ecfdf5; color: #10b981; border-radius: 8px;"><Link2 :size="18" /></div>
                </div>
                <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">{{ data.product.totalLinks.toLocaleString() }}</div>
                <div style="margin-top: 0.75rem; font-size: 0.8rem; color: #64748b; font-weight: 500;">
                  Đang hoạt động: <strong style="color: #0f172a;">{{ data.product.activeLinks.toLocaleString() }}</strong>
                </div>
              </div>
            </div>

            <div class="ui-panel" style="border-top: 4px solid #8b5cf6;">
              <div class="ui-panel-body" style="display: flex; flex-direction: column; justify-content: center; padding: 1.5rem;">
                <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem;">
                  <span style="font-size: 0.85rem; font-weight: 600; color: #64748b; text-transform: uppercase;">Lượt Clicks</span>
                  <div style="padding: 0.5rem; background: #f5f3ff; color: #8b5cf6; border-radius: 8px;"><MousePointerClick :size="18" /></div>
                </div>
                <div style="font-size: 2.25rem; font-weight: 800; color: #0f172a; line-height: 1;">{{ data.product.totalClicks.toLocaleString() }}</div>
                <div style="margin-top: 0.75rem; font-size: 0.8rem; color: #64748b; font-weight: 500;">
                  Unique: <strong style="color: #0f172a;">{{ data.product.uniqueClicks.toLocaleString() }}</strong>
                </div>
              </div>
            </div>

          </div>
        </section>

        <!-- Secondary Diagnostics -->
        <section>
          <div style="margin-bottom: 0.75rem; font-size: 0.85rem; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: 0.5px;">Phân hệ Chi tiết</div>
          
          <div class="ui-card-grid ui-card-grid-3">
            
            <div class="ui-panel" style="background: white;">
              <div class="ui-panel-header" style="border-bottom: 1px solid #f1f5f9; padding: 1rem 1.5rem;">
                <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;"><Zap :size="16" style="color: #f59e0b;" /> Kinh doanh (Business)</h3>
              </div>
              <div class="ui-panel-body" style="padding: 0;">
                <div style="display: flex; justify-content: space-between; padding: 1rem 1.5rem; border-bottom: 1px solid #f8fafc;">
                  <span style="font-size: 0.85rem; color: #64748b; font-weight: 500;">Doanh Thu Định Kỳ (MRR)</span>
                  <span style="font-size: 0.95rem; font-weight: 700; color: #059669;">${{ data.business.monthlyRevenue ?? 0 }}</span>
                </div>
                <div style="display: flex; justify-content: space-between; padding: 1rem 1.5rem; border-bottom: 1px solid #f8fafc;">
                  <span style="font-size: 0.85rem; color: #64748b; font-weight: 500;">Tỉ lệ Chuyển đổi (CR)</span>
                  <span style="font-size: 0.95rem; font-weight: 700; color: #0f172a;">{{ data.business.conversionRate }}%</span>
                </div>
                <div style="display: flex; justify-content: space-between; padding: 1rem 1.5rem;">
                  <span style="font-size: 0.85rem; color: #64748b; font-weight: 500;">Users Trả phí</span>
                  <span style="font-size: 0.95rem; font-weight: 700; color: #0f172a;">{{ data.business.paidUsers }}</span>
                </div>
              </div>
            </div>

            <div class="ui-panel" style="background: white;">
              <div class="ui-panel-header" style="border-bottom: 1px solid #f1f5f9; padding: 1rem 1.5rem;">
                <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem;"><ServerCog :size="16" style="color: #3b82f6;" /> Sản phẩm (Product)</h3>
              </div>
              <div class="ui-panel-body" style="padding: 0;">
                <div style="display: flex; justify-content: space-between; padding: 1rem 1.5rem; border-bottom: 1px solid #f8fafc;">
                  <span style="font-size: 0.85rem; color: #64748b; font-weight: 500;">Liên kết Đang hoạt động</span>
                  <span style="font-size: 0.95rem; font-weight: 700; color: #0f172a;">{{ data.product.activeLinks.toLocaleString() }}</span>
                </div>
                <div style="display: flex; justify-content: space-between; padding: 1rem 1.5rem; border-bottom: 1px solid #f8fafc;">
                  <span style="font-size: 0.85rem; color: #64748b; font-weight: 500;">Unique Clicks</span>
                  <span style="font-size: 0.95rem; font-weight: 700; color: #0f172a;">{{ data.product.uniqueClicks.toLocaleString() }}</span>
                </div>
                <div style="display: flex; justify-content: space-between; padding: 1rem 1.5rem;">
                  <span style="font-size: 0.85rem; color: #64748b; font-weight: 500;">Tỉ lệ Active Link</span>
                  <span style="font-size: 0.95rem; font-weight: 700; color: #0f172a;">
                    {{ data.product.totalLinks > 0 ? ((data.product.activeLinks / data.product.totalLinks) * 100).toFixed(1) : 0 }}%
                  </span>
                </div>
              </div>
            </div>

            <div class="ui-panel" style="background: white; border-color: #fecdd3;">
              <div class="ui-panel-header" style="background: #fff1f2; border-bottom: 1px solid #ffe4e6; padding: 1rem 1.5rem;">
                <h3 class="ui-panel-title" style="display: flex; align-items: center; gap: 0.5rem; color: #e11d48;"><ShieldAlert :size="16" /> Vận hành & Cảnh báo</h3>
              </div>
              <div class="ui-panel-body" style="padding: 0;">
                <div style="display: flex; justify-content: space-between; padding: 1rem 1.5rem; border-bottom: 1px solid #f8fafc;">
                  <span style="font-size: 0.85rem; color: #64748b; font-weight: 500;">Bot Clicks</span>
                  <span style="font-size: 0.95rem; font-weight: 700;" :style="data.operations.botClicks > 0 ? 'color: #e11d48;' : 'color: #0f172a;'">{{ data.operations.botClicks.toLocaleString() }}</span>
                </div>
                <div style="display: flex; justify-content: space-between; padding: 1rem 1.5rem; border-bottom: 1px solid #f8fafc;">
                  <span style="font-size: 0.85rem; color: #64748b; font-weight: 500;">Clicks đáng ngờ</span>
                  <span style="font-size: 0.95rem; font-weight: 700;" :style="data.operations.suspiciousClicks > 0 ? 'color: #f59e0b;' : 'color: #0f172a;'">{{ data.operations.suspiciousClicks.toLocaleString() }}</span>
                </div>
                <div style="display: flex; justify-content: space-between; padding: 1rem 1.5rem;">
                  <span style="font-size: 0.85rem; color: #64748b; font-weight: 500;">Tỉ lệ Lỗi / Độ trễ</span>
                  <span style="font-size: 0.95rem; font-weight: 700;" :class="data.operations.errorRate > 5 ? 'text-danger' : 'text-on-surface'">
                    {{ data.operations.errorRate }}% <span style="color: #cbd5e1; font-weight: 400;">|</span> {{ data.operations.queueLagSeconds }}s
                  </span>
                </div>
              </div>
            </div>

          </div>
        </section>

        <!-- Quick Access Box -->
        <section>
           <div class="ui-panel" style="background: transparent; border: 1px dashed #cbd5e1; padding: 1.5rem;">
              <div style="display: flex; gap: 1rem; align-items: center; justify-content: center; flex-wrap: wrap;">
                  <span style="font-size: 0.85rem; font-weight: 600; color: #64748b;">Truy cập nhanh:</span>
                  <RouterLink to="/admin/users" class="ui-btn ui-btn-outline" style="background: white;">Quản lý Cấp phép</RouterLink>
                  <RouterLink to="/admin/links" class="ui-btn ui-btn-outline" style="background: white;">Giám sát Link</RouterLink>
                  <RouterLink to="/admin/reports" class="ui-btn ui-btn-outline" style="background: white;">Báo cáo Doanh thu</RouterLink>
                  <RouterLink to="/admin/security" class="ui-btn ui-btn-outline" style="background: white;">Trạng thái Bảo mật</RouterLink>
              </div>
           </div>
        </section>

      </div>
    </template>
  </div>
</template>
