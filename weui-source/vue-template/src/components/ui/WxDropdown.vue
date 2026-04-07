<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue'

const open = ref(false)
const wrapperRef = ref<HTMLElement | null>(null)

function toggle() {
  open.value = !open.value
}

function close() {
  open.value = false
}

function onClickOutside(e: MouseEvent) {
  if (wrapperRef.value && !wrapperRef.value.contains(e.target as Node)) {
    close()
  }
}

onMounted(() => document.addEventListener('click', onClickOutside))
onBeforeUnmount(() => document.removeEventListener('click', onClickOutside))

defineExpose({ open, close, toggle })
</script>

<template>
  <div ref="wrapperRef" class="relative inline-block">
    <div @click="toggle">
      <slot name="trigger" :open="open" />
    </div>
    <Transition
      enter-active-class="transition duration-150 ease-out"
      enter-from-class="opacity-0 scale-95"
      enter-to-class="opacity-100 scale-100"
      leave-active-class="transition duration-100 ease-in"
      leave-from-class="opacity-100 scale-100"
      leave-to-class="opacity-0 scale-95"
    >
      <div
        v-if="open"
        class="absolute z-50 mt-1 min-w-[8rem] rounded-lg border border-outline-variant bg-surface-container-lowest shadow-lg py-1 origin-top-left"
      >
        <slot :close="close" />
      </div>
    </Transition>
  </div>
</template>
