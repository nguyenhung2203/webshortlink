<template>
  <div class="min-h-screen bg-surface flex items-center justify-center p-4">
    <section class="max-w-4xl w-full bg-surface-container rounded-3xl shadow-2xl border border-outline-variant p-10 md:p-14 text-center">
      <p class="text-primary font-bold uppercase tracking-widest text-sm mb-3">Wemake Shortlink Platform</p>
      
      <h1 class="text-4xl md:text-6xl font-black text-on-surface mb-6 leading-tight tracking-tight">
        WeShort Nền tảng <br/> <span class="text-primary">quản lý URL thông minh</span>
      </h1>
      
      <p class="text-on-surface-variant text-xl mb-10 leading-relaxed max-w-2xl mx-auto font-medium">
        Biến những URL dài dòng thành đường link siêu ngắn. Theo dõi mọi lượt click, kiểm soát dữ liệu và phân tích người dùng hiệu quả nhất.
      </p>

      <div class="flex flex-col sm:flex-row gap-4 mt-6 justify-center">
        <WxButton variant="primary" to="/auth/register" size="lg" class="px-8 py-4 text-lg">🚀 Tạo tài khoản miễn phí</WxButton>
        <WxButton variant="secondary" to="/auth/login" size="lg" class="px-8 py-4 text-lg">Đăng nhập</WxButton>
      </div>

    </section>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
import WxButton from '@/components/ui/WxButton.vue'
</script>
