<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { SearchX, Clock, Ban, MousePointerClick, ArrowLeft } from 'lucide-vue-next'
import { computed } from 'vue'

const props = defineProps<{
  type: 'not-found' | 'expired' | 'disabled' | 'limit-reached'
}>()

const config = computed(() => {
  switch (props.type) {
    case 'not-found':
      return {
        icon: SearchX,
        iconClass: 'text-danger',
        bgClass: 'bg-danger/10',
        title: 'Link không tồn tại',
        description: 'Link rút gọn này không tồn tại hoặc đã bị xóa khỏi hệ thống. Vui lòng kiểm tra lại địa chỉ.',
        badge: '404',
        badgeClass: 'bg-danger/10 text-danger',
      }
    case 'expired':
      return {
        icon: Clock,
        iconClass: 'text-warning',
        bgClass: 'bg-warning/10',
        title: 'Link đã hết hạn',
        description: 'Link rút gọn này đã hết thời gian sử dụng. Vui lòng liên hệ người tạo link để được cấp đường dẫn mới.',
        badge: 'Hết hạn',
        badgeClass: 'bg-warning/10 text-warning',
      }
    case 'disabled':
      return {
        icon: Ban,
        iconClass: 'text-on-surface-variant',
        bgClass: 'bg-surface-container-high',
        title: 'Link đã bị tắt',
        description: 'Đường dẫn này hiện đang tạm dừng hoạt động. Vui lòng liên hệ người tạo link để biết thêm thông tin.',
        badge: 'Tắt',
        badgeClass: 'bg-surface-container-high text-on-surface-variant',
      }
    case 'limit-reached':
      return {
        icon: MousePointerClick,
        iconClass: 'text-primary',
        bgClass: 'bg-primary/10',
        title: 'Link đã đạt giới hạn',
        description: 'Đường dẫn này đã đạt đến số lượt click tối đa cho phép và không thể truy cập thêm.',
        badge: 'Hết lượt',
        badgeClass: 'bg-primary/10 text-primary',
      }
  }
})
</script>

<template>
  <div class="flex min-h-screen flex-col items-center justify-center px-4 text-center">
    <div class="max-w-md w-full">
      <!-- Icon -->
      <div
        class="w-20 h-20 rounded-full flex items-center justify-center mx-auto mb-6"
        :class="config.bgClass"
      >
        <component :is="config.icon" :size="34" :class="config.iconClass" />
      </div>

      <!-- Badge -->
      <span class="inline-block text-xs font-bold px-3 py-1 rounded-full mb-4 uppercase tracking-widest"
        :class="config.badgeClass">
        {{ config.badge }}
      </span>

      <!-- Title & desc -->
      <h1 class="text-2xl md:text-3xl font-extrabold text-on-surface mb-3">{{ config.title }}</h1>
      <p class="text-on-surface-variant text-sm leading-relaxed mb-8">{{ config.description }}</p>

      <!-- Actions -->
      <div class="flex flex-col sm:flex-row gap-3 justify-center">
        <RouterLink
          to="/"
          class="flex items-center justify-center gap-2 bg-surface-container-high border border-outline rounded-xl px-5 py-2.5 text-sm font-semibold text-on-surface hover:bg-surface-container-highest transition-colors"
        >
          <ArrowLeft :size="15" />
          Trang chủ
        </RouterLink>
        <RouterLink
          to="/auth/register"
          class="flex items-center justify-center gap-2 bg-primary text-on-primary rounded-xl px-5 py-2.5 text-sm font-semibold hover:brightness-110 transition-all"
        >
          Tạo link của bạn
        </RouterLink>
      </div>
    </div>
  </div>
</template>
