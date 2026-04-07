<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxStatCard from '@/components/ui/WxStatCard.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxBadge from '@/components/ui/WxBadge.vue'
import WxProgressBar from '@/components/ui/WxProgressBar.vue'
import { DollarSign, ShoppingCart, Package, Users } from 'lucide-vue-next'

const recentOrders = ref([
  { id: '#ORD-7841', customer: 'Sarah Chen', product: 'Wireless Headphones', amount: '$129.99', status: 'success' as const, statusLabel: 'Delivered' },
  { id: '#ORD-7840', customer: 'James Wilson', product: 'Mechanical Keyboard', amount: '$89.50', status: 'info' as const, statusLabel: 'Shipped' },
  { id: '#ORD-7839', customer: 'Maria Garcia', product: 'USB-C Hub Pro', amount: '$54.00', status: 'warning' as const, statusLabel: 'Processing' },
  { id: '#ORD-7838', customer: 'David Park', product: '4K Monitor 27"', amount: '$449.99', status: 'success' as const, statusLabel: 'Delivered' },
  { id: '#ORD-7837', customer: 'Emily Brown', product: 'Laptop Stand', amount: '$35.00', status: 'error' as const, statusLabel: 'Cancelled' },
])

const topProducts = ref([
  { name: 'Wireless Headphones', sales: 342, revenue: '$44,358' },
  { name: '4K Monitor 27"', sales: 156, revenue: '$70,199' },
  { name: 'Mechanical Keyboard', sales: 289, revenue: '$25,826' },
  { name: 'USB-C Hub Pro', sales: 478, revenue: '$25,812' },
])

const regions = ref([
  { name: 'North America', value: 78, variant: 'primary' as const },
  { name: 'Europe', value: 62, variant: 'success' as const },
  { name: 'Asia Pacific', value: 45, variant: 'warning' as const },
  { name: 'Latin America', value: 28, variant: 'danger' as const },
])
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="E-commerce Dashboard" description="Sales performance, orders, and product insights." />

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <WxStatCard title="Revenue" value="$24,500" :trend="18.2" :trend-up="true">
        <template #icon><DollarSign :size="20" /></template>
      </WxStatCard>
      <WxStatCard title="Orders" value="156" :trend="5.4" :trend-up="true">
        <template #icon><ShoppingCart :size="20" /></template>
      </WxStatCard>
      <WxStatCard title="Products" value="89" :trend="2.1" :trend-up="true">
        <template #icon><Package :size="20" /></template>
      </WxStatCard>
      <WxStatCard title="Customers" value="1,240" :trend="11.3" :trend-up="true">
        <template #icon><Users :size="20" /></template>
      </WxStatCard>
    </div>

    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Recent Orders</h3>
      </template>
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="border-b border-outline-variant text-left">
              <th class="pb-3 font-medium text-on-surface-variant">Order ID</th>
              <th class="pb-3 font-medium text-on-surface-variant">Customer</th>
              <th class="pb-3 font-medium text-on-surface-variant">Product</th>
              <th class="pb-3 font-medium text-on-surface-variant text-right">Amount</th>
              <th class="pb-3 font-medium text-on-surface-variant text-right">Status</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-outline-variant">
            <tr v-for="order in recentOrders" :key="order.id">
              <td class="py-3 font-mono text-on-surface">{{ order.id }}</td>
              <td class="py-3 text-on-surface">{{ order.customer }}</td>
              <td class="py-3 text-on-surface-variant">{{ order.product }}</td>
              <td class="py-3 text-on-surface font-medium text-right">{{ order.amount }}</td>
              <td class="py-3 text-right">
                <WxBadge :variant="order.status" size="sm">{{ order.statusLabel }}</WxBadge>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </WxCard>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <WxCard>
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">Top Products</h3>
        </template>
        <div class="divide-y divide-outline-variant">
          <div v-for="(product, i) in topProducts" :key="product.name" class="flex items-center gap-4 py-3 first:pt-0 last:pb-0">
            <div class="w-8 h-8 rounded-lg bg-primary/10 flex items-center justify-center text-sm font-bold text-primary shrink-0">
              {{ i + 1 }}
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-on-surface truncate">{{ product.name }}</p>
              <p class="text-xs text-on-surface-variant">{{ product.sales }} sold</p>
            </div>
            <span class="text-sm font-semibold text-on-surface shrink-0">{{ product.revenue }}</span>
          </div>
        </div>
      </WxCard>

      <WxCard>
        <template #header>
          <h3 class="text-sm font-semibold text-on-surface">Sales by Region</h3>
        </template>
        <div class="space-y-5">
          <div v-for="region in regions" :key="region.name" class="space-y-2">
            <div class="flex items-center justify-between text-sm">
              <span class="font-medium text-on-surface">{{ region.name }}</span>
              <span class="text-on-surface-variant">{{ region.value }}%</span>
            </div>
            <WxProgressBar :value="region.value" :variant="region.variant" size="sm" />
          </div>
        </div>
      </WxCard>
    </div>
  </div>
</template>
