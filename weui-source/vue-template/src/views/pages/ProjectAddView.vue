<script setup lang="ts">
import { ref } from 'vue'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxTextarea from '@/components/ui/WxTextarea.vue'
import WxSelect from '@/components/ui/WxSelect.vue'
import WxInputNumber from '@/components/ui/WxInputNumber.vue'
import WxMultiSelect from '@/components/ui/WxMultiSelect.vue'
import WxButton from '@/components/ui/WxButton.vue'
import { FolderPlus } from 'lucide-vue-next'

const form = ref({
  name: '',
  description: '',
  client: null as string | number | null,
  startDate: '',
  endDate: '',
  budget: 0,
  priority: null as string | number | null,
  teamMembers: [] as (string | number)[],
  status: null as string | number | null,
})

const clientOptions = [
  { label: 'Acme Corporation', value: 'acme' },
  { label: 'Globex Inc.', value: 'globex' },
  { label: 'Initech', value: 'initech' },
  { label: 'Umbrella Corp.', value: 'umbrella' },
  { label: 'Wayne Enterprises', value: 'wayne' },
]

const priorityOptions = [
  { label: 'Low', value: 'low' },
  { label: 'Medium', value: 'medium' },
  { label: 'High', value: 'high' },
  { label: 'Critical', value: 'critical' },
]

const teamOptions = [
  { label: 'Alice Wang', value: 'alice' },
  { label: 'Bob Smith', value: 'bob' },
  { label: 'Carol Davis', value: 'carol' },
  { label: 'David Kim', value: 'david' },
  { label: 'Eva Martinez', value: 'eva' },
  { label: 'Frank Lee', value: 'frank' },
  { label: 'Grace Chen', value: 'grace' },
  { label: 'Henry Park', value: 'henry' },
]

const statusOptions = [
  { label: 'Planning', value: 'planning' },
  { label: 'Active', value: 'active' },
  { label: 'On Hold', value: 'on-hold' },
  { label: 'Completed', value: 'completed' },
]
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader title="Add Project" description="Create a new project for your team">
      <template #actions>
        <WxButton variant="secondary" size="sm">Cancel</WxButton>
      </template>
    </WxPageHeader>

    <WxCard>
      <template #header>
        <div class="flex items-center gap-2">
          <FolderPlus :size="18" class="text-primary" />
          <h3 class="text-base font-semibold text-on-surface">Project Details</h3>
        </div>
      </template>

      <div class="space-y-5 max-w-2xl">
        <!-- Project Name -->
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1">Project Name <span class="text-danger">*</span></label>
          <WxInput v-model="form.name" placeholder="Enter project name" />
        </div>

        <!-- Description -->
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1">Description</label>
          <WxTextarea v-model="form.description" placeholder="Describe the project goals and scope..." :rows="4" />
        </div>

        <!-- Client -->
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1">Client</label>
          <WxSelect v-model="form.client" :options="clientOptions" placeholder="Select a client" />
        </div>

        <!-- Dates Row -->
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-on-surface mb-1">Start Date</label>
            <WxInput v-model="form.startDate" placeholder="YYYY-MM-DD" />
          </div>
          <div>
            <label class="block text-sm font-medium text-on-surface mb-1">End Date</label>
            <WxInput v-model="form.endDate" placeholder="YYYY-MM-DD" />
          </div>
        </div>

        <!-- Budget -->
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1">Budget ($)</label>
          <WxInputNumber v-model="form.budget" :min="0" :step="1000" />
        </div>

        <!-- Priority -->
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1">Priority</label>
          <WxSelect v-model="form.priority" :options="priorityOptions" placeholder="Select priority level" />
        </div>

        <!-- Team Members -->
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1">Team Members</label>
          <WxMultiSelect v-model="form.teamMembers" :options="teamOptions" placeholder="Select team members" />
        </div>

        <!-- Status -->
        <div>
          <label class="block text-sm font-medium text-on-surface mb-1">Status</label>
          <WxSelect v-model="form.status" :options="statusOptions" placeholder="Select project status" />
        </div>
      </div>

      <template #footer>
        <div class="flex justify-end gap-3">
          <WxButton variant="secondary">Cancel</WxButton>
          <WxButton>Create Project</WxButton>
        </div>
      </template>
    </WxCard>
  </div>
</template>
