export interface ItemDto {
  id: string
  name: string
  description?: string
  quantity: number
  areaId?: string
  categoryId?: string
  photos: MediaReferenceDto[]
  receipts: MediaReferenceDto[]
  createdAt: string
  updatedAt: string
}

export interface MediaReferenceDto {
  fileName: string
  fileUrl: string
  mediaType: string
  uploadedAt: string
  fileSizeBytes: number
}

export interface CreateItemDto {
  name: string
  description?: string
  quantity: number
  areaId?: string
  categoryId?: string
}

export interface UpdateItemDto extends CreateItemDto {
  id: string
}
