<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { DomainService, UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { Globe, Plus, CheckCircle, Clock, Shield, Trash2, RefreshCw, Lock, AlertCircle } from 'lucide-vue-next'
import { useRouter } from 'vue-router'

const router = useRouter()
const authStore = useAuthStore()

const domains = ref<any[]>([])
const loading = ref(true)
const error = ref('')
const showAdd = ref(false)
const newHost = ref('')
const addLoading = ref(false)
const verifyLoadingId = ref<string | null>(null)

const currentPlanId = ref(1)
const isPro = computed(() => currentPlanId.value >= 2)

async function load() {
  loading.value = true
  try {
    if (!authStore.accessToken) return
    const [list, subscription] = await Promise.all([
      DomainService.list(authStore.accessToken),
      UserService.getSubscription(authStore.accessToken),
    ])
    domains.value = list
    currentPlanId.value = (subscription as any)?.planId ?? 1
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải danh sách domain.'
  } finally {
    loading.value = false
  }
}

async function addDomain() {
  if (!authStore.accessToken || !newHost.value.trim()) return
  addLoading.value = true
  error.value = ''
  try {
    await DomainService.create(authStore.accessToken, newHost.value.trim())
    newHost.value = ''
    showAdd.value = false
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể thêm domain.'
  } finally {
    addLoading.value = false
  }
}

async function verifyDomain(domainId: string) {
  if (!authStore.accessToken) return
  verifyLoadingId.value = domainId
  try {
    const domain = domains.value.find((item) => item.id === domainId)
    if (!domain?.verificationToken) {
      throw new Error('Thiếu verification token để xác minh domain.')
    }

    const result = await DomainService.verify(authStore.accessToken, domainId, domain.verificationToken)
    if (result?.verified) {
      alert('✅ Domain đã được xác minh thành công!')
    } else {
      alert('⚠️ Xác minh DNS chưa thành công. Hãy đảm bảo bạn đã cấu hình DNS record đúng cấu trúc.')
    }
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi xác minh domain.')
  } finally {
    verifyLoadingId.value = null
  }
}

async function deleteDomain(domainId: string, host: string) {
  if (!authStore.accessToken || !confirm(`Bạn có chắc chắn muốn xóa domain ${host}?`)) return
  try {
    await DomainService.delete(authStore.accessToken, domainId)
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Không thể xóa domain.')
  }
}

onMounted(load)
</script>

<template>
  <div class="ui-root">
    <!-- Header -->
    <div class="ui-header">
      <div class="ui-header-left">
        <div class="ui-eyebrow"><Globe :size="13" /> Branded Links</div>
        <h1 class="ui-title">Tên miền tùy chỉnh</h1>
        <p class="ui-subtitle">Sử dụng tên miền riêng để xây dựng thương hiệu khi chia sẻ link.</p>
      </div>
      <div>
        <button v-if="isPro" class="ui-btn ui-btn-primary" @click="showAdd = !showAdd">
          <Plus :size="16" /> Thêm Tên Miền
        </button>
      </div>
    </div>

    <div v-if="error && !loading" class="ui-alert ui-alert-error">
      <AlertCircle :size="16"/> {{ error }}
    </div>

    <!-- Loading -->
    <div v-if="loading" class="ui-skeleton" style="height: 200px;" />

    <template v-else>
      <!-- Paywall for Free Users -->
      <div v-if="!isPro" class="ui-empty">
        <div class="ui-empty-icon" style="color: #64748b; opacity: 0.5;"><Lock :size="50" /></div>
        <h3 class="ui-empty-title">Tính năng dành cho gói Pro / Plus</h3>
        <p class="ui-empty-desc">Nâng cấp tài khoản để sử dụng tên miền riêng trên các link rút gọn của bạn.</p>
        <button class="ui-btn ui-btn-primary" style="margin-top: 1.5rem;" @click="router.push('/app/billing')">
          Xem bảng giá nâng cấp
        </button>
      </div>

      <!-- Feature Content -->
      <div v-else style="display: flex; flex-direction: column; gap: 1.5rem;">
        
        <!-- Instruction Card -->
        <div class="ui-panel" style="border-left: 4px solid #3b82f6;">
          <div class="ui-panel-body" style="display: flex; gap: 1rem;">
            <Shield :size="24" style="color: #3b82f6; flex-shrink: 0;" />
            <div>
              <h3 style="margin: 0 0 0.5rem; font-size: 1rem; color: #0f172a; font-weight: 700;">Hướng dẫn cấu hình DNS</h3>
              <p style="margin: 0; font-size: 0.85rem; color: #475569; line-height: 1.5;">
                Để kết nối tên miền, hãy truy cập trình quản lý DNS của bạn (như Cloudflare, GoDaddy, v.v.). Thêm một bản ghi <strong style="font-family: monospace; background: #f1f5f9; padding: 0.15rem 0.3rem; border-radius: 4px; color: #0f172a;">CNAME</strong> trỏ tới <strong style="font-family: monospace; background: #f1f5f9; padding: 0.15rem 0.3rem; border-radius: 4px; color: #0f172a;">sho.rt</strong>. <br>
                Sau đó copy mã TXT xác minh bên dưới và thêm bản ghi <strong style="font-family: monospace; background: #f1f5f9; padding: 0.15rem 0.3rem; border-radius: 4px; color: #0f172a;">TXT</strong> tương ứng với mã đó để chứng minh bạn là chủ sở hữu.
              </p>
            </div>
          </div>
        </div>

        <!-- Add Domain Form Panel -->
        <div v-if="showAdd" class="ui-panel" style="background: #f8fafc; border: 1px dashed #cbd5e1;">
          <div class="ui-panel-body">
            <h3 style="margin: 0 0 1rem; font-size: 1rem; color: #0f172a; font-weight: 700;">Kết nối tên miền mới</h3>
            <div style="display: flex; gap: 0.5rem; flex-wrap: wrap;">
              <input
                v-model="newHost"
                type="text"
                class="ui-form-input"
                placeholder="Ví dụ: go.yourbrand.com"
                style="flex: 1; min-width: 200px; margin: 0;"
                @keydown.enter="addDomain"
              />
              <button class="ui-btn ui-btn-primary" :disabled="addLoading" @click="addDomain">
                Khai báo
              </button>
              <button class="ui-btn ui-btn-ghost" @click="showAdd = false">Hủy</button>
            </div>
          </div>
        </div>

        <!-- Domains List -->
        <div v-if="domains.length === 0 && !showAdd" class="ui-empty" style="padding: 3rem;">
          <Globe :size="40" class="ui-empty-icon" />
          <h3 class="ui-empty-title">Chưa có tên miền nào</h3>
          <p class="ui-empty-desc">Nhấn "Thêm Tên Miền" góc trên để bắt đầu thêm custom domain cho hệ thống rút gọn của bạn.</p>
        </div>

        <div v-if="domains.length > 0" style="display: flex; flex-direction: column; gap: 1rem;">
          <div 
            v-for="domain in domains" 
            :key="domain.id" 
            class="ui-panel"
          >
            <div class="ui-panel-body" style="display: flex; flex-wrap: wrap; gap: 1.5rem; justify-content: space-between; align-items: flex-start;">
              
              <!-- Info -->
              <div style="display: flex; gap: 1rem; flex: 1; min-width: 250px;">
                <div style="width: 44px; height: 44px; border-radius: 12px; background: #eff6ff; display: grid; place-items: center; flex-shrink: 0;">
                  <Globe :size="20" style="color: #3b82f6;" />
                </div>
                <div style="min-width: 0;">
                  <h4 style="margin: 0; font-size: 1.1rem; font-weight: 700; color: #0f172a;">{{ domain.host }}</h4>
                  <div style="margin-top: 0.35rem; display: flex; align-items: center; gap: 0.5rem;">
                    <span v-if="domain.isVerified" class="ui-badge ui-badge-success" style="display: inline-flex; align-items: center; gap: 0.2rem;">
                      <CheckCircle :size="11" /> Đã xác minh
                    </span>
                    <span v-else class="ui-badge ui-badge-warning" style="display: inline-flex; align-items: center; gap: 0.2rem;">
                      <Clock :size="11" /> Đang chờ xác minh
                    </span>
                  </div>
                </div>
              </div>

              <!-- Required Actions / Token -->
              <div v-if="!domain.isVerified" style="flex: 1; min-width: 280px; background: #f8fafc; padding: 0.8rem 1rem; border-radius: 8px; border: 1px solid #e2e8f0;">
                <p style="margin: 0 0 0.4rem; font-size: 0.75rem; font-weight: 700; color: #475569; text-transform: uppercase;">Mã xác thực TXT</p>
                <div style="background: #fff; padding: 0.5rem; border: 1px solid #cbd5e1; border-radius: 6px; font-family: monospace; font-size: 0.8rem; color: #0f172a; word-break: break-all;">
                  {{ domain.verificationToken }}
                </div>
              </div>

              <!-- Actions -->
              <div style="display: flex; align-items: center; gap: 0.5rem; padding-top: 0.5rem;">
                <button 
                  v-if="!domain.isVerified"
                  class="ui-btn ui-btn-outline"
                  style="border-color: #f59e0b; color: #b45309;"
                  :disabled="verifyLoadingId === domain.id"
                  @click="verifyDomain(domain.id)"
                >
                  <RefreshCw :size="14" :class="{'animate-spin': verifyLoadingId === domain.id}" /> Kiểm tra DNS
                </button>
                <button 
                  class="ui-btn ui-btn-ghost"
                  style="color: #ef4444; padding: 0.6rem;"
                  title="Xóa domain"
                  @click="deleteDomain(domain.id, domain.host)"
                >
                  <Trash2 :size="16" />
                </button>
              </div>

            </div>
          </div>
        </div>

      </div>
    </template>
  </div>
</template>
