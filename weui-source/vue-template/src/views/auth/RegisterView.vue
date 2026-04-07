<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink } from 'vue-router'
import WxInput from '@/components/ui/WxInput.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import WxCheckbox from '@/components/ui/WxCheckbox.vue'
import WxButton from '@/components/ui/WxButton.vue'

const fullName = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const agreeTerms = ref(false)
const loading = ref(false)

function handleRegister() {
  loading.value = true
  setTimeout(() => { loading.value = false }, 1500)
}
</script>

<template>
  <div class="space-y-5">
    <div class="text-center">
      <h2 class="text-lg font-bold text-on-surface">Create your account</h2>
      <p class="text-sm text-on-surface-variant mt-1">Start managing campaigns with Wemake</p>
    </div>

    <div>
      <label class="block text-sm font-medium text-on-surface mb-1.5">Full Name</label>
      <WxInput v-model="fullName" placeholder="Nguyen Van An" />
    </div>
    <div>
      <label class="block text-sm font-medium text-on-surface mb-1.5">Email</label>
      <WxInput v-model="email" type="email" placeholder="you@wemake.vn" />
    </div>
    <div>
      <label class="block text-sm font-medium text-on-surface mb-1.5">Password</label>
      <WxPasswordInput v-model="password" placeholder="Create a password" />
    </div>
    <div>
      <label class="block text-sm font-medium text-on-surface mb-1.5">Confirm Password</label>
      <WxPasswordInput v-model="confirmPassword" placeholder="Confirm your password" />
    </div>
    <WxCheckbox v-model="agreeTerms" label="I agree to the Terms of Service and Privacy Policy" />
    <WxButton variant="cta" size="lg" class="w-full" :loading="loading" :disabled="!agreeTerms" @click="handleRegister">
      Create Account
    </WxButton>

    <p class="text-center text-sm text-on-surface-variant">
      Already have an account?
      <RouterLink to="/auth/login" class="text-primary font-medium hover:underline">Sign in</RouterLink>
    </p>
  </div>
</template>
