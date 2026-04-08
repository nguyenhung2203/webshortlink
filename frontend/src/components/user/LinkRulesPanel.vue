<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { LinkRuleService } from '@/api/services'
import { Plus, Trash2, GitBranch, Lock } from 'lucide-vue-next'
import WxButton from '@/components/ui/WxButton.vue'

const props = defineProps<{
  linkId: string
  isPro: boolean   // true if plan >= Pro
}>()

const authStore = useAuthStore()
const rules = ref<any[]>([])
const loading = ref(true)
const addLoading = ref(false)
const error = ref('')
const showAdd = ref(false)
const editingRuleId = ref<string | null>(null)

const ruleTypes = [
  { value: 'GeoTargeting', label: '🌍 Quốc gia (Geo)' },
  { value: 'DeviceTargeting', label: '📱 Thiết bị' },
  { value: 'BrowserTargeting', label: '🌐 Trình duyệt' },
  { value: 'OsTargeting', label: '💻 Hệ điều hành' },
  { value: 'Rotation', label: '🔄 Xoay vòng URL' },
  { value: 'Percentage', label: '⚖️ Phân bổ theo trọng số (Weighted)' },
]

const form = ref({
  ruleType: 'GeoTargeting',
  ruleValue: '',
  targetUrl: '',
  priority: 1,
  isActive: true,
})

const ruleValueHint = computed(() => {
  switch (form.value.ruleType) {
    case 'GeoTargeting': return 'VD: VN,US,JP (mã quốc gia ISO 2 ký tự, phân cách bằng dấu phẩy)'
    case 'DeviceTargeting': return 'VD: Mobile,Desktop'
    case 'BrowserTargeting': return 'VD: Chrome,Safari,Firefox'
    case 'OsTargeting': return 'VD: Android,iOS,Windows'
    case 'Rotation': return 'VD: https://url1.com (rule tiếp theo sẽ là URL 2, xoay vòng đều)'
    case 'Percentage': return 'VD: 50 (Trọng số. VD: Rule A trọng số 7, Rule B trọng số 3 => tỷ lệ 70/30)'
    default: return ''
  }
})

async function load() {
  loading.value = true
  try {
    if (!authStore.accessToken) return
    rules.value = await LinkRuleService.list(authStore.accessToken, props.linkId)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tải rules.'
  } finally {
    loading.value = false
  }
}

async function addRule() {
  if (!authStore.accessToken || !form.value.ruleValue || !form.value.targetUrl) return
  addLoading.value = true
  error.value = ''
  try {
    if (editingRuleId.value) {
      await LinkRuleService.update(authStore.accessToken, props.linkId, editingRuleId.value, form.value)
    } else {
      await LinkRuleService.create(authStore.accessToken, props.linkId, form.value)
    }
    showAdd.value = false
    editingRuleId.value = null
    form.value = { ruleType: 'GeoTargeting', ruleValue: '', targetUrl: '', priority: 1, isActive: true }
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể tạo rule.'
  } finally {
    addLoading.value = false
  }
}

const confirmDeleteId = ref<string | null>(null)

async function deleteRule(ruleId: string) {
  if (!authStore.accessToken) return
  try {
    await LinkRuleService.delete(authStore.accessToken, props.linkId, ruleId)
    confirmDeleteId.value = null
    await load()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Không thể xóa rule.'
  }
}

function editRule(rule: any) {
  editingRuleId.value = rule.id
  form.value = {
    ruleType: rule.ruleType,
    ruleValue: rule.ruleValue,
    targetUrl: rule.targetUrl,
    priority: rule.priority,
    isActive: rule.isActive,
  }
  showAdd.value = true
}

onMounted(load)
</script>

<template>
  <div class="rounded-2xl border border-outline bg-surface-container overflow-hidden">
    <!-- Header -->
    <div class="flex items-center justify-between p-5 border-b border-outline">
      <div class="flex items-center gap-2">
        <GitBranch :size="18" class="text-primary" />
        <h3 class="font-semibold text-on-surface">Rules chuyển hướng</h3>
        <span class="text-xs text-on-surface-variant ml-1">({{ rules.length }})</span>
      </div>
      <template v-if="isPro">
        <WxButton size="sm" variant="primary" @click="showAdd = !showAdd">
          <Plus :size="14" /> Thêm rule
        </WxButton>
      </template>
      <template v-else>
        <span class="flex items-center gap-1.5 text-xs text-on-surface-variant bg-surface-container-high px-2.5 py-1.5 rounded-full font-medium">
          <Lock :size="12" /> Cần gói Pro trở lên
        </span>
      </template>
    </div>

    <!-- Add Rule Form -->
    <div v-if="showAdd && isPro" class="p-5 bg-surface-container-low border-b border-outline">
      <div class="flex flex-col gap-3">
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <div>
            <label class="block text-xs font-semibold text-on-surface-variant mb-1">Loại Rule</label>
            <select v-model="form.ruleType" class="w-full h-10 rounded-lg border border-outline bg-surface px-3 text-sm text-on-surface focus:outline-none focus:border-primary">
              <option v-for="t in ruleTypes" :key="t.value" :value="t.value">{{ t.label }}</option>
            </select>
          </div>
          <div>
            <label class="block text-xs font-semibold text-on-surface-variant mb-1">Độ ưu tiên</label>
            <input v-model.number="form.priority" type="number" min="1" max="100" class="w-full h-10 rounded-lg border border-outline bg-surface px-3 text-sm focus:outline-none focus:border-primary" />
          </div>
        </div>
        <div>
          <label class="block text-xs font-semibold text-on-surface-variant mb-1">Giá trị điều kiện</label>
          <input v-model="form.ruleValue" type="text" :placeholder="ruleValueHint" class="w-full h-10 rounded-lg border border-outline bg-surface px-3 text-sm focus:outline-none focus:border-primary" />
          <p class="text-xs text-on-surface-variant mt-1">{{ ruleValueHint }}</p>
        </div>
        <div>
          <label class="block text-xs font-semibold text-on-surface-variant mb-1">Chuyển hướng đến URL</label>
          <input v-model="form.targetUrl" type="url" placeholder="https://example.com/landing-page" class="w-full h-10 rounded-lg border border-outline bg-surface px-3 text-sm focus:outline-none focus:border-primary" />
        </div>
        <p v-if="error" class="text-danger text-xs">{{ error }}</p>
        <div class="flex gap-2">
          <WxButton size="sm" variant="primary" :loading="addLoading" @click="addRule">
            {{ editingRuleId ? 'Cập nhật Rule' : 'Lưu Rule' }}
          </WxButton>
          <WxButton size="sm" variant="ghost" @click="showAdd = false; editingRuleId = null">Hủy</WxButton>
        </div>
      </div>
    </div>

    <!-- Rules List -->
    <div v-if="loading" class="p-5 text-sm text-on-surface-variant text-center">Đang tải rules...</div>
    <div v-else-if="rules.length === 0" class="p-6 text-center text-sm text-on-surface-variant">
      <p>Chưa có rule nào. {{ isPro ? 'Thêm rule để điều hướng thông minh theo thiết bị, quốc gia, v.v.' : '' }}</p>
    </div>
    <div v-else class="divide-y divide-outline">
      <div v-for="rule in rules" :key="rule.id" class="flex items-center gap-3 px-5 py-4">
        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-2 mb-0.5">
            <span class="text-xs font-bold text-primary bg-primary/10 px-2 py-0.5 rounded-full">{{ rule.ruleType }}</span>
            <span v-if="!rule.isActive" class="text-xs text-on-surface-variant">(Tắt)</span>
            <span class="text-xs text-on-surface-variant">P{{ rule.priority }}</span>
          </div>
          <p class="text-sm text-on-surface font-medium truncate">{{ rule.ruleValue }}</p>
          <p class="text-xs text-on-surface-variant truncate">→ {{ rule.targetUrl }}</p>
        </div>
        <!-- Editing / Deleting Actions -->
        <div class="flex items-center gap-2 shrink-0">
          <template v-if="confirmDeleteId === rule.id">
            <span class="text-xs text-danger font-medium">Chắc chắn xóa?</span>
            <button @click="deleteRule(rule.id)" class="text-xs text-white bg-danger rounded px-2 py-1">Xóa</button>
            <button @click="confirmDeleteId = null" class="text-xs text-on-surface-variant hover:bg-surface-container-high rounded px-2 py-1">Hủy</button>
          </template>
          <template v-else-if="isPro">
            <button
              @click="editRule(rule)"
              class="text-primary hover:bg-primary/10 rounded-lg p-1.5 transition-colors"
            >
              Sửa
            </button>
            <button
              @click="confirmDeleteId = rule.id"
              class="text-danger hover:bg-danger/10 rounded-lg p-1.5 transition-colors"
            >
              <Trash2 :size="15" />
            </button>
          </template>
        </div>
      </div>
    </div>
  </div>
</template>
