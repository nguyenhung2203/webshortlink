export function validatePasswordPolicy(password: string): string | null {
  if (!password || password.length < 8) {
    return 'Mật khẩu phải có ít nhất 8 ký tự.'
  }

  if (!/[A-Z]/.test(password)) {
    return 'Mật khẩu phải có ít nhất 1 chữ cái in hoa.'
  }

  if (!/\d/.test(password)) {
    return 'Mật khẩu phải có ít nhất 1 chữ số.'
  }

  return null
}

