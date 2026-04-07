<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  Bold, Italic, Underline, List, Link, AlignLeft, AlignCenter, AlignRight,
  Code, FileText, Columns,
} from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'

// Rich text editor
const richTextContent = ref('<p>Start typing your content here. This is a <strong>rich text</strong> editor placeholder with <em>basic formatting</em> support.</p>')

function execCommand(command: string, value?: string) {
  document.execCommand(command, false, value)
}

function insertLink() {
  const url = prompt('Enter URL:')
  if (url) execCommand('createLink', url)
}

// Code editor
const codeContent = ref(`<template>
  <div class="app">
    <h1>{{ title }}</h1>
    <p>Welcome to Vue 3</p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const title = ref('Hello World')
<\/script>`)

const codeLines = computed(() => {
  return codeContent.value.split('\n').length
})

// Markdown editor
const markdownSource = ref(`# Hello World

This is a **Markdown** preview demo.

## Features

- Real-time preview
- Basic formatting support
- Easy to use

### Code Example

\`const greeting = "Hello!"\`

> This is a blockquote that demonstrates
> the markdown rendering capability.

Visit [Vue.js](https://vuejs.org) for more info.`)

const markdownPreview = computed(() => {
  let html = markdownSource.value
    // Escape HTML
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    // Headings
    .replace(/^### (.+)$/gm, '<h3 class="text-base font-semibold text-on-surface mt-4 mb-2">$1</h3>')
    .replace(/^## (.+)$/gm, '<h2 class="text-lg font-bold text-on-surface mt-5 mb-2">$1</h2>')
    .replace(/^# (.+)$/gm, '<h1 class="text-xl font-bold text-on-surface mt-6 mb-3">$1</h1>')
    // Bold & italic
    .replace(/\*\*(.+?)\*\*/g, '<strong class="font-semibold">$1</strong>')
    .replace(/\*(.+?)\*/g, '<em>$1</em>')
    // Inline code
    .replace(/`([^`]+)`/g, '<code class="bg-surface-container px-1.5 py-0.5 rounded text-xs font-mono text-primary">$1</code>')
    // Blockquote
    .replace(/^&gt; (.+)$/gm, '<div class="border-l-4 border-primary/30 pl-4 py-1 text-on-surface-variant italic">$1</div>')
    // Lists
    .replace(/^- (.+)$/gm, '<li class="ml-4 list-disc text-sm text-on-surface">$1</li>')
    // Links
    .replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2" class="text-primary underline">$1</a>')
    // Paragraphs
    .replace(/\n\n/g, '</p><p class="text-sm text-on-surface-variant mb-3">')
    // Line breaks
    .replace(/\n/g, '<br>')
  return `<p class="text-sm text-on-surface-variant mb-3">${html}</p>`
})
</script>

<template>
  <div class="space-y-10">
    <WxPageHeader
      title="Text Editors"
      description="Rich text editor, code editor, and markdown preview components for content creation workflows."
    />

    <!-- ==================== Rich Text Editor ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <FileText class="w-5 h-5" />
        <span>Rich Text Editor</span>
      </div>
      <WxCard>
        <div class="border border-outline-variant rounded-lg overflow-hidden">
          <!-- Toolbar -->
          <div class="flex items-center gap-1 p-2 bg-surface-container border-b border-outline-variant flex-wrap">
            <button
              class="p-1.5 rounded hover:bg-surface-container-high transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
              title="Bold"
              @click="execCommand('bold')"
            >
              <Bold class="w-4 h-4" />
            </button>
            <button
              class="p-1.5 rounded hover:bg-surface-container-high transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
              title="Italic"
              @click="execCommand('italic')"
            >
              <Italic class="w-4 h-4" />
            </button>
            <button
              class="p-1.5 rounded hover:bg-surface-container-high transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
              title="Underline"
              @click="execCommand('underline')"
            >
              <Underline class="w-4 h-4" />
            </button>
            <div class="w-px h-5 bg-outline-variant mx-1" />
            <button
              class="p-1.5 rounded hover:bg-surface-container-high transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
              title="Unordered List"
              @click="execCommand('insertUnorderedList')"
            >
              <List class="w-4 h-4" />
            </button>
            <button
              class="p-1.5 rounded hover:bg-surface-container-high transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
              title="Insert Link"
              @click="insertLink"
            >
              <Link class="w-4 h-4" />
            </button>
            <div class="w-px h-5 bg-outline-variant mx-1" />
            <button
              class="p-1.5 rounded hover:bg-surface-container-high transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
              title="Align Left"
              @click="execCommand('justifyLeft')"
            >
              <AlignLeft class="w-4 h-4" />
            </button>
            <button
              class="p-1.5 rounded hover:bg-surface-container-high transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
              title="Align Center"
              @click="execCommand('justifyCenter')"
            >
              <AlignCenter class="w-4 h-4" />
            </button>
            <button
              class="p-1.5 rounded hover:bg-surface-container-high transition-colors text-on-surface-variant hover:text-on-surface cursor-pointer"
              title="Align Right"
              @click="execCommand('justifyRight')"
            >
              <AlignRight class="w-4 h-4" />
            </button>
          </div>
          <!-- Editable area -->
          <div
            class="min-h-[200px] p-4 text-sm text-on-surface focus:outline-none prose-sm bg-surface-container-lowest"
            contenteditable="true"
            v-html="richTextContent"
          />
        </div>
        <p class="text-xs text-on-surface-variant mt-2">
          Click on the toolbar buttons and type in the area above. Supports bold, italic, underline, lists, links, and alignment.
        </p>
      </WxCard>
    </section>

    <!-- ==================== Code Editor ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Code class="w-5 h-5" />
        <span>Code Editor</span>
      </div>
      <WxCard>
        <div class="border border-outline-variant rounded-lg overflow-hidden">
          <!-- Header bar -->
          <div class="flex items-center justify-between px-3 py-2 bg-gray-900 border-b border-gray-700">
            <div class="flex items-center gap-2">
              <div class="flex gap-1.5">
                <div class="w-3 h-3 rounded-full bg-red-500" />
                <div class="w-3 h-3 rounded-full bg-yellow-500" />
                <div class="w-3 h-3 rounded-full bg-green-500" />
              </div>
              <span class="text-xs text-gray-400 ml-2">App.vue</span>
            </div>
            <span class="text-xs text-gray-500">Vue 3 + TypeScript</span>
          </div>
          <!-- Editor area -->
          <div class="flex bg-gray-950">
            <!-- Line numbers -->
            <div class="select-none py-3 pl-3 pr-2 text-right border-r border-gray-800 min-w-[3rem]">
              <div
                v-for="n in codeLines"
                :key="n"
                class="text-xs leading-5 font-mono text-gray-600"
              >
                {{ n }}
              </div>
            </div>
            <!-- Code area -->
            <textarea
              v-model="codeContent"
              class="flex-1 bg-transparent text-gray-200 font-mono text-xs leading-5 p-3 outline-none resize-none min-h-[300px]"
              spellcheck="false"
            />
          </div>
        </div>
        <p class="text-xs text-on-surface-variant mt-2">
          Monospace editor with line numbers. Suitable as a code input placeholder.
        </p>
      </WxCard>
    </section>

    <!-- ==================== Markdown Preview ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Columns class="w-5 h-5" />
        <span>Markdown Preview</span>
      </div>
      <WxCard>
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-0 border border-outline-variant rounded-lg overflow-hidden">
          <!-- Left: Source -->
          <div class="flex flex-col border-b lg:border-b-0 lg:border-r border-outline-variant">
            <div class="px-3 py-2 bg-surface-container border-b border-outline-variant">
              <span class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider">Markdown</span>
            </div>
            <textarea
              v-model="markdownSource"
              class="flex-1 bg-surface-container-lowest text-sm text-on-surface font-mono p-4 outline-none resize-none min-h-[350px]"
              spellcheck="false"
            />
          </div>
          <!-- Right: Preview -->
          <div class="flex flex-col">
            <div class="px-3 py-2 bg-surface-container border-b border-outline-variant">
              <span class="text-xs font-semibold text-on-surface-variant uppercase tracking-wider">Preview</span>
            </div>
            <div
              class="flex-1 p-4 bg-surface-container-lowest overflow-auto min-h-[350px]"
              v-html="markdownPreview"
            />
          </div>
        </div>
        <p class="text-xs text-on-surface-variant mt-2">
          Type markdown on the left and see the rendered preview on the right in real time.
        </p>
      </WxCard>
    </section>
  </div>
</template>
