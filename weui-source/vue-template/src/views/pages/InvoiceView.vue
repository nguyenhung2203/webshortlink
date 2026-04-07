<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { Printer, CreditCard } from 'lucide-vue-next'

interface InvoiceItem {
  description: string
  quantity: number
  unitPrice: number
  total: number
}

const invoiceDate = ref('March 15, 2026')
const dueDate = ref('April 15, 2026')

const items = ref<InvoiceItem[]>([
  { description: 'Website Design & Development', quantity: 1, unitPrice: 3500, total: 3500 },
  { description: 'UI/UX Consultation (hours)', quantity: 12, unitPrice: 150, total: 1800 },
  { description: 'Hosting & Domain (annual)', quantity: 1, unitPrice: 240, total: 240 },
  { description: 'Maintenance Package (monthly)', quantity: 3, unitPrice: 200, total: 600 },
])

const subtotal = ref(6140)
const taxRate = 0.1
const tax = ref(subtotal.value * taxRate)
const total = ref(subtotal.value + tax.value)

function formatCurrency(value: number): string {
  return `$${value.toLocaleString('en-US', { minimumFractionDigits: 2 })}`
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Invoice" description="Manage and view invoice details">
      <template #actions>
        <WxButton variant="secondary" size="sm">
          <template #icon><Printer :size="16" /></template>
          Print
        </WxButton>
        <WxButton size="sm">
          <template #icon><CreditCard :size="16" /></template>
          Pay Now
        </WxButton>
      </template>
    </WxPageHeader>

    <WxCard>
      <!-- Invoice Header -->
      <div class="flex flex-col sm:flex-row justify-between gap-4 mb-8">
        <div>
          <h2 class="text-xl font-bold text-on-surface">Invoice #007</h2>
          <p class="text-sm text-on-surface-variant mt-1">Issued: {{ invoiceDate }}</p>
        </div>
        <div class="text-left sm:text-right">
          <span class="inline-block px-3 py-1 rounded-full text-xs font-semibold bg-warning/15 text-warning">
            Unpaid
          </span>
          <p class="text-sm text-on-surface-variant mt-2">Due: {{ dueDate }}</p>
        </div>
      </div>

      <!-- From / To -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-6 mb-8 pb-8 border-b border-outline-variant">
        <div>
          <h3 class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider mb-2">From</h3>
          <p class="text-sm font-semibold text-on-surface">Acme Studios Inc.</p>
          <p class="text-sm text-on-surface-variant mt-1">
            123 Creative Ave, Suite 400<br />
            San Francisco, CA 94102<br />
            billing@acmestudios.com
          </p>
        </div>
        <div>
          <h3 class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider mb-2">Bill To</h3>
          <p class="text-sm font-semibold text-on-surface">Globex Corporation</p>
          <p class="text-sm text-on-surface-variant mt-1">
            456 Enterprise Blvd<br />
            New York, NY 10001<br />
            accounts@globexcorp.com
          </p>
        </div>
      </div>

      <!-- Items Table -->
      <div class="overflow-x-auto mb-8">
        <table class="w-full text-sm">
          <thead>
            <tr class="border-b border-outline-variant">
              <th class="text-left py-3 pr-4 font-semibold text-on-surface-variant">Description</th>
              <th class="text-right py-3 px-4 font-semibold text-on-surface-variant">Qty</th>
              <th class="text-right py-3 px-4 font-semibold text-on-surface-variant">Unit Price</th>
              <th class="text-right py-3 pl-4 font-semibold text-on-surface-variant">Total</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(item, index) in items"
              :key="index"
              class="border-b border-outline-variant/50"
            >
              <td class="py-3 pr-4 text-on-surface">{{ item.description }}</td>
              <td class="py-3 px-4 text-right text-on-surface-variant">{{ item.quantity }}</td>
              <td class="py-3 px-4 text-right text-on-surface-variant">{{ formatCurrency(item.unitPrice) }}</td>
              <td class="py-3 pl-4 text-right font-medium text-on-surface">{{ formatCurrency(item.total) }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Totals -->
      <div class="flex justify-end mb-8">
        <div class="w-full max-w-xs space-y-2">
          <div class="flex justify-between text-sm">
            <span class="text-on-surface-variant">Subtotal</span>
            <span class="text-on-surface font-medium">{{ formatCurrency(subtotal) }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-on-surface-variant">Tax (10%)</span>
            <span class="text-on-surface font-medium">{{ formatCurrency(tax) }}</span>
          </div>
          <div class="flex justify-between text-base pt-2 border-t border-outline-variant">
            <span class="font-bold text-on-surface">Total</span>
            <span class="font-bold text-on-surface">{{ formatCurrency(total) }}</span>
          </div>
        </div>
      </div>

      <!-- Payment Info -->
      <div class="bg-surface-container rounded-lg p-4">
        <h3 class="text-sm font-semibold text-on-surface mb-2">Payment Information</h3>
        <div class="grid grid-cols-1 sm:grid-cols-3 gap-3 text-sm text-on-surface-variant">
          <div>
            <span class="font-medium text-on-surface">Bank:</span> First National Bank
          </div>
          <div>
            <span class="font-medium text-on-surface">Account:</span> XXXX-XXXX-4821
          </div>
          <div>
            <span class="font-medium text-on-surface">Routing:</span> 021000021
          </div>
        </div>
        <p class="text-xs text-on-surface-variant mt-3">
          Please include invoice number #007 as reference when making the payment.
        </p>
      </div>
    </WxCard>

    <!-- Bottom Actions -->
    <div class="flex justify-end gap-3">
      <WxButton variant="secondary">
        <template #icon><Printer :size="16" /></template>
        Print Invoice
      </WxButton>
      <WxButton>
        <template #icon><CreditCard :size="16" /></template>
        Pay Now
      </WxButton>
    </div>
  </div>
</template>
