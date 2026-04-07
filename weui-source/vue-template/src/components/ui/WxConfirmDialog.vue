<script setup lang="ts">
import { computed } from 'vue'
import { Info, AlertTriangle, AlertCircle } from 'lucide-vue-next'
import WxModal from './WxModal.vue'
import WxButton from './WxButton.vue'

type DialogType = 'info' | 'warning' | 'danger'

const props = withDefaults(defineProps<{
  modelValue: boolean
  title?: string
  message?: string
  type?: DialogType
  confirmText?: string
  cancelText?: string
}>(), {
  title: 'Confirm',
  message: '',
  type: 'info',
  confirmText: 'Confirm',
  cancelText: 'Cancel',
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  confirm: []
  cancel: []
}>()

const typeConfig: Record<DialogType, { iconBg: string; iconColor: string; btnVariant: 'primary' | 'danger' | 'success' }> = {
  info: { iconBg: 'bg-info/10', iconColor: 'text-info', btnVariant: 'primary' },
  warning: { iconBg: 'bg-warning/10', iconColor: 'text-warning', btnVariant: 'primary' },
  danger: { iconBg: 'bg-danger/10', iconColor: 'text-danger', btnVariant: 'danger' },
}

const config = computed(() => typeConfig[props.type])

const iconComponent = computed(() => {
  const map: Record<DialogType, typeof Info> = {
    info: Info,
    warning: AlertTriangle,
    danger: AlertCircle,
  }
  return map[props.type]
})

function close() {
  emit('update:modelValue', false)
}

function handleConfirm() {
  emit('confirm')
  close()
}

function handleCancel() {
  emit('cancel')
  close()
}
</script>

<template>
  <WxModal
    :model-value="modelValue"
    :title="title"
    size="sm"
    @update:model-value="emit('update:modelValue', $event)"
  >
    <div class="flex flex-col items-center text-center gap-4">
      <div class="w-12 h-12 rounded-full flex items-center justify-center" :class="config.iconBg">
        <component :is="iconComponent" class="w-6 h-6" :class="config.iconColor" />
      </div>
      <p class="text-sm text-on-surface-variant leading-relaxed">{{ message }}</p>
    </div>

    <template #footer>
      <WxButton variant="secondary" size="sm" @click="handleCancel">
        {{ cancelText }}
      </WxButton>
      <WxButton :variant="config.btnVariant" size="sm" @click="handleConfirm">
        {{ confirmText }}
      </WxButton>
    </template>
  </WxModal>
</template>
