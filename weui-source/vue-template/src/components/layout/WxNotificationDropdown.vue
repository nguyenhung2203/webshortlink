<script setup lang="ts">
import { ref } from 'vue'
import { Bell, MessageSquare, ShoppingCart, Users, AlertTriangle } from 'lucide-vue-next'

const isOpen = ref(false)

const notifications = [
  {
    id: 1,
    icon: MessageSquare,
    iconBg: 'bg-blue-500/15',
    iconColor: 'text-blue-500',
    text: '5 new messages in your inbox',
    time: '3 mins ago',
  },
  {
    id: 2,
    icon: Users,
    iconBg: 'bg-green-500/15',
    iconColor: 'text-green-500',
    text: 'New user registered: John Doe',
    time: '12 mins ago',
  },
  {
    id: 3,
    icon: ShoppingCart,
    iconBg: 'bg-amber-500/15',
    iconColor: 'text-amber-500',
    text: '3 new orders have been placed',
    time: '1 hour ago',
  },
  {
    id: 4,
    icon: AlertTriangle,
    iconBg: 'bg-red-500/15',
    iconColor: 'text-red-500',
    text: 'Server CPU usage is at 92%',
    time: '2 hours ago',
  },
]

function toggle() {
  isOpen.value = !isOpen.value
}

function close() {
  isOpen.value = false
}
</script>

<template>
  <div class="relative">
    <button
      class="relative flex h-9 w-9 items-center justify-center rounded-lg text-white/90 transition-colors hover:bg-white/15 hover:text-white"
      @click="toggle"
    >
      <Bell :size="18" />
      <span
        class="absolute right-1.5 top-1.5 flex h-4 w-4 items-center justify-center rounded-full bg-danger text-[10px] font-bold text-white"
      >
        4
      </span>
    </button>

    <!-- Dropdown overlay (transparent, behind everything visual but catches clicks) -->
    <div v-if="isOpen" class="fixed inset-0 z-30" @click="close" />

    <!-- Dropdown -->
    <Transition name="dropdown">
      <div
        v-if="isOpen"
        class="absolute right-0 top-full z-50 mt-2 w-80 overflow-hidden rounded-xl border border-outline bg-surface-container shadow-xl"
      >
        <!-- Header -->
        <div class="border-b border-outline bg-surface-container-high px-4 py-3">
          <h3 class="text-sm font-semibold text-on-surface">4 New Notifications</h3>
        </div>

        <!-- Items -->
        <div class="max-h-72 overflow-y-auto">
          <div
            v-for="item in notifications"
            :key="item.id"
            class="flex cursor-pointer items-start gap-3 border-b border-outline/50 px-4 py-3 transition-colors hover:bg-surface-container-high"
          >
            <div
              class="flex h-9 w-9 shrink-0 items-center justify-center rounded-full"
              :class="[item.iconBg]"
            >
              <component :is="item.icon" :size="16" :class="item.iconColor" />
            </div>
            <div class="min-w-0 flex-1">
              <p class="text-sm text-on-surface">{{ item.text }}</p>
              <p class="mt-0.5 text-xs text-on-surface-variant">{{ item.time }}</p>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="border-t border-outline px-4 py-2.5 text-center">
          <button class="text-sm font-medium text-primary hover:underline" @click="close">
            See All Notifications
          </button>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.dropdown-enter-active,
.dropdown-leave-active {
  transition: opacity 0.15s ease, transform 0.15s ease;
}
.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
