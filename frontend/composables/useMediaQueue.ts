import { db, type MediaQueueItem } from '~/utils/db'

export const useMediaQueue = () => {
  const { isOnline } = useOffline()
  const uploadingCount = useState('uploadingMediaCount', () => 0)
  const pendingMediaCount = useState('pendingMediaCount', () => 0)

  // Queue media for upload
  const queueMedia = async (
    entityId: string,
    entityType: 'item',
    mediaType: 'photo' | 'receipt',
    file: File
  ) => {
    const mediaItem: MediaQueueItem = {
      id: `media_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
      entityId,
      entityType,
      mediaType,
      file,
      fileName: file.name,
      fileSize: file.size,
      timestamp: Date.now(),
      retryCount: 0
    }

    await db.mediaQueue.add(mediaItem)
    await updateMediaCount()

    // If online, try to upload immediately
    if (isOnline.value) {
      processMediaQueue()
    }

    return mediaItem.id
  }

  // Process the media upload queue
  const processMediaQueue = async () => {
    if (!isOnline.value || uploadingCount.value > 0) return

    const mediaItems = await db.mediaQueue.orderBy('timestamp').toArray()

    if (mediaItems.length === 0) return

    for (const media of mediaItems) {
      // Check if the entity still has a temp ID
      // If so, skip and wait for entity sync to complete
      if (media.entityId.startsWith('temp_')) {
        continue
      }

      try {
        uploadingCount.value++
        await uploadMedia(media)
        await db.mediaQueue.delete(media.id)
      } catch (error: any) {
        await handleUploadError(media, error)
      } finally {
        uploadingCount.value--
      }
    }

    await updateMediaCount()
  }

  // Upload a single media file
  const uploadMedia = async (media: MediaQueueItem) => {
    const config = useRuntimeConfig()
    const baseURL = config.public.apiBase

    // Create FormData for file upload
    const formData = new FormData()
    formData.append('file', media.file, media.fileName)

    // Determine endpoint based on media type
    const endpoint = `${baseURL}/items/${media.entityId}/${media.mediaType === 'photo' ? 'photos' : 'receipts'}`

    // Upload using $fetch
    await $fetch(endpoint, {
      method: 'POST',
      body: formData
      // Don't set Content-Type header - browser will set it with boundary
    })
  }

  // Handle upload errors
  const handleUploadError = async (media: MediaQueueItem, error: any) => {
    const retryCount = media.retryCount + 1

    if (retryCount > 3) {
      // After 3 retries, remove from queue and notify user
      await db.mediaQueue.delete(media.id)

      const toast = useToast()
      toast.error(`Failed to upload ${media.fileName}`)
    } else {
      // Update retry count
      await db.mediaQueue.update(media.id, {
        retryCount,
        lastError: error.message || 'Unknown error'
      })
    }
  }

  // Update pending media count
  const updateMediaCount = async () => {
    pendingMediaCount.value = await db.mediaQueue.count()
  }

  // Get pending media for a specific entity
  const getPendingMediaForEntity = async (entityId: string): Promise<MediaQueueItem[]> => {
    return await db.mediaQueue.where('entityId').equals(entityId).toArray()
  }

  // Clear media queue for an entity (e.g., if entity is deleted)
  const clearMediaForEntity = async (entityId: string) => {
    const items = await db.mediaQueue.where('entityId').equals(entityId).toArray()
    for (const item of items) {
      await db.mediaQueue.delete(item.id)
    }
    await updateMediaCount()
  }

  return {
    queueMedia,
    processMediaQueue,
    uploadingCount: readonly(uploadingCount),
    pendingMediaCount: readonly(pendingMediaCount),
    getPendingMediaForEntity,
    clearMediaForEntity,
    updateMediaCount
  }
}
