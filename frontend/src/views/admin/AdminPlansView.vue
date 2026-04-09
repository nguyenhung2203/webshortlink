<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { AdminService } from '@/api/services'
import {
  Package, CheckCircle, XCircle, RefreshCw, AlertCircle,
  Edit2, ToggleLeft, ToggleRight, Plus, Trash2, X, Info, Hash,
  Zap, Crown, Check
} from 'lucide-vue-next'

const authStore = useAuthStore()

interface PlanFeature { id: number; featureKey: string; isEnabled: boolean; limitValue: number | null; featureValue: string | null }
interface Plan { id: number; code: string; name: string; monthlyPrice: number; isActive: boolean; features: PlanFeature[] }
interface FeatureLabel { featureKey: string; label: string; description: string; featureType: string }

const plans = ref<Plan[]>([])
const customLabels = ref<FeatureLabel[]>([])
const loading = ref(true)
const error = ref('')

const builtInFeatures: Record<string, FeatureLabel> = {
  'links.max_count':           { featureKey: 'links.max_count',           label: 'Số link tối đa',        description: 'Giới hạn tổng số shortlink được tạo',       featureType: 'number' },
  'links.custom_slug':         { featureKey: 'links.custom_slug',         label: 'Slug tùy chỉnh',        description: 'Cho phép đặt slug thân thiện',              featureType: 'toggle' },
  'links.expiration':          { featureKey: 'links.expiration',          label: 'Ngày hết hạn link',     description: 'Link tự động hết hạn theo ngày',            featureType: 'toggle' },
  'links.click_limit':         { featureKey: 'links.click_limit',         label: 'Giới hạn click',        description: 'Link tự tắt sau N lần click',               featureType: 'toggle' },
  'links.password_protection': { featureKey: 'links.password_protection', label: 'Bảo vệ mật khẩu',      description: 'Yêu cầu mật khẩu để mở link',              featureType: 'toggle' },
  'domains.custom':            { featureKey: 'domains.custom',            label: 'Domain tùy chỉnh',      description: 'Dùng tên miền riêng cho link',              featureType: 'toggle' },
  'domains.max_count':         { featureKey: 'domains.max_count',         label: 'Số domain tối đa',      description: 'Số lượng custom domain được thêm',          featureType: 'number' },
  'analytics.retention_days':  { featureKey: 'analytics.retention_days',  label: 'Lưu analytics (ngày)', description: 'Thời gian lưu dữ liệu click',               featureType: 'number' },
  'analytics.export_csv':      { featureKey: 'analytics.export_csv',      label: 'Xuất CSV',             description: 'Xuất dữ liệu phân tích sang CSV',           featureType: 'toggle' },
  'api.access':                { featureKey: 'api.access',                label: 'Truy cập API',          description: 'Tích hợp REST API trực tiếp',               featureType: 'toggle' },
  'links.social_preview':      { featureKey: 'links.social_preview',      label: 'Xem trước MXH',         description: 'Cho phép tùy chỉnh thẻ xem trước OpenGraph', featureType: 'toggle' },
}

const allFeatures = computed<FeatureLabel[]>(() => {
  const custom = customLabels.value.filter(c => !builtInFeatures[c.featureKey])
  return [...Object.values(builtInFeatures), ...custom]
})

const getFeature = (plan: Plan, key: string): PlanFeature =>
  plan.features.find(f => f.featureKey === key) ?? { id: 0, featureKey: key, isEnabled: false, limitValue: null, featureValue: null }

// ─── Edit modal ────────────────────────────────────────────────────────────────
const editModal = ref<{ planId: number; planName: string; fl: FeatureLabel } | null>(null)
const editForm = ref({ isEnabled: false, limitValue: null as number | null })
const saving = ref(false)
const saveError = ref('')

const openEdit = (plan: Plan, fl: FeatureLabel) => {
  const f = getFeature(plan, fl.featureKey)
  editModal.value = { planId: plan.id, planName: plan.name, fl }
  editForm.value = { isEnabled: f.isEnabled, limitValue: f.limitValue }
  saveError.value = ''
}

const saveFeature = async () => {
  if (!editModal.value || !authStore.accessToken) return
  saving.value = true; saveError.value = ''
  try {
    await AdminService.updatePlanFeature(authStore.accessToken, editModal.value.planId, editModal.value.fl.featureKey,
      { isEnabled: editForm.value.isEnabled, limitValue: editForm.value.limitValue, featureValue: null })
    editModal.value = null
    await loadData()
  } catch (err) { saveError.value = err instanceof Error ? err.message : 'Lỗi lưu'
  } finally { saving.value = false }
}

// ─── Add feature modal ─────────────────────────────────────────────────────────
const addModal = ref(false)
const addForm = ref({ featureKey: '', label: '', description: '', featureType: 'toggle' })
const addError = ref('')
const addSaving = ref(false)

const openAddModal = () => {
  addForm.value = { featureKey: '', label: '', description: '', featureType: 'toggle' }
  addError.value = ''; addModal.value = true
}

const saveNewFeature = async () => {
  addError.value = ''
  if (!addForm.value.featureKey.trim() || !addForm.value.label.trim()) {
    addError.value = 'Vui lòng nhập Feature Key và Tên hiển thị.'; return
  }
  if (!/^[a-z0-9_.]+$/.test(addForm.value.featureKey)) {
    addError.value = 'Feature Key chỉ chứa chữ thường, số, dấu . và _'; return
  }
  if (!authStore.accessToken) return
  addSaving.value = true
  try {
    await AdminService.saveFeatureLabel(authStore.accessToken, { ...addForm.value })
    addModal.value = false; await loadData()
  } catch (err) { addError.value = err instanceof Error ? err.message : 'Lỗi tạo tính năng'
  } finally { addSaving.value = false }
}

// ─── Delete feature ────────────────────────────────────────────────────────────
const deleteFeature = async (fl: FeatureLabel) => {
  if (!authStore.accessToken) return
  if (!confirm(`Xóa tính năng "${fl.label}" (${fl.featureKey}) khỏi toàn bộ hệ thống?`)) return
  try { await AdminService.deleteFeatureLabel(authStore.accessToken, fl.featureKey); await loadData()
  } catch (err) { alert(err instanceof Error ? err.message : 'Lỗi xóa') }
}

// ─── Load ──────────────────────────────────────────────────────────────────────
const loadData = async () => {
  loading.value = true; error.value = ''
  try {
    if (!authStore.accessToken) return
    const [p, l] = await Promise.all([
      AdminService.getPlans(authStore.accessToken),
      AdminService.getFeatureLabels(authStore.accessToken),
    ])
    plans.value = p; customLabels.value = l
  } catch (err) { error.value = err instanceof Error ? err.message : 'Không thể tải'
  } finally { loading.value = false }
}

onMounted(loadData)

// Plan gradient configs
const planStyle = [
  { gradient: 'linear-gradient(135deg, #1e293b 0%, #334155 100%)', badge: '#94a3b8', badgeBg: '#1e293b', accent: '#94a3b8', icon: '🆓' },
  { gradient: 'linear-gradient(135deg, #1d4ed8 0%, #3b82f6 100%)', badge: '#bfdbfe', badgeBg: '#1e40af', accent: '#60a5fa', icon: '⚡' },
  { gradient: 'linear-gradient(135deg, #7c3aed 0%, #a855f7 100%)', badge: '#e9d5ff', badgeBg: '#6d28d9', accent: '#c084fc', icon: '👑' },
]
const ps = (idx: number) => planStyle[idx] ?? planStyle[2]
</script>

<template>
  <div class="ui-root">
    <!-- Header -->
    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><Package :size="13" /> Cài đặt hệ thống</div>
        <h1 class="ui-title">Quản lý Gói & Tính năng</h1>
        <p class="ui-subtitle">Cấu hình từng tính năng cho từng gói dịch vụ. Thêm tính năng mới bất cứ lúc nào.</p>
      </div>
      <div style="display:flex;gap:0.5rem;">
        <button class="ui-btn ui-btn-outline" @click="loadData" :disabled="loading">
          <RefreshCw :size="14" :class="{'animate-spin':loading}" /> Làm mới
        </button>
        <button class="ui-btn ui-btn-primary" @click="openAddModal">
          <Plus :size="14" /> Thêm tính năng
        </button>
      </div>
    </div>

    <div v-if="error" class="ui-alert ui-alert-error" style="margin-bottom:1.5rem;">
      <AlertCircle :size="16" /> {{ error }}
    </div>

    <div v-if="loading" style="display:flex;gap:1.25rem;">
      <div v-for="i in 3" :key="i" class="ui-skeleton" style="flex:1;height:600px;border-radius:16px;" />
    </div>

    <!-- 3-column plan cards -->
    <div v-else class="pricing-grid">
      <div
        v-for="(plan, idx) in plans" :key="plan.id"
        :class="['pricing-card', `plan-${plan.code.toLowerCase()}`]"
      >
        <!-- Highlight labels -->
        <div v-if="plan.code === 'pro'" class="popular-label">
          <Zap :size="14" fill="currentColor" /> ĐƯỢC ƯA CHUỘNG NHẤT
        </div>
        <div v-if="plan.code === 'plus'" class="enterprise-label">
          <Crown :size="14" /> DÀNH CHO DOANH NGHIỆP
        </div>

        <!-- Header -->
        <div class="card-header">
          <h3 class="plan-name">{{ plan.name }}</h3>
          <p class="plan-desc">Cấu hình tính năng gói {{ plan.name }}</p>
          
          <div class="plan-price-box">
            <span class="price-val">{{ plan.monthlyPrice === 0 ? 'Miễn phí' : (plan.monthlyPrice / 1000).toLocaleString() + 'k' }}</span>
            <span v-if="plan.monthlyPrice > 0" class="price-cycle">/tháng</span>
            <span v-if="plan.monthlyPrice > 0" class="price-currency">VND</span>
          </div>
        </div>

        <!-- Feature list -->
        <div class="card-features">
          <div class="feature-title">Quyền lợi thiết lập:</div>
          <div style="display:flex; flex-direction:column; gap: 0;">
            <div
              v-for="fl in allFeatures"
              :key="fl.featureKey"
              class="admin-feature-row"
            >
              <!-- Feature info -->
              <div style="flex:1;min-width:0;">
                <div style="display:flex;align-items:center;gap:0.35rem;flex-wrap:wrap;">
                  <span class="feat-main">{{ fl.label }}</span>
                  <span v-if="!builtInFeatures[fl.featureKey]" class="ui-badge custom-badge">CUSTOM</span>
                </div>
                <!-- Value display -->
                <div style="margin-top:0.25rem;">
                  <template v-if="fl.featureType === 'toggle'">
                    <span v-if="getFeature(plan, fl.featureKey).isEnabled" class="feat-status feat-enabled">
                      <Check :size="12" stroke-width="3" /> Bật
                    </span>
                    <span v-else class="feat-status feat-disabled">
                      <XCircle :size="12" /> Tắt
                    </span>
                  </template>
                  <template v-else>
                    <span v-if="getFeature(plan, fl.featureKey).limitValue != null" class="feat-status feat-limit">
                      {{ getFeature(plan, fl.featureKey).limitValue }}
                    </span>
                    <span v-else class="feat-status feat-disabled">Không giới hạn</span>
                  </template>
                </div>
              </div>

              <!-- Actions -->
              <div class="admin-feat-actions">
                <button @click="openEdit(plan, fl)" class="admin-edit-btn">
                  <Edit2 :size="11" /> Sửa
                </button>
                <!-- Delete only on first plan card to avoid 3x delete buttons -->
                <button
                  v-if="idx === 0 && !builtInFeatures[fl.featureKey]"
                  @click="deleteFeature(fl)"
                  class="admin-delete-btn"
                  title="Xóa tính năng khỏi hệ thống"
                >
                  <Trash2 :size="11" />
                </button>
              </div>
            </div>

            <!-- Empty state -->
            <div v-if="allFeatures.length === 0" style="padding:2rem;text-align:center;color:#94a3b8;font-size:0.85rem;">
              Chưa có tính năng nào
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ─── Modal: Edit Feature ────────────────────────────────────────────────── -->
    <div
      v-if="editModal"
      style="position:fixed;inset:0;background:rgba(0,0,0,0.45);display:flex;align-items:center;justify-content:center;z-index:100;"
      @click.self="editModal=null"
    >
      <div class="ui-panel" style="width:100%;max-width:440px;padding:2rem;">
        <div style="display:flex;align-items:flex-start;justify-content:space-between;margin-bottom:1.5rem;">
          <div>
            <h2 style="font-size:1.05rem;font-weight:800;color:#0f172a;margin:0 0 0.2rem;">Sửa tính năng</h2>
            <p style="color:#64748b;font-size:0.82rem;margin:0;">
              Gói <strong>{{ editModal.planName }}</strong> · <code style="color:#a78bfa;">{{ editModal.fl.featureKey }}</code>
            </p>
            <p style="color:#94a3b8;font-size:0.75rem;margin:0.25rem 0 0;">{{ editModal.fl.description }}</p>
          </div>
          <button @click="editModal=null" style="background:transparent;border:none;cursor:pointer;color:#94a3b8;"><X :size="18"/></button>
        </div>

        <div v-if="saveError" class="ui-alert ui-alert-error" style="margin-bottom:1rem;padding:0.65rem;font-size:0.83rem;">
          <AlertCircle :size="13"/> {{ saveError }}
        </div>

        <div style="display:flex;flex-direction:column;gap:1rem;">
          <!-- Toggle -->
          <div style="display:flex;align-items:center;justify-content:space-between;background:#f8fafc;padding:1rem;border-radius:10px;border:1px solid #e2e8f0;">
            <div>
              <p style="font-weight:700;margin:0;font-size:0.9rem;color:#0f172a;">
                {{ editModal.fl.featureType === 'toggle' ? 'Bật / Tắt tính năng' : 'Cho phép dùng tính năng' }}
              </p>
              <p style="color:#64748b;font-size:0.75rem;margin:0.1rem 0 0;">
                {{ editForm.isEnabled ? 'Đang bật — user gói này có quyền' : 'Đang tắt — user gói này không có quyền' }}
              </p>
            </div>
            <button @click="editForm.isEnabled=!editForm.isEnabled" :style="{color:editForm.isEnabled?'#10b981':'#94a3b8'}" style="background:transparent;border:none;cursor:pointer;">
              <ToggleRight v-if="editForm.isEnabled" :size="44"/>
              <ToggleLeft v-else :size="44"/>
            </button>
          </div>

          <!-- Limit value -->
          <div v-if="editModal.fl.featureType === 'number'">
            <label class="ui-label" style="display:flex;align-items:center;gap:0.35rem;"><Hash :size="12"/> Giá trị giới hạn</label>
            <input
              v-model.number="editForm.limitValue"
              type="number" min="0"
              class="ui-input"
              placeholder="Để trống = không giới hạn"
              style="width:100%;"
            />
            <p style="font-size:0.73rem;color:#94a3b8;margin:0.3rem 0 0;">Để trống = không giới hạn</p>
          </div>

          <div style="display:flex;gap:0.75rem;justify-content:flex-end;margin-top:0.5rem;">
            <button class="ui-btn ui-btn-outline" @click="editModal=null">Hủy</button>
            <button class="ui-btn ui-btn-primary" @click="saveFeature" :disabled="saving" style="min-width:110px;justify-content:center;">
              <RefreshCw v-if="saving" :size="13" class="animate-spin"/>
              <span v-else>Lưu thay đổi</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- ─── Modal: Add Feature ─────────────────────────────────────────────────── -->
    <div
      v-if="addModal"
      style="position:fixed;inset:0;background:rgba(0,0,0,0.45);display:flex;align-items:center;justify-content:center;z-index:100;"
      @click.self="addModal=false"
    >
      <div class="ui-panel" style="width:100%;max-width:500px;padding:2rem;">
        <div style="display:flex;align-items:center;justify-content:space-between;margin-bottom:1.5rem;">
          <div>
            <h2 style="font-size:1.05rem;font-weight:800;color:#0f172a;margin:0 0 0.2rem;">Thêm tính năng mới</h2>
            <p style="color:#64748b;font-size:0.82rem;margin:0;">Tính năng sẽ hiển thị trong tất cả gói. Admin cấu hình riêng cho từng gói sau.</p>
          </div>
          <button @click="addModal=false" style="background:transparent;border:none;cursor:pointer;color:#94a3b8;"><X :size="18"/></button>
        </div>

        <div v-if="addError" class="ui-alert ui-alert-error" style="margin-bottom:1rem;padding:0.65rem;font-size:0.83rem;">
          <AlertCircle :size="13"/> {{ addError }}
        </div>

        <div style="display:flex;flex-direction:column;gap:1rem;">
          <div>
            <label class="ui-label">Feature Key <span style="color:#ef4444;">*</span></label>
            <input v-model="addForm.featureKey" type="text" class="ui-input" placeholder="vd: links.qr_code" style="width:100%;font-family:monospace;"/>
            <p style="font-size:0.72rem;color:#94a3b8;margin:0.25rem 0 0;display:flex;align-items:center;gap:0.25rem;">
              <Info :size="11"/> Chỉ chữ thường, số, dấu <code>.</code> và <code>_</code>. Không thể đổi sau khi tạo.
            </p>
          </div>
          <div>
            <label class="ui-label">Tên hiển thị <span style="color:#ef4444;">*</span></label>
            <input v-model="addForm.label" type="text" class="ui-input" placeholder="vd: Tạo mã QR Code" style="width:100%;"/>
          </div>
          <div>
            <label class="ui-label">Mô tả ngắn</label>
            <input v-model="addForm.description" type="text" class="ui-input" placeholder="vd: Tự động tạo QR Code cho mỗi shortlink" style="width:100%;"/>
          </div>
          <div>
            <label class="ui-label">Loại tính năng</label>
            <select v-model="addForm.featureType" class="ui-input" style="width:100%;">
              <option value="toggle">Toggle — Bật / Tắt quyền truy cập</option>
              <option value="number">Số — Giới hạn số lượng (VD: 50 link, 3 domain)</option>
            </select>
          </div>

          <div style="display:flex;gap:0.75rem;justify-content:flex-end;margin-top:0.5rem;">
            <button class="ui-btn ui-btn-outline" @click="addModal=false">Hủy</button>
            <button class="ui-btn ui-btn-primary" @click="saveNewFeature" :disabled="addSaving" style="min-width:140px;justify-content:center;">
              <RefreshCw v-if="addSaving" :size="13" class="animate-spin"/>
              <span v-else><Plus :size="13" style="display:inline;"/> Tạo tính năng</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
  /* Base Grid */
  .pricing-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1.5rem;
    max-width: 1100px;
    margin: 0 auto;
    align-items: stretch;
  }

  /* Card Base */
  .pricing-card {
    position: relative;
    background: white;
    border-radius: 16px;
    border: 1px solid #e2e8f0;
    display: flex;
    flex-direction: column;
    padding: 1.5rem;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  }
  
  /* Cards Styles */
  .pricing-card.plan-pro {
    background: #0f172a;
    border: none;
    color: white;
    transform: scale(1.03);
    box-shadow: 0 15px 30px -10px rgba(15,23,42,0.5);
    z-index: 10;
  }
  .pricing-card.plan-plus {
    border: 2px solid #10b981;
    box-shadow: 0 10px 25px -5px rgba(16,185,129,0.15);
  }

  /* Pro specific overrides */
  .plan-pro .plan-name { color: white; }
  .plan-pro .plan-desc { color: #94a3b8; border-color: #1e293b; }
  .plan-pro .price-val { color: white; }
  .plan-pro .price-cycle, .plan-pro .price-currency { color: #94a3b8; }
  .plan-pro .feature-title { color: #f8fafc; }
  .plan-pro .feat-main { color: #f1f5f9; }
  .plan-pro .admin-feature-row { border-bottom-color: #1e293b; }
  .plan-pro .feat-disabled { color: #475569; }
  .plan-pro .feat-enabled { color: #38bdf8; }
  .plan-pro .admin-edit-btn { background: #1e293b; color: #94a3b8; }
  .plan-pro .admin-edit-btn:hover { background: #334155; color: #f8fafc; }

  /* Badges */
  .popular-label {
    position: absolute;
    top: -12px;
    left: 50%;
    transform: translateX(-50%);
    background: linear-gradient(135deg, #3b82f6, #2563eb);
    color: white;
    padding: 0.25rem 0.85rem;
    border-radius: 999px;
    font-size: 0.65rem;
    font-weight: 800;
    letter-spacing: 0.5px;
    box-shadow: 0 2px 4px rgba(37,99,235,0.4);
    display: flex;
    align-items: center;
    gap: 0.3rem;
    white-space: nowrap;
  }
  .enterprise-label {
    position: absolute;
    top: 1rem;
    right: 1rem;
    background: #fffbeb;
    color: #b45309;
    padding: 0.2rem 0.5rem;
    border-radius: 4px;
    font-size: 0.65rem;
    font-weight: 800;
    border: 1px solid #fde68a;
    display: flex;
    align-items: center;
    gap: 0.25rem;
  }
  .custom-badge {
    background: #fce7f3;
    color: #be185d;
    font-size: 0.6rem;
    padding: 0.05rem 0.3rem;
  }
  .plan-pro .custom-badge { background: #4c1d95; color: #ddd6fe; }

  /* Card Header */
  .card-header {
    margin-bottom: 1.25rem;
  }
  .plan-name {
    font-size: 1.15rem;
    font-weight: 900;
    color: #0f172a;
    margin: 0 0 0.35rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
  }
  .plan-desc {
    font-size: 0.8rem;
    color: #64748b;
    margin: 0;
    line-height: 1.4;
    padding-bottom: 1rem;
    border-bottom: 1px solid #f1f5f9;
  }
  .plan-price-box {
    margin-top: 1rem;
    display: flex;
    align-items: flex-end;
    gap: 0.2rem;
  }
  .price-val {
    font-size: 2.25rem;
    font-weight: 900;
    color: #0f172a;
    line-height: 1;
    letter-spacing: -0.03em;
  }
  .price-cycle {
    font-size: 0.85rem;
    color: #64748b;
    margin-bottom: 0.4rem;
    font-weight: 600;
  }
  .price-currency {
    font-size: 0.75rem;
    color: #94a3b8;
    margin-bottom: 0.45rem;
    font-weight: 700;
    margin-left: 0.2rem;
  }

  /* Admin Features */
  .card-features {
    display: flex;
    flex-direction: column;
    flex: 1;
    background: transparent;
  }
  .feature-title {
    font-size: 0.75rem;
    font-weight: 800;
    color: #0f172a;
    text-transform: uppercase;
    letter-spacing: 1px;
    margin-bottom: 1rem;
  }
  .admin-feature-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.8rem 0;
    border-bottom: 1px solid #f1f5f9;
    gap: 0.5rem;
  }
  .admin-feature-row:last-child {
    border-bottom: none;
  }
  .feat-main {
    font-size: 0.87rem;
    font-weight: 600;
    color: #1e293b;
  }
  .feat-status {
    font-size: 0.75rem;
    display: flex;
    align-items: center;
    gap: 0.25rem;
  }
  .feat-enabled { color: #10b981; font-weight: 600; }
  .feat-disabled { color: #cbd5e1; }
  .feat-limit { font-size: 0.8rem; color: #3b82f6; font-weight: 700; }
  .plan-pro .feat-limit { color: #60a5fa; }

  /* Admin Actions */
  .admin-feat-actions {
    display: flex;
    align-items: center;
    gap: 0.3rem;
    flex-shrink: 0;
  }
  .admin-edit-btn {
    background: #f1f5f9;
    border: none;
    border-radius: 6px;
    padding: 0.3rem 0.5rem;
    cursor: pointer;
    color: #64748b;
    font-size: 0.72rem;
    display: flex;
    align-items: center;
    gap: 0.2rem;
    transition: background 0.15s;
  }
  .admin-edit-btn:hover { background: #e2e8f0; color: #0f172a; }
  
  .admin-delete-btn {
    background: #fef2f2;
    border: none;
    border-radius: 6px;
    padding: 0.3rem 0.4rem;
    cursor: pointer;
    color: #ef4444;
  }
  .admin-delete-btn:hover { background: #fecaca; }
  .plan-pro .admin-delete-btn { background: #450a0a; color: #f87171; }
</style>
