<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { Zap, Check, ArrowRight } from 'lucide-vue-next'

const plans = [
  {
    id: 1,
    name: 'Thường',
    price: 'Miễn phí',
    period: '',
    description: 'Dành cho người mới bắt đầu.',
    cta: 'Dùng miễn phí',
    ctaRoute: '/auth/register',
    highlighted: false,
    features: [
      '100 link rút gọn',
      'Thống kê click cơ bản',
      'Slug tùy chỉnh',
      'Lưu trữ vĩnh viễn',
    ],
    disabled: [],
  },
  {
    id: 2,
    name: 'Pro',
    price: '199.000đ',
    period: '/ tháng',
    description: 'Cho cá nhân và freelancer năng động.',
    cta: 'Nâng cấp Pro',
    ctaRoute: '/auth/register',
    highlighted: true,
    features: [
      '5.000 link rút gọn',
      'Phân tích nâng cao (thiết bị, quốc gia)',
      'Bảo vệ link bằng mật khẩu',
      'Link có hạn sử dụng',
      'Giới hạn số lần click',
      '3 tên miền riêng',
    ],
    disabled: [],
  },
  {
    id: 3,
    name: 'Plus',
    price: '499.000đ',
    period: '/ tháng',
    description: 'Cho doanh nghiệp và team lớn.',
    cta: 'Nâng cấp Plus',
    ctaRoute: '/auth/register',
    highlighted: false,
    features: [
      '50.000 link rút gọn',
      'Toàn bộ tính năng Pro',
      'Targeting nâng cao (geo/device)',
      '20 tên miền riêng',
      'Phân tích giữ lại dữ liệu lâu nhất',
      'Ưu tiên hỗ trợ kỹ thuật',
    ],
    disabled: [],
  },
]
</script>

<template>
  <div class="min-h-screen py-16 px-4">
    <!-- Header -->
    <div class="text-center mb-14">
      <div class="inline-flex items-center gap-2 bg-primary/10 text-primary font-semibold text-sm px-4 py-1.5 rounded-full mb-4">
        <Zap :size="14" />
        WeShort by Wemake
      </div>
      <h1 class="text-4xl md:text-5xl font-extrabold text-on-surface mb-4">Bảng giá minh bạch</h1>
      <p class="text-on-surface-variant text-lg max-w-xl mx-auto">
        Chọn gói phù hợp. Nâng cấp hoặc hủy bất kỳ lúc nào, không ràng buộc.
      </p>
    </div>

    <!-- Plans -->
    <div class="max-w-5xl mx-auto grid grid-cols-1 md:grid-cols-3 gap-6">
      <div
        v-for="plan in plans"
        :key="plan.id"
        class="relative flex flex-col rounded-2xl border p-7 transition-all duration-200"
        :class="plan.highlighted
          ? 'border-primary bg-primary/5 shadow-lg shadow-primary/20 scale-[1.03]'
          : 'border-outline bg-surface-container'"
      >
        <div v-if="plan.highlighted" class="absolute -top-3.5 left-1/2 -translate-x-1/2">
          <span class="bg-primary text-on-primary text-xs font-bold px-3 py-1 rounded-full shadow">PHỔ BIẾN NHẤT</span>
        </div>

        <div class="mb-6">
          <h2 class="text-lg font-bold text-on-surface mb-1">{{ plan.name }}</h2>
          <p class="text-sm text-on-surface-variant mb-4">{{ plan.description }}</p>
          <div class="flex items-end gap-1">
            <span class="text-3xl font-extrabold text-on-surface">{{ plan.price }}</span>
            <span v-if="plan.period" class="text-on-surface-variant text-sm mb-1">{{ plan.period }}</span>
          </div>
        </div>

        <ul class="flex flex-col gap-2.5 flex-1 mb-7">
          <li v-for="feature in plan.features" :key="feature" class="flex items-start gap-2 text-sm text-on-surface">
            <Check :size="16" class="text-success mt-0.5 shrink-0" />
            {{ feature }}
          </li>
        </ul>

        <RouterLink
          :to="plan.ctaRoute"
          class="flex items-center justify-center gap-2 rounded-xl font-semibold py-3 text-sm transition-all duration-200"
          :class="plan.highlighted
            ? 'bg-primary text-on-primary hover:brightness-110'
            : 'border border-outline text-on-surface hover:bg-surface-container-high'"
        >
          {{ plan.cta }}
          <ArrowRight :size="15" />
        </RouterLink>
      </div>
    </div>

    <!-- Bottom note -->
    <p class="text-center text-sm text-on-surface-variant mt-12">
      Đã có tài khoản?
      <RouterLink to="/auth/login" class="text-primary font-semibold hover:underline">Đăng nhập ngay</RouterLink>
    </p>
  </div>
</template>
