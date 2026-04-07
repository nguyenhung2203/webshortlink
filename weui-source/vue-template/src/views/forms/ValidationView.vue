<script setup lang="ts">
import { ref, reactive } from 'vue'
import { UserPlus, Mail, Lock, CheckCircle, Send } from 'lucide-vue-next'

import WxPageHeader from '@/components/ui/WxPageHeader.vue'
import WxCard from '@/components/ui/WxCard.vue'
import WxButton from '@/components/ui/WxButton.vue'
import WxInput from '@/components/ui/WxInput.vue'
import WxPasswordInput from '@/components/ui/WxPasswordInput.vue'
import WxCheckbox from '@/components/ui/WxCheckbox.vue'
import WxTextarea from '@/components/ui/WxTextarea.vue'
import WxAlert from '@/components/ui/WxAlert.vue'
import { useFormValidation } from '@/composables/useFormValidation'

// ── Registration Form ──
const regForm = reactive({
  name: '',
  email: '',
  password: '',
  confirmPassword: '',
  terms: false,
})
const regSubmitted = ref(false)

const regRules = {
  name: (val: unknown) => {
    if (!val || (val as string).trim().length === 0) return 'Name is required'
    if ((val as string).trim().length < 3) return 'Name must be at least 3 characters'
    return true
  },
  email: (val: unknown) => {
    if (!val || (val as string).trim().length === 0) return 'Email is required'
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val as string)) return 'Please enter a valid email address'
    return true
  },
  password: (val: unknown) => {
    if (!val || (val as string).length === 0) return 'Password is required'
    if ((val as string).length < 8) return 'Password must be at least 8 characters'
    return true
  },
  confirmPassword: (val: unknown) => {
    if (!val || (val as string).length === 0) return 'Please confirm your password'
    if (val !== regForm.password) return 'Passwords do not match'
    return true
  },
  terms: (val: unknown) => {
    if (!val) return 'You must accept the terms and conditions'
    return true
  },
}

const { errors: regErrors, validate: validateReg, clearErrors: clearRegErrors } = useFormValidation(regRules)

function submitRegistration() {
  const valid = validateReg({
    name: regForm.name,
    email: regForm.email,
    password: regForm.password,
    confirmPassword: regForm.confirmPassword,
    terms: regForm.terms,
  })
  if (valid) {
    regSubmitted.value = true
  }
}

function resetRegistration() {
  regForm.name = ''
  regForm.email = ''
  regForm.password = ''
  regForm.confirmPassword = ''
  regForm.terms = false
  regSubmitted.value = false
  clearRegErrors()
}

// ── Contact Form ──
const contactForm = reactive({
  name: '',
  email: '',
  message: '',
})
const contactSubmitted = ref(false)

const contactRules = {
  name: (val: unknown) => {
    if (!val || (val as string).trim().length === 0) return 'Name is required'
    return true
  },
  email: (val: unknown) => {
    if (!val || (val as string).trim().length === 0) return 'Email is required'
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val as string)) return 'Please enter a valid email'
    return true
  },
  message: (val: unknown) => {
    if (!val || (val as string).trim().length === 0) return 'Message is required'
    if ((val as string).trim().length < 10) return 'Message must be at least 10 characters'
    return true
  },
}

const { errors: contactErrors, validate: validateContact, clearErrors: clearContactErrors } = useFormValidation(contactRules)

function submitContact() {
  const valid = validateContact({
    name: contactForm.name,
    email: contactForm.email,
    message: contactForm.message,
  })
  if (valid) {
    contactSubmitted.value = true
  }
}

function resetContact() {
  contactForm.name = ''
  contactForm.email = ''
  contactForm.message = ''
  contactSubmitted.value = false
  clearContactErrors()
}
</script>

<template>
  <div class="space-y-10">
    <WxPageHeader
      title="Form Validation"
      description="Client-side form validation using the useFormValidation composable with inline error messages and success states."
    />

    <!-- ==================== Registration Form ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <UserPlus class="w-5 h-5" />
        <span>Registration Form</span>
      </div>
      <WxCard>
        <!-- Success state -->
        <div v-if="regSubmitted" class="space-y-4">
          <WxAlert type="success" title="Registration Successful!">
            Your account has been created. Welcome aboard, {{ regForm.name }}!
          </WxAlert>
          <div class="flex items-center justify-center py-6">
            <div class="text-center space-y-3">
              <div class="w-16 h-16 rounded-full bg-success/10 flex items-center justify-center mx-auto">
                <CheckCircle class="w-8 h-8 text-green-600 dark:text-green-400" />
              </div>
              <p class="text-sm text-on-surface-variant">Account created for <span class="font-semibold text-on-surface">{{ regForm.email }}</span></p>
              <WxButton variant="secondary" size="sm" @click="resetRegistration">Register Another</WxButton>
            </div>
          </div>
        </div>

        <!-- Registration form -->
        <form v-else class="space-y-5 max-w-lg" @submit.prevent="submitRegistration">
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">
              Full Name <span class="text-red-500">*</span>
            </label>
            <WxInput
              v-model="regForm.name"
              placeholder="John Doe"
              :error="!!regErrors.name"
              :error-message="regErrors.name"
            />
          </div>

          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">
              Email Address <span class="text-red-500">*</span>
            </label>
            <WxInput
              v-model="regForm.email"
              type="email"
              placeholder="john@example.com"
              :error="!!regErrors.email"
              :error-message="regErrors.email"
            >
              <template #prefix><Mail class="w-4 h-4" /></template>
            </WxInput>
          </div>

          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">
              Password <span class="text-red-500">*</span>
            </label>
            <WxPasswordInput
              v-model="regForm.password"
            />
            <p v-if="regErrors.password" class="text-xs text-red-500">{{ regErrors.password }}</p>
            <p v-else class="text-xs text-on-surface-variant">Must be at least 8 characters</p>
          </div>

          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">
              Confirm Password <span class="text-red-500">*</span>
            </label>
            <WxPasswordInput
              v-model="regForm.confirmPassword"
            />
            <p v-if="regErrors.confirmPassword" class="text-xs text-red-500">{{ regErrors.confirmPassword }}</p>
          </div>

          <div class="space-y-1">
            <WxCheckbox v-model="regForm.terms" label="I accept the Terms and Conditions" />
            <p v-if="regErrors.terms" class="text-xs text-red-500 ml-6">{{ regErrors.terms }}</p>
          </div>

          <div class="flex items-center gap-3 pt-2">
            <WxButton variant="primary" type="submit">
              <template #icon><UserPlus class="w-4 h-4" /></template>
              Create Account
            </WxButton>
            <WxButton variant="ghost" type="button" @click="resetRegistration">Clear</WxButton>
          </div>
        </form>
      </WxCard>
    </section>

    <!-- ==================== Contact Form ==================== -->
    <section class="space-y-6">
      <div class="flex items-center space-x-2 text-primary font-bold text-lg">
        <Send class="w-5 h-5" />
        <span>Contact Form</span>
      </div>
      <WxCard>
        <!-- Success state -->
        <div v-if="contactSubmitted" class="space-y-4">
          <WxAlert type="success" title="Message Sent!">
            Thank you, {{ contactForm.name }}. We will get back to you shortly.
          </WxAlert>
          <div class="flex justify-center pt-2">
            <WxButton variant="secondary" size="sm" @click="resetContact">Send Another Message</WxButton>
          </div>
        </div>

        <!-- Contact form -->
        <form v-else class="space-y-5 max-w-lg" @submit.prevent="submitContact">
          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">
              Name <span class="text-red-500">*</span>
            </label>
            <WxInput
              v-model="contactForm.name"
              placeholder="Your name"
              :error="!!contactErrors.name"
              :error-message="contactErrors.name"
            />
          </div>

          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">
              Email <span class="text-red-500">*</span>
            </label>
            <WxInput
              v-model="contactForm.email"
              type="email"
              placeholder="you@example.com"
              :error="!!contactErrors.email"
              :error-message="contactErrors.email"
            >
              <template #prefix><Mail class="w-4 h-4" /></template>
            </WxInput>
          </div>

          <div class="space-y-1.5">
            <label class="block text-sm font-medium text-on-surface">
              Message <span class="text-red-500">*</span>
            </label>
            <WxTextarea
              v-model="contactForm.message"
              placeholder="How can we help you?"
              :rows="4"
            />
            <p v-if="contactErrors.message" class="text-xs text-red-500">{{ contactErrors.message }}</p>
            <p v-else class="text-xs text-on-surface-variant">Minimum 10 characters</p>
          </div>

          <div class="flex items-center gap-3 pt-2">
            <WxButton variant="primary" type="submit">
              <template #icon><Send class="w-4 h-4" /></template>
              Send Message
            </WxButton>
            <WxButton variant="ghost" type="button" @click="resetContact">Clear</WxButton>
          </div>
        </form>
      </WxCard>
    </section>
  </div>
</template>
