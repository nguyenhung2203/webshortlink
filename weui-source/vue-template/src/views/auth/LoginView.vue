<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import WxInput from '@/components/ui/WxInput.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import WxCheckbox from '@/components/ui/WxCheckbox.vue'
import WxButton from '@/components/ui/WxButton.vue'

const email = ref('')
const password = ref('')
const remember = ref(false)
const loading = ref(false)

function handleLogin() {
  loading.value = true
  setTimeout(() => { loading.value = false }, 1500)
}
</script>

<template>
  <div class="space-y-5">
    <div class="text-center">
      <h2 class="text-lg font-bold text-on-surface">Sign in to your account</h2>
      <p class="text-sm text-on-surface-variant mt-1">Enter your credentials below</p>
    </div>

    <div>
      <label class="block text-sm font-medium text-on-surface mb-1.5">Email</label>
      <WxInput v-model="email" type="email" placeholder="you@wemake.vn" />
    </div>
    <div>
      <label class="block text-sm font-medium text-on-surface mb-1.5">Password</label>
      <WxPasswordInput v-model="password" placeholder="Enter your password" />
    </div>
    <div class="flex items-center justify-between">
      <WxCheckbox v-model="remember" label="Remember me" />
      <RouterLink to="/auth/forgot-password" class="text-sm text-primary hover:underline">
        Forgot password?
      </RouterLink>
    </div>
    <WxButton variant="cta" size="lg" class="w-full" :loading="loading" @click="handleLogin">
      Sign In
    </WxButton>

    <p class="text-center text-sm text-on-surface-variant">
      Don't have an account?
      <RouterLink to="/auth/register" class="text-primary font-medium hover:underline">
        Create one
      </RouterLink>
    </p>
  </div>
</template>
