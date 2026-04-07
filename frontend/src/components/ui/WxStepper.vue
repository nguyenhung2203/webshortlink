<script setup lang="ts">
import { Check } from 'lucide-vue-next'

interface Step {
  label: string
  description?: string
}

withDefaults(defineProps<{
  steps: Step[]
  currentStep?: number
}>(), {
  currentStep: 0,
})
</script>

<template>
  <div class="flex items-start w-full">
    <template v-for="(step, index) in steps" :key="index">
      <div class="flex flex-col items-center flex-1 min-w-0">
        <div class="flex items-center w-full">
          <!-- Left line -->
          <div
            class="flex-1 h-0.5 transition-colors duration-300"
            :class="index === 0 ? 'bg-transparent' : (index <= currentStep ? 'bg-primary' : 'bg-surface-container-high')"
          />
          <!-- Circle -->
          <div
            class="w-8 h-8 rounded-full flex items-center justify-center shrink-0 text-sm font-semibold transition-all duration-300 border-2"
            :class="[
              index < currentStep
                ? 'bg-primary border-primary text-white'
                : index === currentStep
                  ? 'border-primary text-primary bg-primary/10'
                  : 'border-surface-container-high text-on-surface-variant bg-surface-container-lowest'
            ]"
          >
            <Check v-if="index < currentStep" :size="16" />
            <span v-else>{{ index + 1 }}</span>
          </div>
          <!-- Right line -->
          <div
            class="flex-1 h-0.5 transition-colors duration-300"
            :class="index === steps.length - 1 ? 'bg-transparent' : (index < currentStep ? 'bg-primary' : 'bg-surface-container-high')"
          />
        </div>
        <div class="mt-2 text-center px-2">
          <p
            class="text-xs font-semibold"
            :class="index <= currentStep ? 'text-on-surface' : 'text-on-surface-variant'"
          >
            {{ step.label }}
          </p>
          <p v-if="step.description" class="text-[11px] text-on-surface-variant mt-0.5">
            {{ step.description }}
          </p>
        </div>
      </div>
    </template>
  </div>
</template>
