<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxStatCard from '@/components/ui/WxStatCard.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import { DollarSign, ShoppingCart, Clock, TrendingUp, Star } from 'lucide-vue-next'

const stats = [
  { title: 'Total Sales', value: '$125.4K', trend: 12, trendUp: true, icon: DollarSign },
  { title: 'Orders Today', value: '68', trend: 8, trendUp: true, icon: ShoppingCart },
  { title: 'Pending', value: '12', trend: 3, trendUp: false, icon: Clock },
  { title: 'Revenue Growth', value: '+15%', trend: 15, trendUp: true, icon: TrendingUp },
]

interface Order {
  id: string
  customer: string
  items: number
  total: string
  status: 'delivered' | 'processing' | 'shipped' | 'cancelled'
  date: string
}

const orders = ref<Order[]>([
  { id: 'ORD-2401', customer: 'James Wilson', items: 3, total: '$284.00', status: 'delivered', date: 'Mar 25, 2026' },
  { id: 'ORD-2402', customer: 'Emily Brown', items: 1, total: '$149.99', status: 'processing', date: 'Mar 25, 2026' },
  { id: 'ORD-2403', customer: 'Michael Lee', items: 5, total: '$532.50', status: 'shipped', date: 'Mar 24, 2026' },
  { id: 'ORD-2404', customer: 'Sofia Martinez', items: 2, total: '$89.00', status: 'delivered', date: 'Mar 24, 2026' },
  { id: 'ORD-2405', customer: 'David Kim', items: 4, total: '$410.00', status: 'cancelled', date: 'Mar 23, 2026' },
  { id: 'ORD-2406', customer: 'Olivia Taylor', items: 1, total: '$64.99', status: 'processing', date: 'Mar 23, 2026' },
])

const statusVariant: Record<string, 'success' | 'warning' | 'info' | 'error'> = {
  delivered: 'success',
  processing: 'warning',
  shipped: 'info',
  cancelled: 'error',
}

interface Product {
  name: string
  price: string
  rating: number
  sold: number
}

const products = ref<Product[]>([
  { name: 'Wireless Noise-Cancelling Headphones', price: '$299.99', rating: 5, sold: 1240 },
  { name: 'Smart Fitness Watch Pro', price: '$199.99', rating: 4, sold: 890 },
  { name: 'Portable Bluetooth Speaker', price: '$79.99', rating: 4, sold: 756 },
  { name: 'Ergonomic Mechanical Keyboard', price: '$149.99', rating: 5, sold: 623 },
])
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="E-commerce Dashboard" description="Overview of your store performance" />

    <!-- Stat Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <WxStatCard
        v-for="stat in stats"
        :key="stat.title"
        :title="stat.title"
        :value="stat.value"
        :trend="stat.trend"
        :trend-up="stat.trendUp"
      >
        <template #icon>
          <component :is="stat.icon" :size="20" />
        </template>
      </WxStatCard>
    </div>

    <!-- Recent Orders -->
    <WxCard>
      <template #header>
        <h3 class="text-base font-semibold text-on-surface">Recent Orders</h3>
      </template>
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="border-b border-outline-variant">
              <th class="text-left py-3 pr-4 font-semibold text-on-surface-variant">Order ID</th>
              <th class="text-left py-3 px-4 font-semibold text-on-surface-variant">Customer</th>
              <th class="text-right py-3 px-4 font-semibold text-on-surface-variant">Items</th>
              <th class="text-right py-3 px-4 font-semibold text-on-surface-variant">Total</th>
              <th class="text-center py-3 px-4 font-semibold text-on-surface-variant">Status</th>
              <th class="text-right py-3 pl-4 font-semibold text-on-surface-variant">Date</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="order in orders"
              :key="order.id"
              class="border-b border-outline-variant/50 hover:bg-surface-container/50 transition-colors"
            >
              <td class="py-3 pr-4 font-medium text-on-surface">{{ order.id }}</td>
              <td class="py-3 px-4 text-on-surface">{{ order.customer }}</td>
              <td class="py-3 px-4 text-right text-on-surface-variant">{{ order.items }}</td>
              <td class="py-3 px-4 text-right font-medium text-on-surface">{{ order.total }}</td>
              <td class="py-3 px-4 text-center">
                <WxBadge :variant="statusVariant[order.status]" size="sm">
                  {{ order.status }}
                </WxBadge>
              </td>
              <td class="py-3 pl-4 text-right text-on-surface-variant">{{ order.date }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </WxCard>

    <!-- Top Selling Products -->
    <div>
      <h3 class="text-base font-semibold text-on-surface mb-4">Top Selling Products</h3>
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <WxCard v-for="product in products" :key="product.name" padding="none">
          <div class="h-40 bg-gradient-to-br from-surface-container to-surface-container-high rounded-t-xl flex items-center justify-center">
            <ShoppingCart :size="40" class="text-on-surface-variant/30" />
          </div>
          <div class="p-4">
            <h4 class="text-sm font-semibold text-on-surface line-clamp-1">{{ product.name }}</h4>
            <p class="text-lg font-bold text-on-surface mt-1">{{ product.price }}</p>
            <div class="flex items-center justify-between mt-2">
              <div class="flex items-center gap-0.5">
                <Star
                  v-for="i in 5"
                  :key="i"
                  :size="14"
                  :class="i <= product.rating ? 'text-amber-400 fill-amber-400' : 'text-on-surface-variant/30'"
                />
              </div>
              <span class="text-xs text-on-surface-variant">{{ product.sold }} sold</span>
            </div>
          </div>
        </WxCard>
      </div>
    </div>
  </div>
</template>
