<script setup lang="ts">
import { ref, computed } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { Search, Plus, Mail, Phone, Pencil, Trash2, MessageCircle } from 'lucide-vue-next'

interface Contact {
  name: string
  role: string
  company: string
  email: string
  phone: string
}

const search = ref('')

const contacts = ref<Contact[]>([
  { name: 'Alice Wang', role: 'Product Designer', company: 'Acme Studios', email: 'alice@acme.com', phone: '+1 (555) 100-2001' },
  { name: 'Bob Smith', role: 'Frontend Developer', company: 'TechFlow Inc.', email: 'bob@techflow.io', phone: '+1 (555) 100-2002' },
  { name: 'Carol Davis', role: 'Project Manager', company: 'Globex Corp.', email: 'carol@globex.com', phone: '+1 (555) 100-2003' },
  { name: 'David Kim', role: 'Backend Engineer', company: 'Initech', email: 'david@initech.io', phone: '+1 (555) 100-2004' },
  { name: 'Eva Martinez', role: 'Marketing Lead', company: 'BrightSpark', email: 'eva@brightspark.co', phone: '+1 (555) 100-2005' },
  { name: 'Frank Lee', role: 'DevOps Engineer', company: 'CloudNine', email: 'frank@cloudnine.dev', phone: '+1 (555) 100-2006' },
  { name: 'Grace Chen', role: 'UX Researcher', company: 'Acme Studios', email: 'grace@acme.com', phone: '+1 (555) 100-2007' },
  { name: 'Henry Park', role: 'Data Analyst', company: 'DataVista', email: 'henry@datavista.ai', phone: '+1 (555) 100-2008' },
])

const filteredContacts = computed(() => {
  if (!search.value.trim()) return contacts.value
  const q = search.value.toLowerCase()
  return contacts.value.filter(
    (c) =>
      c.name.toLowerCase().includes(q) ||
      c.role.toLowerCase().includes(q) ||
      c.company.toLowerCase().includes(q) ||
      c.email.toLowerCase().includes(q)
  )
})
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Contacts" description="Manage your contact list">
      <template #actions>
        <WxButton size="sm">
          <template #icon><Plus :size="16" /></template>
          Add Contact
        </WxButton>
      </template>
    </WxPageHeader>

    <!-- Search -->
    <WxInput v-model="search" type="search" placeholder="Search contacts by name, role, company, or email...">
      <template #prefix>
        <Search :size="16" />
      </template>
    </WxInput>

    <!-- Contacts Grid -->
    <div v-if="filteredContacts.length" class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <WxCard v-for="contact in filteredContacts" :key="contact.email">
        <div class="flex gap-4">
          <WxAvatar :name="contact.name" size="lg" />
          <div class="flex-1 min-w-0">
            <h3 class="text-base font-semibold text-on-surface">{{ contact.name }}</h3>
            <p class="text-sm text-on-surface-variant">{{ contact.role }}</p>
            <p class="text-xs text-on-surface-variant mt-0.5">{{ contact.company }}</p>

            <div class="mt-3 space-y-1.5">
              <div class="flex items-center gap-2 text-sm text-on-surface-variant">
                <Mail :size="14" class="shrink-0" />
                <span class="truncate">{{ contact.email }}</span>
              </div>
              <div class="flex items-center gap-2 text-sm text-on-surface-variant">
                <Phone :size="14" class="shrink-0" />
                <span>{{ contact.phone }}</span>
              </div>
            </div>

            <div class="flex items-center gap-2 mt-4 pt-3 border-t border-outline-variant">
              <WxButton variant="ghost" size="sm">
                <template #icon><MessageCircle :size="14" /></template>
                Message
              </WxButton>
              <WxButton variant="ghost" size="sm">
                <template #icon><Pencil :size="14" /></template>
                Edit
              </WxButton>
              <WxButton variant="ghost" size="sm">
                <template #icon><Trash2 :size="14" class="text-danger" /></template>
              </WxButton>
            </div>
          </div>
        </div>
      </WxCard>
    </div>

    <!-- Empty state -->
    <div v-else class="text-center py-12">
      <Search :size="40" class="mx-auto text-on-surface-variant/30 mb-3" />
      <p class="text-sm text-on-surface-variant">No contacts found matching "{{ search }}"</p>
    </div>
  </div>
</template>
