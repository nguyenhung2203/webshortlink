<script setup lang="ts">
import { ref, computed } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxInput from '@/components/ui/WxInput.vue'
import { Search, ChevronDown } from 'lucide-vue-next'

interface FaqItem {
  question: string
  answer: string
  category: string
}

const search = ref('')
const openIndex = ref<number | null>(null)

const faqs = ref<FaqItem[]>([
  {
    question: 'How do I set up my account?',
    answer:
      'Getting started is easy. Click the "Sign Up" button on the homepage, fill in your details including name, email, and password, then verify your email address through the confirmation link we send. Once verified, you can log in and complete your profile by adding your organization details, preferences, and payment information if applicable.',
    category: 'Account',
  },
  {
    question: 'How does billing work and what payment methods are supported?',
    answer:
      'We offer monthly and annual billing plans. Annual plans include a 20% discount. We accept all major credit cards (Visa, Mastercard, American Express), PayPal, and bank transfers for enterprise accounts. Invoices are generated automatically on the first of each month and sent to your billing email. You can update your payment method at any time from the Billing section in Settings.',
    category: 'Billing',
  },
  {
    question: 'How can I access the API and what are the rate limits?',
    answer:
      'API access is available on Pro and Enterprise plans. Navigate to Settings > API Keys to generate your credentials. Our REST API supports JSON and includes comprehensive documentation at docs.example.com/api. Rate limits are 100 requests/minute for Pro plans and 1000 requests/minute for Enterprise. All API calls require authentication via Bearer tokens in the request header.',
    category: 'API',
  },
  {
    question: 'How do I manage my team and assign roles?',
    answer:
      'Go to Settings > Team Management to invite members via email. You can assign roles including Admin, Editor, and Viewer. Admins have full access to all settings and billing. Editors can create and modify content but cannot manage billing or team settings. Viewers have read-only access. You can change roles or remove members at any time. Team size limits depend on your plan.',
    category: 'Team',
  },
  {
    question: 'What security measures are in place to protect my data?',
    answer:
      'We take security seriously. All data is encrypted at rest using AES-256 and in transit using TLS 1.3. We support two-factor authentication (2FA) via authenticator apps and SMS. Our infrastructure is hosted on SOC 2 Type II certified cloud providers. We perform regular penetration testing and security audits. Enterprise plans include SSO/SAML integration, audit logs, and custom data retention policies.',
    category: 'Security',
  },
  {
    question: 'How do I contact support and what are the response times?',
    answer:
      'You can reach our support team through multiple channels. Use the in-app chat for instant help during business hours (9am-6pm EST, Mon-Fri). Email support@example.com for non-urgent issues (24-hour response time). Enterprise customers have access to a dedicated account manager and priority support with 1-hour response SLA. Our knowledge base at help.example.com covers common questions and tutorials.',
    category: 'Support',
  },
])

const filteredFaqs = computed(() => {
  if (!search.value.trim()) return faqs.value
  const q = search.value.toLowerCase()
  return faqs.value.filter(
    (f) =>
      f.question.toLowerCase().includes(q) ||
      f.answer.toLowerCase().includes(q) ||
      f.category.toLowerCase().includes(q)
  )
})

function toggle(index: number) {
  openIndex.value = openIndex.value === index ? null : index
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Frequently Asked Questions" description="Find answers to common questions about our platform" />

    <!-- Search -->
    <div class="max-w-xl mx-auto">
      <WxInput v-model="search" type="search" placeholder="Search questions...">
        <template #prefix>
          <Search :size="16" />
        </template>
      </WxInput>
    </div>

    <!-- FAQ Accordion -->
    <div class="max-w-3xl mx-auto space-y-3">
      <WxCard
        v-for="(faq, index) in filteredFaqs"
        :key="index"
        padding="none"
      >
        <button
          type="button"
          class="flex items-start justify-between gap-4 w-full text-left px-5 py-4 cursor-pointer"
          @click="toggle(index)"
        >
          <div class="flex items-start gap-3">
            <span class="inline-block px-2 py-0.5 rounded text-[10px] font-semibold uppercase tracking-wider bg-primary/10 text-primary shrink-0 mt-0.5">
              {{ faq.category }}
            </span>
            <h3 class="text-sm font-semibold text-on-surface">{{ faq.question }}</h3>
          </div>
          <ChevronDown
            :size="18"
            class="shrink-0 text-on-surface-variant transition-transform duration-200 mt-0.5"
            :class="openIndex === index && 'rotate-180'"
          />
        </button>
        <div
          v-if="openIndex === index"
          class="px-5 pb-4 pt-0"
        >
          <div class="pl-[calc(theme(spacing.2)*2+theme(spacing.3)+2ch)] text-sm text-on-surface-variant leading-relaxed">
            {{ faq.answer }}
          </div>
        </div>
      </WxCard>

      <!-- Empty state -->
      <div v-if="!filteredFaqs.length" class="text-center py-12">
        <Search :size="40" class="mx-auto text-on-surface-variant/30 mb-3" />
        <p class="text-sm text-on-surface-variant">No questions found matching "{{ search }}"</p>
        <p class="text-xs text-on-surface-variant mt-1">Try a different search term or browse all questions.</p>
      </div>
    </div>
  </div>
</template>
