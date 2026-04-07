<script setup lang="ts">
import { Star, Zap, Shield, Crown, Award, Gem, Gift, Sparkles } from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
</script>

<template>
  <div class="space-y-6">
    <WxPageHeader
      title="Ribbons & Banners"
      description="Corner ribbons, banner strips, and label ribbons for highlighting promotions, statuses, and categories."
    />

    <!-- Corner Ribbons -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Corner Ribbons</h3>
      </template>
      <div class="grid grid-cols-1 md:grid-cols-4 gap-5">
        <div class="relative overflow-hidden rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm p-5 min-h-[160px]">
          <div class="corner-ribbon ribbon-primary">NEW</div>
          <div class="flex flex-col items-center text-center pt-4 gap-2">
            <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center">
              <Star :size="18" class="text-primary" />
            </div>
            <h4 class="text-sm font-semibold text-on-surface">Starter Plan</h4>
            <p class="text-xs text-on-surface-variant">Perfect for individuals and small projects getting started.</p>
          </div>
        </div>
        <div class="relative overflow-hidden rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm p-5 min-h-[160px]">
          <div class="corner-ribbon ribbon-success">SALE</div>
          <div class="flex flex-col items-center text-center pt-4 gap-2">
            <div class="w-10 h-10 rounded-full bg-success/10 flex items-center justify-center">
              <Zap :size="18" class="text-success" />
            </div>
            <h4 class="text-sm font-semibold text-on-surface">Pro Plan</h4>
            <p class="text-xs text-on-surface-variant">Advanced features for growing teams. Save 30% this month.</p>
          </div>
        </div>
        <div class="relative overflow-hidden rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm p-5 min-h-[160px]">
          <div class="corner-ribbon ribbon-warning">BETA</div>
          <div class="flex flex-col items-center text-center pt-4 gap-2">
            <div class="w-10 h-10 rounded-full bg-warning/10 flex items-center justify-center">
              <Shield :size="18" class="text-warning" />
            </div>
            <h4 class="text-sm font-semibold text-on-surface">Enterprise</h4>
            <p class="text-xs text-on-surface-variant">Early access to enterprise features with priority support.</p>
          </div>
        </div>
        <div class="relative overflow-hidden rounded-xl border border-outline-variant bg-surface-container-lowest shadow-sm p-5 min-h-[160px]">
          <div class="corner-ribbon ribbon-danger">HOT</div>
          <div class="flex flex-col items-center text-center pt-4 gap-2">
            <div class="w-10 h-10 rounded-full bg-danger/10 flex items-center justify-center">
              <Crown :size="18" class="text-danger" />
            </div>
            <h4 class="text-sm font-semibold text-on-surface">Ultimate</h4>
            <p class="text-xs text-on-surface-variant">Everything included. Unlimited usage with white-glove onboarding.</p>
          </div>
        </div>
      </div>
    </WxCard>

    <!-- Banner Ribbons -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Banner Ribbons</h3>
      </template>
      <div class="space-y-4">
        <div class="rounded-lg overflow-hidden">
          <div class="bg-gradient-to-r from-cyan-500 to-blue-600 px-5 py-3 flex items-center justify-between">
            <div class="flex items-center gap-3">
              <Gift :size="18" class="text-white" />
              <div>
                <p class="text-sm font-semibold text-white">Spring Sale - 40% Off All Plans</p>
                <p class="text-xs text-white/70">Limited time offer. Ends April 15, 2026.</p>
              </div>
            </div>
            <button class="px-3 py-1 text-xs font-medium bg-white/20 text-white rounded-md hover:bg-white/30 transition-colors">
              Claim Now
            </button>
          </div>
        </div>
        <div class="rounded-lg overflow-hidden">
          <div class="bg-gradient-to-r from-green-500 to-emerald-600 px-5 py-3 flex items-center justify-between">
            <div class="flex items-center gap-3">
              <Award :size="18" class="text-white" />
              <div>
                <p class="text-sm font-semibold text-white">Congratulations! You've earned Gold status.</p>
                <p class="text-xs text-white/70">Enjoy exclusive benefits and priority support.</p>
              </div>
            </div>
            <button class="px-3 py-1 text-xs font-medium bg-white/20 text-white rounded-md hover:bg-white/30 transition-colors">
              View Benefits
            </button>
          </div>
        </div>
        <div class="rounded-lg overflow-hidden">
          <div class="bg-gradient-to-r from-amber-500 to-orange-500 px-5 py-3 flex items-center justify-between">
            <div class="flex items-center gap-3">
              <Sparkles :size="18" class="text-white" />
              <div>
                <p class="text-sm font-semibold text-white">New Feature: AI-Powered Analytics</p>
                <p class="text-xs text-white/70">Try our new intelligent reporting engine today.</p>
              </div>
            </div>
            <button class="px-3 py-1 text-xs font-medium bg-white/20 text-white rounded-md hover:bg-white/30 transition-colors">
              Try Now
            </button>
          </div>
        </div>
        <div class="rounded-lg overflow-hidden">
          <div class="bg-gradient-to-r from-rose-500 to-red-600 px-5 py-3 flex items-center justify-between">
            <div class="flex items-center gap-3">
              <Shield :size="18" class="text-white" />
              <div>
                <p class="text-sm font-semibold text-white">Security Update Required</p>
                <p class="text-xs text-white/70">Please update your password to meet new security requirements.</p>
              </div>
            </div>
            <button class="px-3 py-1 text-xs font-medium bg-white/20 text-white rounded-md hover:bg-white/30 transition-colors">
              Update Now
            </button>
          </div>
        </div>
      </div>
    </WxCard>

    <!-- Label Ribbons -->
    <WxCard>
      <template #header>
        <h3 class="text-sm font-semibold text-on-surface">Label Ribbons</h3>
      </template>
      <p class="text-xs text-on-surface-variant mb-4">Inline labels with arrow shape for categorization and status indicators.</p>
      <div class="space-y-5">
        <div class="flex flex-wrap gap-3">
          <span class="label-ribbon label-primary">Featured</span>
          <span class="label-ribbon label-success">Verified</span>
          <span class="label-ribbon label-warning">Pending</span>
          <span class="label-ribbon label-danger">Urgent</span>
        </div>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="flex items-start gap-3 p-4 rounded-xl border border-outline-variant bg-surface-container-lowest">
            <span class="label-ribbon label-primary shrink-0 mt-0.5">v4.2</span>
            <div>
              <h4 class="text-sm font-semibold text-on-surface">Release Notes</h4>
              <p class="text-xs text-on-surface-variant mt-1">Improved performance, new chart components, and 15 bug fixes included in this release.</p>
            </div>
          </div>
          <div class="flex items-start gap-3 p-4 rounded-xl border border-outline-variant bg-surface-container-lowest">
            <span class="label-ribbon label-success shrink-0 mt-0.5">Stable</span>
            <div>
              <h4 class="text-sm font-semibold text-on-surface">Production Build</h4>
              <p class="text-xs text-on-surface-variant mt-1">All tests passing. Zero critical vulnerabilities detected in the latest security scan.</p>
            </div>
          </div>
          <div class="flex items-start gap-3 p-4 rounded-xl border border-outline-variant bg-surface-container-lowest">
            <span class="label-ribbon label-warning shrink-0 mt-0.5">Review</span>
            <div>
              <h4 class="text-sm font-semibold text-on-surface">Pull Request #312</h4>
              <p class="text-xs text-on-surface-variant mt-1">Awaiting code review from 2 team members. 4 files changed, 127 additions.</p>
            </div>
          </div>
          <div class="flex items-start gap-3 p-4 rounded-xl border border-outline-variant bg-surface-container-lowest">
            <span class="label-ribbon label-danger shrink-0 mt-0.5">Critical</span>
            <div>
              <h4 class="text-sm font-semibold text-on-surface">Incident Report</h4>
              <p class="text-xs text-on-surface-variant mt-1">Database failover triggered at 03:42 UTC. Recovery completed within SLA window.</p>
            </div>
          </div>
        </div>
      </div>
    </WxCard>
  </div>
</template>

<style scoped>
.corner-ribbon {
  position: absolute;
  top: 14px;
  right: -34px;
  width: 130px;
  padding: 5px 0;
  text-align: center;
  font-size: 10px;
  font-weight: 700;
  letter-spacing: 1.5px;
  color: white;
  transform: rotate(45deg);
  z-index: 1;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}
.ribbon-primary { background: linear-gradient(135deg, #22d3ee, #3b82f6); }
.ribbon-success { background: linear-gradient(135deg, #22c55e, #16a34a); }
.ribbon-warning { background: linear-gradient(135deg, #f59e0b, #d97706); }
.ribbon-danger { background: linear-gradient(135deg, #f43f5e, #dc2626); }

.label-ribbon {
  display: inline-flex;
  align-items: center;
  padding: 3px 12px 3px 10px;
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.3px;
  color: white;
  border-radius: 3px 0 0 3px;
  position: relative;
  clip-path: polygon(0 0, calc(100% - 8px) 0, 100% 50%, calc(100% - 8px) 100%, 0 100%);
  padding-right: 18px;
}
.label-primary { background: linear-gradient(135deg, #22d3ee, #3b82f6); }
.label-success { background: linear-gradient(135deg, #22c55e, #16a34a); }
.label-warning { background: linear-gradient(135deg, #f59e0b, #d97706); }
.label-danger { background: linear-gradient(135deg, #f43f5e, #dc2626); }
</style>
