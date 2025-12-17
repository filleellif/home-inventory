// Auth types - prepared for future backend implementation

export interface LoginCredentials {
  email: string
  password: string
}

export interface RegisterCredentials {
  email: string
  password: string
  confirmPassword: string
  name: string
}

export interface User {
  id: string
  email: string
  name: string
}

export interface AuthResponse {
  token: string
  user: User
}
