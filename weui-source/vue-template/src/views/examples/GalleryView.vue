<script setup lang="ts">
import { ref, computed } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxModal from '@/components/ui/WxModal.vue'
import { Eye, X } from 'lucide-vue-next'

type Category = 'all' | 'nature' | 'architecture' | 'people'

interface GalleryItem {
  id: number
  title: string
  category: Category
  gradient: string
  author: string
}

const activeFilter = ref<Category>('all')
const modalOpen = ref(false)
const selectedItem = ref<GalleryItem | null>(null)

const filters: { label: string; value: Category }[] = [
  { label: 'All', value: 'all' },
  { label: 'Nature', value: 'nature' },
  { label: 'Architecture', value: 'architecture' },
  { label: 'People', value: 'people' },
]

const items = ref<GalleryItem[]>([
  { id: 1, title: 'Mountain Sunrise', category: 'nature', gradient: 'from-orange-400 to-rose-500', author: 'Nguyen Van An' },
  { id: 2, title: 'Modern Tower', category: 'architecture', gradient: 'from-slate-500 to-zinc-700', author: 'Tran Minh Duc' },
  { id: 3, title: 'Street Portrait', category: 'people', gradient: 'from-amber-400 to-orange-600', author: 'Le Thi Hoa' },
  { id: 4, title: 'Forest Path', category: 'nature', gradient: 'from-emerald-400 to-teal-600', author: 'Pham Quoc Bao' },
  { id: 5, title: 'Glass Facade', category: 'architecture', gradient: 'from-cyan-400 to-blue-600', author: 'Vo Thanh Tam' },
  { id: 6, title: 'Festival Crowd', category: 'people', gradient: 'from-pink-400 to-fuchsia-600', author: 'Hoang Mai Anh' },
  { id: 7, title: 'Ocean Waves', category: 'nature', gradient: 'from-blue-400 to-indigo-600', author: 'Do Minh Khoi' },
  { id: 8, title: 'Ancient Temple', category: 'architecture', gradient: 'from-yellow-500 to-amber-700', author: 'Bui Ngoc Lan' },
  { id: 9, title: 'Candid Smile', category: 'people', gradient: 'from-rose-400 to-red-600', author: 'Dang Huu Phuc' },
  { id: 10, title: 'Lavender Field', category: 'nature', gradient: 'from-violet-400 to-purple-600', author: 'Ngo Thu Hang' },
  { id: 11, title: 'City Skyline', category: 'architecture', gradient: 'from-gray-400 to-slate-600', author: 'Ly Van Hieu' },
  { id: 12, title: 'Market Vendor', category: 'people', gradient: 'from-lime-400 to-green-600', author: 'Truong Bich Ngoc' },
])

const filteredItems = computed(() =>
  activeFilter.value === 'all'
    ? items.value
    : items.value.filter(i => i.category === activeFilter.value)
)

function openModal(item: GalleryItem) {
  selectedItem.value = item
  modalOpen.value = true
}
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Gallery" description="Browse our curated collection of photography and artwork.">
      <template #actions>
        <span class="text-sm text-on-surface-variant">{{ filteredItems.length }} items</span>
      </template>
    </WxPageHeader>

    <!-- Filter Buttons -->
    <div class="flex flex-wrap gap-2">
      <WxButton
        v-for="f in filters"
        :key="f.value"
        :variant="activeFilter === f.value ? 'primary' : 'secondary'"
        size="sm"
        @click="activeFilter = f.value"
      >
        {{ f.label }}
      </WxButton>
    </div>

    <!-- Image Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
      <div
        v-for="item in filteredItems"
        :key="item.id"
        class="group cursor-pointer"
        @click="openModal(item)"
      >
        <div class="relative overflow-hidden rounded-xl border border-outline-variant">
          <!-- Gradient placeholder as image -->
          <div
            class="aspect-[4/3] bg-gradient-to-br w-full"
            :class="item.gradient"
          />
          <!-- Hover overlay -->
          <div class="absolute inset-0 bg-black/0 group-hover:bg-black/40 transition-all duration-300 flex items-center justify-center">
            <div class="opacity-0 group-hover:opacity-100 transition-opacity duration-300 bg-white/20 backdrop-blur-sm rounded-full p-3">
              <Eye class="w-6 h-6 text-white" />
            </div>
          </div>
        </div>
        <div class="mt-2.5 px-0.5">
          <p class="text-sm font-medium text-on-surface">{{ item.title }}</p>
          <p class="text-xs text-on-surface-variant mt-0.5">by {{ item.author }}</p>
        </div>
      </div>
    </div>

    <!-- Detail Modal -->
    <WxModal v-model="modalOpen" :title="selectedItem?.title ?? 'Image'" size="lg">
      <div v-if="selectedItem" class="space-y-4">
        <div
          class="aspect-[16/10] rounded-lg bg-gradient-to-br w-full"
          :class="selectedItem.gradient"
        />
        <div class="flex items-center justify-between">
          <div>
            <p class="text-base font-semibold text-on-surface">{{ selectedItem.title }}</p>
            <p class="text-sm text-on-surface-variant">by {{ selectedItem.author }}</p>
          </div>
          <span
            class="inline-block px-2.5 py-1 text-xs font-medium rounded-full bg-surface-container-high text-on-surface capitalize"
          >
            {{ selectedItem.category }}
          </span>
        </div>
      </div>
      <template #footer>
        <WxButton variant="secondary" size="sm" @click="modalOpen = false">Close</WxButton>
      </template>
    </WxModal>
  </div>
</template>
