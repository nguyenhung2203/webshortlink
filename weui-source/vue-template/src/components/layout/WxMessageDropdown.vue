<script setup lang="ts">
import { ref } from 'vue'
import { MessageSquare } from 'lucide-vue-next'

const isOpen = ref(false)

const messages = [
  {
    id: 1,
    sender: 'Sarah Johnson',
    initials: 'SJ',
    color: 'bg-blue-500',
    preview: 'Hey, can you review the latest design mockups?',
    time: '5 mins ago',
  },
  {
    id: 2,
    sender: 'Michael Chen',
    initials: 'MC',
    color: 'bg-green-500',
    preview: 'The deployment was successful. All tests passed.',
    time: '25 mins ago',
  },
  {
    id: 3,
    sender: 'Emily Davis',
    initials: 'ED',
    color: 'bg-purple-500',
    preview: 'Meeting rescheduled to 3 PM tomorrow.',
    time: '1 hour ago',
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
      <MessageSquare :size="18" />
      <span
        class="absolute right-1.5 top-1.5 flex h-4 w-4 items-center justify-center rounded-full bg-success text-[10px] font-bold text-white"
      >
        3
      </span>
    </button>

    <!-- Dropdown overlay -->
    <div v-if="isOpen" class="fixed inset-0 z-30" @click="close" />

    <!-- Dropdown -->
    <Transition name="dropdown">
      <div
        v-if="isOpen"
        class="absolute right-0 top-full z-50 mt-2 w-80 overflow-hidden rounded-xl border border-outline bg-surface-container shadow-xl"
      >
        <!-- Header -->
        <div class="border-b border-outline bg-surface-container-high px-4 py-3">
          <h3 class="text-sm font-semibold text-on-surface">3 New Messages</h3>
        </div>

        <!-- Items -->
        <div class="max-h-72 overflow-y-auto">
          <div
            v-for="msg in messages"
            :key="msg.id"
            class="flex cursor-pointer items-start gap-3 border-b border-outline/50 px-4 py-3 transition-colors hover:bg-surface-container-high"
          >
            <div
              class="flex h-9 w-9 shrink-0 items-center justify-center rounded-full text-xs font-bold text-white"
              :class="msg.color"
            >
              {{ msg.initials }}
            </div>
            <div class="min-w-0 flex-1">
              <div class="flex items-center justify-between">
                <p class="text-sm font-medium text-on-surface">{{ msg.sender }}</p>
                <span class="text-[11px] text-on-surface-variant">{{ msg.time }}</span>
              </div>
              <p class="mt-0.5 truncate text-xs text-on-surface-variant">
                {{ msg.preview }}
              </p>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="border-t border-outline px-4 py-2.5 text-center">
          <button class="text-sm font-medium text-primary hover:underline" @click="close">
            See All Messages
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
