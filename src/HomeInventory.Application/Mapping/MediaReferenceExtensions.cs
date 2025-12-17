using HomeInventory.Application.DTOs;
using HomeInventory.Domain.Enums;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Application.Mapping;

public static class MediaReferenceExtensions
{
    public static MediaReferenceDto FromDomain(this MediaReference mediaReference) => new()
    {
        FileName = mediaReference.FileName,
        FileUrl = mediaReference.FileUrl,
        MediaType = mediaReference.MediaType.FromDomain(),
        UploadedAt = mediaReference.UploadedAt,
        FileSizeBytes = mediaReference.FileSizeBytes,
    };

    private static string FromDomain(this MediaType mediaType) => mediaType switch
    {
        MediaType.Photo => "Photo",
        MediaType.Receipt => "Receipt",
        _ => throw new InvalidOperationException("Unknown media type."),
    };
}