import { ref, computed } from 'vue'

type ValidationRule = (value: unknown) => string | true
type Rules = Record<string, ValidationRule>

export function useFormValidation(rules: Rules) {
  const errors = ref<Record<string, string>>({})

  const isValid = computed(() => Object.keys(errors.value).length === 0)

  function validate(formData: Record<string, unknown>): boolean {
    const newErrors: Record<string, string> = {}

    for (const [field, rule] of Object.entries(rules)) {
      const result = rule(formData[field])
      if (result !== true) {
        newErrors[field] = result
      }
    }

    errors.value = newErrors
    return Object.keys(newErrors).length === 0
  }

  function clearErrors() {
    errors.value = {}
  }

  return { errors, validate, clearErrors, isValid }
}
