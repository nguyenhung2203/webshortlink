<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { ShieldCheck } from 'lucide-vue-next'

const router = useRouter()
const newPassword = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const success = ref(false)

const strengthScore = computed(() => {
  const p = newPassword.value
  if (!p) return 0
  let score = 0
  if (p.length >= 8) score++
  if (/[A-Z]/.test(p)) score++
  if (/[0-9]/.test(p)) score++
  if (/[^A-Za-z0-9]/.test(p)) score++
  return score
})

const strengthLabel = computed(() => ['', 'Weak', 'Fair', 'Good', 'Strong'][strengthScore.value])
const strengthColor = computed(() => ['bg-transparent', 'bg-red-500', 'bg-amber-500', 'bg-blue-500', 'bg-emerald-500'][strengthScore.value])
const passwordsMatch = computed(() => confirmPassword.value.length > 0 && newPassword.value === confirmPassword.value)
const canSubmit = computed(() => strengthScore.value >= 2 && passwordsMatch.value)

function handleReset() {
  if (!canSubmit.value) return
  loading.value = true
  setTimeout(() => {
    loading.value = false
    success.value = true
  }, 1500)
}
</script>

<template>
  <div class="space-y-5">
    <div class="text-center">
      <h2 class="text-lg font-bold text-on-surface">Reset Your Password</h2>
      <p class="text-sm text-on-surface-variant mt-1">Choose a strong new password</p>
    </div>

    <template v-if="!success">
      <div>
        <label class="block text-sm font-medium text-on-surface mb-1.5">New Password</label>
        <WxPasswordInput v-model="newPassword" placeholder="Enter new password" />
        <div v-if="newPassword" class="mt-2 space-y-1">
          <div class="flex gap-1">
            <div v-for="i in 4" :key="i" class="h-1.5 flex-1 rounded-full transition-colors duration-300"
              :class="i <= strengthScore ? strengthColor : 'bg-outline-variant/30'" />
          </div>
          <p class="text-xs" :class="strengthScore >= 3 ? 'text-success' : strengthScore >= 2 ? 'text-info' : 'text-danger'">
            {{ strengthLabel }}
          </p>
        </div>
      </div>
      <div>
        <label class="block text-sm font-medium text-on-surface mb-1.5">Confirm Password</label>
        <WxPasswordInput v-model="confirmPassword" placeholder="Confirm new password"
          :error="confirmPassword.length > 0 && !passwordsMatch"
          :error-message="confirmPassword.length > 0 && !passwordsMatch ? 'Passwords do not match' : ''" />
      </div>
      <WxButton variant="cta" size="lg" class="w-full" :loading="loading" :disabled="!canSubmit" @click="handleReset">
        Reset Password
      </WxButton>
    </template>
    <template v-else>
      <div class="text-center py-4">
        <div class="w-16 h-16 mx-auto mb-4 rounded-full bg-success/10 flex items-center justify-center">
          <ShieldCheck class="w-8 h-8 text-success" />
        </div>
        <h3 class="text-lg font-semibold text-on-surface">Password Updated</h3>
        <p class="text-sm text-on-surface-variant mt-1">Your password has been reset successfully.</p>
      </div>
      <WxButton variant="cta" size="lg" class="w-full" @click="router.push('/auth/login')">
        Continue to Login
      </WxButton>
    </template>

    <p class="text-center text-sm text-on-surface-variant">
      <a class="text-primary font-medium hover:underline cursor-pointer" @click="router.push('/auth/login')">Back to Login</a>
    </p>
  </div>
</template>
