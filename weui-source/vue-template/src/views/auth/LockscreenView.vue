<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { Lock } from 'lucide-vue-next'

const router = useRouter()
const password = ref('')
const loading = ref(false)
const currentTime = ref('')
const currentDate = ref('')

function updateClock() {
  const now = new Date()
  currentTime.value = now.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: true })
  currentDate.value = now.toLocaleDateString('en-US', { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' })
}

let timer: ReturnType<typeof setInterval>
onMounted(() => { updateClock(); timer = setInterval(updateClock, 1000) })
onUnmounted(() => { clearInterval(timer) })

function handleUnlock() {
  if (!password.value) return
  loading.value = true
  setTimeout(() => { loading.value = false; password.value = '' }, 1500)
}
</script>

<template>
  <div class="space-y-5">
    <div class="text-center mb-2">
      <p class="text-4xl font-light text-on-surface tracking-tight">{{ currentTime }}</p>
      <p class="text-xs text-on-surface-variant mt-1">{{ currentDate }}</p>
    </div>

    <div class="flex flex-col items-center gap-2">
      <WxAvatar name="Admin User" size="xl" />
      <h2 class="text-lg font-semibold text-on-surface">Admin User</h2>
      <div class="flex items-center gap-1.5 text-on-surface-variant">
        <Lock :size="14" />
        <span class="text-xs">Session locked</span>
      </div>
    </div>

    <div>
      <label class="block text-sm font-medium text-on-surface mb-1.5">Password</label>
      <WxPasswordInput v-model="password" placeholder="Enter password to unlock" @keyup.enter="handleUnlock" />
    </div>

    <WxButton variant="cta" size="lg" class="w-full" :loading="loading" :disabled="!password" @click="handleUnlock">
      Unlock
    </WxButton>

    <p class="text-center text-sm text-on-surface-variant">
      <a class="text-primary font-medium hover:underline cursor-pointer" @click="router.push('/auth/login')">
        Or sign in as a different user
      </a>
    </p>
  </div>
</template>
