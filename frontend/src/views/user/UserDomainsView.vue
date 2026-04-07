<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { DomainService, UserService } from '@/api/services'
import { useAuthStore } from '@/stores/auth'
import { Globe, Plus, CheckCircle, XCircle, Shield, Trash2, RefreshCw, Lock } from 'lucide-vue-next'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxButton from '@/components/ui/WxButton.vue'

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
    const result = await DomainService.verify(authStore.accessToken, domainId)
    if (result?.verified) {
      alert('✅ Domain đã được xác minh thành công!')
    } else {
      alert('⚠️ Xác minh DNS chưa thành công. Hãy đảm bảo bạn đã thêm TXT record đúng.')
    }
    await load()
  } catch (err) {
    alert(err instanceof Error ? err.message : 'Lỗi khi xác minh domain.')
  } finally {
    verifyLoadingId.value = null
  }
}

async function deleteDomain(domainId: string, host: string) {
  if (!authStore.accessToken || !confirm(`Xóa domain ${host}?`)) return
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
  <div class="flex flex-col gap-6">
    <WxPageHeader
      title="Tên miền riêng"
      description="Kết nối tên miền của bạn để tạo link rút gọn thương hiệu."
    >
      <template #actions>
        <WxButton v-if="isPro" variant="primary" @click="showAdd = !showAdd">
          <Plus :size="16" class="mr-1" /> Thêm Domain
        </WxButton>
      </template>
    </WxPageHeader>

    <!-- Paywall for Free -->
    <div v-if="!isPro" class="bg-surface-container rounded-2xl border border-outline p-8 text-center">
      <Lock :size="36" class="text-on-surface-variant mx-auto mb-3 opacity-40" />
      <h3 class="font-bold text-on-surface mb-2">Tính năng dành cho gói Pro / Plus</h3>
      <p class="text-sm text-on-surface-variant mb-5">Nâng cấp để sử dụng tên miền riêng trên link rút gọn của bạn.</p>
      <RouterLink to="/app/billing" class="inline-flex items-center gap-2 bg-primary text-on-primary font-semibold px-5 py-2.5 rounded-xl text-sm hover:brightness-110 transition-all">
        Xem bảng giá
      </RouterLink>
    </div>

    <template v-else>
      <!-- DNS Instruction -->
      <div class="bg-primary/5 border border-primary/20 rounded-2xl p-5">
        <div class="flex items-start gap-3">
          <Shield :size="20" class="text-primary shrink-0 mt-0.5" />
          <div class="text-sm">
            <p class="font-semibold text-on-surface mb-1">Hướng dẫn xác minh domain</p>
            <p class="text-on-surface-variant">Trỏ domain của bạn về IP máy chủ bằng record <strong class="font-mono text-on-surface">A 203.0.113.1</strong> hoặc <strong class="font-mono text-on-surface">CNAME sho.rt</strong>, sau đó nhấn <strong>Xác minh</strong>.</p>
          </div>
        </div>
      </div>

      <!-- Add Form -->
      <div v-if="showAdd" class="bg-surface-container rounded-2xl border border-outline p-5">
        <h3 class="font-semibold text-on-surface mb-3">Thêm domain mới</h3>
        <div class="flex gap-3">
          <div class="flex-1">
            <input
              v-model="newHost"
              type="text"
              placeholder="link.yourbrand.com"
              class="w-full h-10 rounded-lg border border-outline bg-surface px-3 text-sm focus:outline-none focus:border-primary"
              @keydown.enter="addDomain"
            />
          </div>
          <WxButton variant="primary" :loading="addLoading" @click="addDomain">Thêm</WxButton>
          <WxButton variant="ghost" @click="showAdd = false">Hủy</WxButton>
        </div>
        <p v-if="error" class="text-danger text-xs mt-2">{{ error }}</p>
      </div>

      <!-- Domains List -->
      <div v-if="loading" class="text-center py-10 text-on-surface-variant">Đang tải...</div>
      <div v-else-if="domains.length === 0" class="bg-surface-container rounded-2xl border border-outline p-8 text-center">
        <Globe :size="36" class="text-on-surface-variant mx-auto mb-3 opacity-40" />
        <p class="text-on-surface-variant text-sm">Chưa có domain nào. Thêm domain đầu tiên của bạn!</p>
      </div>
      <div v-else class="flex flex-col gap-3">
        <div
          v-for="domain in domains"
          :key="domain.id"
          class="bg-surface-container rounded-2xl border border-outline p-5 flex flex-col sm:flex-row sm:items-center gap-4"
        >
          <div class="flex items-center gap-3 flex-1 min-w-0">
            <Globe :size="20" class="text-on-surface-variant shrink-0" />
            <div class="min-w-0">
              <p class="font-semibold text-on-surface truncate">{{ domain.host }}</p>
              <div class="flex items-center gap-2 mt-1">
                <span v-if="domain.isVerified" class="flex items-center gap-1 text-xs text-success font-medium">
                  <CheckCircle :size="12" /> Đã xác minh
                </span>
                <span v-else class="flex items-center gap-1 text-xs text-warning font-medium">
                  <XCircle :size="12" /> Chờ xác minh
                </span>
              </div>
            </div>
          </div>

          <div class="flex items-center gap-2 shrink-0">
            <WxButton
              v-if="!domain.isVerified"
              size="sm"
              variant="secondary"
              :loading="verifyLoadingId === domain.id"
              @click="verifyDomain(domain.id)"
            >
              <RefreshCw :size="13" class="mr-1" /> Xác minh
            </WxButton>
            <button
              @click="deleteDomain(domain.id, domain.host)"
              class="text-danger hover:bg-danger/10 rounded-lg p-1.5 transition-colors"
            >
              <Trash2 :size="15" />
            </button>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>
