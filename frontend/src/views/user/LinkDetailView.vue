<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import WxPageHeader from '@/components/ui/WxPageHeader.vue'

const route = useRoute()
const linkId = route.params.id
</script>

<template>
  <div>
    <WxPageHeader
      title="Chi tiết Link"
      :subtitle="`Thông Đài chi tiết cho link ID: ${linkId}`"
    />
  </div>
</template>
