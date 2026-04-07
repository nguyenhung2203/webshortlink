<script setup lang="ts">
import { ref, computed } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { ChevronLeft, ChevronRight, Clock, MapPin } from 'lucide-vue-next'

const currentYear = ref(2026)
const currentMonth = ref(2) // 0-indexed, March = 2

const monthNames = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']
const dayNames = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']

interface CalendarEvent {
  day: number
  title: string
  time: string
  color: string
  dotClass: string
  location?: string
}

const events = ref<CalendarEvent[]>([
  { day: 5, title: 'Product Strategy Meeting', time: '09:00 - 10:30', color: 'blue', dotClass: 'bg-blue-500', location: 'Room A3' },
  { day: 12, title: 'Sprint Review & Demo', time: '14:00 - 15:00', color: 'green', dotClass: 'bg-emerald-500', location: 'Online' },
  { day: 18, title: 'Client Presentation Deadline', time: '11:00 - 12:00', color: 'red', dotClass: 'bg-red-500', location: 'HQ Office' },
  { day: 25, title: 'Team Building Workshop', time: '13:00 - 17:00', color: 'amber', dotClass: 'bg-amber-500', location: 'Park Garden' },
])

const eventsByDay = computed(() => {
  const map = new Map<number, CalendarEvent[]>()
  events.value.forEach(e => {
    if (!map.has(e.day)) map.set(e.day, [])
    map.get(e.day)!.push(e)
  })
  return map
})

const calendarDays = computed(() => {
  const year = currentYear.value
  const month = currentMonth.value
  const firstDay = new Date(year, month, 1)
  const lastDay = new Date(year, month + 1, 0)
  const daysInMonth = lastDay.getDate()

  // Monday=0 ... Sunday=6
  let startDow = firstDay.getDay() - 1
  if (startDow < 0) startDow = 6

  const prevMonthLast = new Date(year, month, 0).getDate()

  const cells: { day: number; current: boolean }[] = []

  for (let i = startDow - 1; i >= 0; i--) {
    cells.push({ day: prevMonthLast - i, current: false })
  }
  for (let d = 1; d <= daysInMonth; d++) {
    cells.push({ day: d, current: true })
  }
  const remaining = 42 - cells.length
  for (let d = 1; d <= remaining; d++) {
    cells.push({ day: d, current: false })
  }
  return cells
})

const displayMonth = computed(() => `${monthNames[currentMonth.value]} ${currentYear.value}`)

function prevMonth() {
  if (currentMonth.value === 0) {
    currentMonth.value = 11
    currentYear.value--
  } else {
    currentMonth.value--
  }
}

function nextMonth() {
  if (currentMonth.value === 11) {
    currentMonth.value = 0
    currentYear.value++
  } else {
    currentMonth.value++
  }
}

const today = 27

const upcomingEvents = computed(() =>
  [...events.value].sort((a, b) => a.day - b.day)
)
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Calendar" description="View and manage your schedule and upcoming events." />

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Calendar Grid -->
      <div class="lg:col-span-2">
        <WxCard>
          <!-- Month Navigation -->
          <div class="flex items-center justify-between mb-6">
            <WxButton variant="ghost" size="sm" @click="prevMonth">
              <ChevronLeft :size="18" />
            </WxButton>
            <h2 class="text-lg font-semibold text-on-surface">{{ displayMonth }}</h2>
            <WxButton variant="ghost" size="sm" @click="nextMonth">
              <ChevronRight :size="18" />
            </WxButton>
          </div>

          <!-- Day Headers -->
          <div class="grid grid-cols-7 gap-1 mb-2">
            <div
              v-for="name in dayNames"
              :key="name"
              class="text-center text-xs font-medium text-on-surface-variant py-2"
            >
              {{ name }}
            </div>
          </div>

          <!-- Day Cells -->
          <div class="grid grid-cols-7 gap-1">
            <div
              v-for="(cell, idx) in calendarDays"
              :key="idx"
              class="relative aspect-square flex flex-col items-center justify-center rounded-lg text-sm transition-colors cursor-pointer"
              :class="[
                cell.current ? 'hover:bg-surface-container-high' : 'text-on-surface-variant/40',
                cell.current && cell.day === today ? 'bg-primary/10 text-primary font-bold ring-1 ring-primary/30' : '',
              ]"
            >
              <span>{{ cell.day }}</span>
              <div v-if="cell.current && eventsByDay.has(cell.day)" class="flex gap-0.5 mt-0.5">
                <span
                  v-for="ev in eventsByDay.get(cell.day)"
                  :key="ev.title"
                  class="w-1.5 h-1.5 rounded-full"
                  :class="ev.dotClass"
                />
              </div>
            </div>
          </div>
        </WxCard>
      </div>

      <!-- Upcoming Events Sidebar -->
      <div>
        <WxCard>
          <template #header>
            <h3 class="text-base font-semibold text-on-surface">Upcoming Events</h3>
          </template>
          <div class="space-y-4">
            <div
              v-for="event in upcomingEvents"
              :key="event.title"
              class="flex gap-3 p-3 rounded-lg hover:bg-surface-container-high/50 transition-colors cursor-pointer"
            >
              <div class="w-1 rounded-full shrink-0" :class="event.dotClass" />
              <div class="min-w-0 flex-1">
                <p class="text-sm font-medium text-on-surface truncate">{{ event.title }}</p>
                <div class="flex items-center gap-1 mt-1 text-xs text-on-surface-variant">
                  <Clock :size="12" />
                  <span>Mar {{ event.day }}, {{ event.time }}</span>
                </div>
                <div v-if="event.location" class="flex items-center gap-1 mt-0.5 text-xs text-on-surface-variant">
                  <MapPin :size="12" />
                  <span>{{ event.location }}</span>
                </div>
              </div>
            </div>
          </div>
        </WxCard>
      </div>
    </div>
  </div>
</template>
