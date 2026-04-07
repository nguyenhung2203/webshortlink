<script setup lang="ts">
import { Loader2 } from 'lucide-vue-next'

type Size = 'sm' | 'md' | 'lg'

withDefaults(defineProps<{
  size?: Size
}>(), {
  size: 'md',
})

const sizeMap: Record<Size, number> = {
  sm: 16,
  md: 24,
  lg: 32,
}
</script>

<template>
  <Loader2
    class="animate-spin text-primary"
    :size="sizeMap[size]"
  />
</template>
