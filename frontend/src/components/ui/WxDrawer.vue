<script setup lang="ts">
import { watch, onUnmounted } from 'vue'
import { X } from 'lucide-vue-next'

type Side = 'left' | 'right'
type Size = 'sm' | 'md' | 'lg'

const props = withDefaults(defineProps<{
  modelValue: boolean
  title?: string
  side?: Side
  size?: Size
}>(), {
  title: '',
  side: 'right',
  size: 'md',
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const sizeClasses: Record<Size, string> = {
  sm: 'w-80',
  md: 'w-96',
  lg: 'w-[480px]',
}

function close() {
  emit('update:modelValue', false)
}

function onKeydown(e: KeyboardEvent) {
  if (e.key === 'Escape') close()
}

watch(() => props.modelValue, (open) => {
  if (open) {
    document.body.style.overflow = 'hidden'
    document.addEventListener('keydown', onKeydown)
  } else {
    document.body.style.overflow = ''
    document.removeEventListener('keydown', onKeydown)
  }
})

onUnmounted(() => {
  document.body.style.overflow = ''
  document.removeEventListener('keydown', onKeydown)
})
</script>

<template>
  <Teleport to="body">
    <Transition name="drawer-backdrop">
      <div
        v-if="modelValue"
        class="fixed inset-0 z-50 flex"
        :class="side === 'right' ? 'justify-end' : 'justify-start'"
      >
        <!-- Backdrop -->
        <div
          class="absolute inset-0 bg-black/50 backdrop-blur-sm"
          @click="close"
        />

        <!-- Panel -->
        <Transition :name="side === 'right' ? 'drawer-right' : 'drawer-left'" appear>
          <div
            v-if="modelValue"
            class="relative h-full flex flex-col bg-surface-container border-outline-variant shadow-2xl max-w-[90vw]"
            :class="[
              sizeClasses[size],
              side === 'right' ? 'border-l' : 'border-r',
            ]"
          >
            <!-- Header -->
            <div class="flex items-center justify-between px-6 py-4 border-b border-outline-variant shrink-0">
              <h3 class="text-lg font-semibold text-on-surface">{{ title }}</h3>
              <button
                class="p-1.5 rounded-lg text-on-surface-variant hover:bg-surface-container-high hover:text-on-surface transition-colors cursor-pointer"
                @click="close"
              >
                <X class="w-4 h-4" />
              </button>
            </div>

            <!-- Body -->
            <div class="flex-1 overflow-y-auto px-6 py-5 text-on-surface">
              <slot />
            </div>

            <!-- Footer -->
            <div v-if="$slots.footer" class="px-6 py-4 border-t border-outline-variant flex items-center justify-end gap-3 shrink-0">
              <slot name="footer" />
            </div>
          </div>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.drawer-backdrop-enter-active,
.drawer-backdrop-leave-active {
  transition: opacity 0.25s ease;
}
.drawer-backdrop-enter-from,
.drawer-backdrop-leave-to {
  opacity: 0;
}

.drawer-right-enter-active,
.drawer-right-leave-active,
.drawer-left-enter-active,
.drawer-left-leave-active {
  transition: transform 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

.drawer-right-enter-from,
.drawer-right-leave-to {
  transform: translateX(100%);
}

.drawer-left-enter-from,
.drawer-left-leave-to {
  transform: translateX(-100%);
}
</style>
