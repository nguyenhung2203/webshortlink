<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxAvatar from '@/components/ui/WxAvatar.vue'
import WxTabs from '@/components/ui/WxTabs.vue'
import WxTimeline from '@/components/ui/WxTimeline.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxTextarea from '@/components/ui/WxTextarea.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { MapPin, Mail, Phone, Home, Cake, Building2 } from 'lucide-vue-next'

const activeTab = ref('about')
const tabs = [
  { key: 'about', label: 'About' },
  { key: 'timeline', label: 'Timeline' },
  { key: 'settings', label: 'Settings' },
]

const stats = [
  { label: 'Posts', value: '142' },
  { label: 'Following', value: '456' },
  { label: 'Followers', value: '2.1K' },
]

const infoItems = [
  { icon: Mail, label: 'Email', value: 'sarah.chen@example.com' },
  { icon: Phone, label: 'Phone', value: '+1 (555) 234-5678' },
  { icon: Home, label: 'Address', value: '123 Maple Street, Portland, OR 97201' },
  { icon: Cake, label: 'Birthday', value: 'June 15, 1992' },
  { icon: Building2, label: 'Company', value: 'Acme Studios Inc.' },
]

const timelineItems = [
  { title: 'Completed project "Website Redesign"', description: 'Delivered final assets and documentation to the client.', time: '2 hours ago', color: 'success' },
  { title: 'Added 3 new team members', description: 'Onboarded designers and a frontend developer.', time: '1 day ago', color: 'primary' },
  { title: 'Published a blog post', description: '"Top 10 UI Design Trends for 2026"', time: '3 days ago', color: 'info' },
  { title: 'Received a 5-star review', description: 'Client praised the mobile app interface work.', time: '1 week ago', color: 'warning' },
  { title: 'Joined the Design Community', description: 'Became a member of the UX Professionals network.', time: '2 weeks ago', color: 'primary' },
]

const settingsForm = ref({
  name: 'Sarah Chen',
  email: 'sarah.chen@example.com',
  bio: 'Senior UI/UX designer with 8+ years of experience creating user-centered digital products. Passionate about accessibility and design systems.',
})
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Profile" description="View and manage your profile information" />

    <!-- Cover + Avatar -->
    <WxCard padding="none">
      <div class="h-48 bg-gradient-to-r from-cyan-500 via-blue-500 to-violet-500 rounded-t-xl" />
      <div class="px-5 pb-5">
        <div class="flex flex-col sm:flex-row sm:items-end gap-4 -mt-10">
          <div class="ring-4 ring-surface-container-lowest rounded-full">
            <WxAvatar name="Sarah Chen" size="xl" />
          </div>
          <div class="flex-1 sm:pb-1">
            <h2 class="text-xl font-bold text-on-surface">Sarah Chen</h2>
            <p class="text-sm text-on-surface-variant">Senior UI/UX Designer</p>
            <div class="flex items-center gap-1 text-xs text-on-surface-variant mt-1">
              <MapPin :size="12" />
              <span>Portland, Oregon</span>
            </div>
          </div>
          <div class="flex gap-6 sm:gap-8 pb-1">
            <div v-for="stat in stats" :key="stat.label" class="text-center">
              <p class="text-lg font-bold text-on-surface">{{ stat.value }}</p>
              <p class="text-xs text-on-surface-variant">{{ stat.label }}</p>
            </div>
          </div>
        </div>
      </div>
    </WxCard>

    <!-- Tabs -->
    <WxCard>
      <WxTabs v-model="activeTab" :items="tabs">
        <template #default="{ activeKey }">
          <!-- About Tab -->
          <div v-if="activeKey === 'about'" class="space-y-6">
            <div>
              <h3 class="text-sm font-semibold text-on-surface mb-2">Bio</h3>
              <p class="text-sm text-on-surface-variant leading-relaxed">
                Senior UI/UX designer with 8+ years of experience creating user-centered digital products.
                Passionate about accessibility, design systems, and bridging the gap between design and engineering.
                Previously worked at Google and Figma. Currently leading the design team at Acme Studios.
              </p>
            </div>
            <div>
              <h3 class="text-sm font-semibold text-on-surface mb-3">Information</h3>
              <div class="space-y-3">
                <div
                  v-for="item in infoItems"
                  :key="item.label"
                  class="flex items-center gap-3"
                >
                  <component :is="item.icon" :size="16" class="text-on-surface-variant shrink-0" />
                  <div class="flex gap-2 text-sm">
                    <span class="text-on-surface-variant min-w-[70px]">{{ item.label }}</span>
                    <span class="text-on-surface">{{ item.value }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Timeline Tab -->
          <div v-if="activeKey === 'timeline'">
            <WxTimeline :items="timelineItems" />
          </div>

          <!-- Settings Tab -->
          <div v-if="activeKey === 'settings'" class="space-y-4 max-w-lg">
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1">Full Name</label>
              <WxInput v-model="settingsForm.name" placeholder="Your name" />
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1">Email</label>
              <WxInput v-model="settingsForm.email" type="email" placeholder="Your email" />
            </div>
            <div>
              <label class="block text-sm font-medium text-on-surface mb-1">Bio</label>
              <WxTextarea v-model="settingsForm.bio" placeholder="Write a short bio..." :rows="4" />
            </div>
            <div class="pt-2">
              <WxButton>Save Changes</WxButton>
            </div>
          </div>
        </template>
      </WxTabs>
    </WxCard>
  </div>
</template>
