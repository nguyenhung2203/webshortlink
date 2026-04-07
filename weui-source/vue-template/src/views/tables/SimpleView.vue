<script setup lang="ts">
import { Table, StretchHorizontal, Grid3x3, MousePointerClick, Smartphone, Palette } from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'

// Shared employee data
const employees = [
  { name: 'Alice Nguyen', position: 'Software Engineer', office: 'Ho Chi Minh', age: 28, startDate: '2022-03-15', salary: '$85,000' },
  { name: 'Bob Tran', position: 'Product Manager', office: 'Ha Noi', age: 34, startDate: '2020-07-01', salary: '$95,000' },
  { name: 'Charlie Pham', position: 'UX Designer', office: 'Da Nang', age: 26, startDate: '2023-01-10', salary: '$72,000' },
  { name: 'Diana Le', position: 'Data Analyst', office: 'Ho Chi Minh', age: 31, startDate: '2021-09-20', salary: '$78,000' },
  { name: 'Edward Vo', position: 'DevOps Engineer', office: 'Ha Noi', age: 29, startDate: '2022-11-05', salary: '$90,000' },
]

const headers = ['Name', 'Position', 'Office', 'Age', 'Start Date', 'Salary']
</script>

<template>
  <div class="space-y-10">
    <WxPageHeader
      title="Simple Tables"
      description="Basic HTML tables styled with Tailwind CSS. Demonstrates various table styles including striped, bordered, hover, responsive, and colored variants."
    />

    <!-- ==================== Basic Table ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Table class="w-5 h-5" />
        <span>Basic Table</span>
      </div>
      <WxCard>
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="border-b border-outline-variant">
              <th v-for="h in headers" :key="h" class="py-3 px-4 font-semibold text-on-surface-variant text-xs uppercase tracking-wider">
                {{ h }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="emp in employees" :key="emp.name" class="border-b border-outline-variant last:border-0">
              <td class="py-3 px-4 font-medium text-on-surface">{{ emp.name }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.position }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.office }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.age }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.startDate }}</td>
              <td class="py-3 px-4 font-medium text-on-surface tabular-nums">{{ emp.salary }}</td>
            </tr>
          </tbody>
        </table>
      </WxCard>
    </section>

    <!-- ==================== Striped Table ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <StretchHorizontal class="w-5 h-5" />
        <span>Striped Table</span>
      </div>
      <WxCard>
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="border-b border-outline-variant">
              <th v-for="h in headers" :key="h" class="py-3 px-4 font-semibold text-on-surface-variant text-xs uppercase tracking-wider">
                {{ h }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(emp, idx) in employees"
              :key="emp.name"
              :class="idx % 2 === 0 ? 'bg-surface-container-lowest' : 'bg-surface-container'"
            >
              <td class="py-3 px-4 font-medium text-on-surface">{{ emp.name }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.position }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.office }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.age }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.startDate }}</td>
              <td class="py-3 px-4 font-medium text-on-surface tabular-nums">{{ emp.salary }}</td>
            </tr>
          </tbody>
        </table>
      </WxCard>
    </section>

    <!-- ==================== Bordered Table ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Grid3x3 class="w-5 h-5" />
        <span>Bordered Table</span>
      </div>
      <WxCard>
        <table class="w-full text-sm text-left border border-outline-variant">
          <thead>
            <tr class="bg-surface-container">
              <th v-for="h in headers" :key="h" class="py-3 px-4 font-semibold text-on-surface-variant text-xs uppercase tracking-wider border border-outline-variant">
                {{ h }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="emp in employees" :key="emp.name">
              <td class="py-3 px-4 font-medium text-on-surface border border-outline-variant">{{ emp.name }}</td>
              <td class="py-3 px-4 text-on-surface-variant border border-outline-variant">{{ emp.position }}</td>
              <td class="py-3 px-4 text-on-surface-variant border border-outline-variant">{{ emp.office }}</td>
              <td class="py-3 px-4 text-on-surface-variant border border-outline-variant">{{ emp.age }}</td>
              <td class="py-3 px-4 text-on-surface-variant border border-outline-variant">{{ emp.startDate }}</td>
              <td class="py-3 px-4 font-medium text-on-surface tabular-nums border border-outline-variant">{{ emp.salary }}</td>
            </tr>
          </tbody>
        </table>
      </WxCard>
    </section>

    <!-- ==================== Hover Table ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <MousePointerClick class="w-5 h-5" />
        <span>Hover Table</span>
      </div>
      <WxCard>
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="border-b-2 border-outline-variant">
              <th v-for="h in headers" :key="h" class="py-3 px-4 font-semibold text-on-surface-variant text-xs uppercase tracking-wider">
                {{ h }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="emp in employees"
              :key="emp.name"
              class="border-b border-outline-variant last:border-0 transition-colors duration-150 hover:bg-primary/5 cursor-pointer"
            >
              <td class="py-3 px-4 font-medium text-on-surface">{{ emp.name }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.position }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.office }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.age }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.startDate }}</td>
              <td class="py-3 px-4 font-medium text-on-surface tabular-nums">{{ emp.salary }}</td>
            </tr>
          </tbody>
        </table>
      </WxCard>
    </section>

    <!-- ==================== Responsive Table ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Smartphone class="w-5 h-5" />
        <span>Responsive Table</span>
      </div>
      <WxCard>
        <p class="text-xs text-on-surface-variant mb-3">Resize the browser to see horizontal scrolling on smaller viewports.</p>
        <div class="overflow-x-auto -mx-4 sm:mx-0">
          <div class="inline-block min-w-full align-middle">
            <table class="min-w-full text-sm text-left">
              <thead>
                <tr class="border-b border-outline-variant bg-surface-container">
                  <th v-for="h in headers" :key="h" class="py-3 px-4 font-semibold text-on-surface-variant text-xs uppercase tracking-wider whitespace-nowrap">
                    {{ h }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="emp in employees" :key="emp.name" class="border-b border-outline-variant last:border-0">
                  <td class="py-3 px-4 font-medium text-on-surface whitespace-nowrap">{{ emp.name }}</td>
                  <td class="py-3 px-4 text-on-surface-variant whitespace-nowrap">{{ emp.position }}</td>
                  <td class="py-3 px-4 text-on-surface-variant whitespace-nowrap">{{ emp.office }}</td>
                  <td class="py-3 px-4 text-on-surface-variant whitespace-nowrap">{{ emp.age }}</td>
                  <td class="py-3 px-4 text-on-surface-variant whitespace-nowrap">{{ emp.startDate }}</td>
                  <td class="py-3 px-4 font-medium text-on-surface tabular-nums whitespace-nowrap">{{ emp.salary }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </WxCard>
    </section>

    <!-- ==================== Colored Tables ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Palette class="w-5 h-5" />
        <span>Colored Header Tables</span>
      </div>

      <!-- Primary header -->
      <WxCard>
        <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant mb-3">Primary Header</h4>
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="bg-primary text-white">
              <th v-for="h in headers" :key="h" class="py-3 px-4 font-semibold text-xs uppercase tracking-wider">
                {{ h }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="emp in employees" :key="emp.name" class="border-b border-outline-variant last:border-0 hover:bg-primary/5 transition-colors">
              <td class="py-3 px-4 font-medium text-on-surface">{{ emp.name }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.position }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.office }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.age }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.startDate }}</td>
              <td class="py-3 px-4 font-medium text-on-surface tabular-nums">{{ emp.salary }}</td>
            </tr>
          </tbody>
        </table>
      </WxCard>

      <!-- Dark header -->
      <WxCard>
        <h4 class="text-xs font-semibold uppercase tracking-wider text-on-surface-variant mb-3">Dark Header</h4>
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="bg-surface-container text-on-surface">
              <th v-for="h in headers" :key="h" class="py-3 px-4 font-semibold text-xs uppercase tracking-wider">
                {{ h }}
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(emp, idx) in employees"
              :key="emp.name"
              :class="idx % 2 === 0 ? 'bg-surface-container-lowest' : 'bg-surface-container'"
              class="hover:bg-surface-container-low transition-colors"
            >
              <td class="py-3 px-4 font-medium text-on-surface">{{ emp.name }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.position }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.office }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.age }}</td>
              <td class="py-3 px-4 text-on-surface-variant">{{ emp.startDate }}</td>
              <td class="py-3 px-4 font-medium text-on-surface tabular-nums">{{ emp.salary }}</td>
            </tr>
          </tbody>
        </table>
      </WxCard>
    </section>
  </div>
</template>
