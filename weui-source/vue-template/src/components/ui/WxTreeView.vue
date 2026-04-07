<script setup lang="ts">
import { ref } from 'vue'
import { ChevronRight } from 'lucide-vue-next'
import type { TreeNode } from '@/types'

defineProps<{
  nodes: TreeNode[]
  depth?: number
}>()

const emit = defineEmits<{
  select: [nodeId: string]
}>()

const expandedIds = ref<Set<string>>(new Set())
const selectedId = ref<string>('')

function toggle(nodeId: string) {
  if (expandedIds.value.has(nodeId)) {
    expandedIds.value.delete(nodeId)
  } else {
    expandedIds.value.add(nodeId)
  }
}

function selectNode(nodeId: string) {
  selectedId.value = nodeId
  emit('select', nodeId)
}
</script>

<template>
  <ul class="select-none" :class="depth === undefined || depth === 0 ? '' : ''">
    <li v-for="node in nodes" :key="node.id">
      <div
        class="flex items-center gap-1.5 px-2 py-1.5 rounded-lg cursor-pointer transition-colors duration-150 text-sm"
        :class="selectedId === node.id
          ? 'bg-primary/10 text-primary font-medium'
          : 'text-on-surface hover:bg-surface-container-high/50'"
        :style="{ paddingLeft: `${((depth ?? 0) * 20) + 8}px` }"
        @click="selectNode(node.id)"
      >
        <!-- Expand/collapse toggle -->
        <button
          v-if="node.children && node.children.length > 0"
          class="p-0.5 rounded hover:bg-surface-container-high transition-all cursor-pointer"
          @click.stop="toggle(node.id)"
        >
          <ChevronRight
            class="w-4 h-4 text-on-surface-variant transition-transform duration-200"
            :class="expandedIds.has(node.id) && 'rotate-90'"
          />
        </button>
        <span v-else class="w-5" />

        <!-- Icon (if provided as slot or generic) -->
        <span class="truncate">{{ node.label }}</span>
      </div>

      <!-- Recursive children -->
      <Transition name="tree">
        <WxTreeView
          v-if="node.children && node.children.length > 0 && expandedIds.has(node.id)"
          :nodes="node.children"
          :depth="(depth ?? 0) + 1"
          @select="emit('select', $event)"
        />
      </Transition>
    </li>
  </ul>
</template>

<style scoped>
.tree-enter-active,
.tree-leave-active {
  transition: all 0.2s ease;
  overflow: hidden;
}
.tree-enter-from,
.tree-leave-to {
  opacity: 0;
  max-height: 0;
}
.tree-enter-to,
.tree-leave-from {
  opacity: 1;
  max-height: 500px;
}
</style>
