export interface ApiResult<T> {
  value?: T
  isSuccess: boolean
  error?: string
}

export interface ApiError {
  message: string
  statusCode?: number
}

export interface ItemFilters {
  search?: string
  categoryId?: string
  room?: string
  tags?: string[]
  minPrice?: number
  maxPrice?: number
}
