<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import WxInput from '@/components/ui/WxInput.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { CheckCircle2 } from 'lucide-vue-next'

const email = ref('')
const loading = ref(false)
const sent = ref(false)

function handleReset() {
  loading.value = true
  setTimeout(() => {
    loading.value = false
    sent.value = true
  }, 1500)
}
</script>

<template>
  <div class="space-y-5">
    <div class="text-center">
      <h2 class="text-lg font-bold text-on-surface">Reset password</h2>
      <p class="text-sm text-on-surface-variant mt-1">
        {{ sent ? 'Check your inbox for the reset link' : 'Enter your email to receive a reset link' }}
      </p>
    </div>

    <template v-if="!sent">
      <div>
        <label class="block text-sm font-medium text-on-surface mb-1.5">Email Address</label>
        <WxInput v-model="email" type="email" placeholder="you@wemake.vn" />
      </div>
      <WxButton variant="cta" size="lg" class="w-full" :loading="loading" @click="handleReset">
        Reset Password
      </WxButton>
    </template>
    <template v-else>
      <div class="text-center py-4">
        <div class="w-16 h-16 mx-auto mb-4 rounded-full bg-success/10 flex items-center justify-center">
          <CheckCircle2 class="w-8 h-8 text-success" />
        </div>
        <p class="text-sm text-on-surface-variant">
          We sent a password reset link to <span class="font-medium text-on-surface">{{ email }}</span>.
        </p>
      </div>
      <WxButton variant="secondary" size="lg" class="w-full" @click="sent = false">
        Try another email
      </WxButton>
    </template>

    <p class="text-center text-sm text-on-surface-variant">
      <RouterLink to="/auth/login" class="text-primary font-medium hover:underline">Back to sign in</RouterLink>
    </p>
  </div>
</template>
