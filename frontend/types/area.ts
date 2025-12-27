export interface AreaDto {
  id: string
  name: string
  description?: string
  parentAreaId?: string
  parentAreaName?: string
  fullPath: string
  itemCount: number
  childCount: number
  createdAt: string
  updatedAt: string
}

export interface CreateAreaDto {
  name: string
  parentAreaId?: string
  description?: string
}

export interface UpdateAreaDto extends CreateAreaDto {
  id: string
}

export interface AreaTreeNode extends AreaDto {
  children: AreaTreeNode[]
}
