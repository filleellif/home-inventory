using HomeInventory.Domain.Common;
using HomeInventory.Domain.Enums;

namespace HomeInventory.Domain.ValueObjects;

public class MediaReference : ValueObject
{
    public string FileName { get; }
    public string FileUrl { get; }
    public MediaType MediaType { get; }
    public DateTime UploadedAt { get; }
    public long FileSizeBytes { get; }

    private MediaReference(string fileName, string fileUrl, MediaType mediaType, DateTime uploadedAt, long fileSizeBytes)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("File name cannot be empty.");

        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new DomainException("File URL cannot be empty.");

        if (fileSizeBytes <= 0)
            throw new DomainException("File size must be greater than zero.");

        if (fileSizeBytes > 10 * 1024 * 1024) // 10 MB
            throw new DomainException("File size cannot exceed 10 MB.");

        FileName = fileName.Trim();
        FileUrl = fileUrl.Trim();
        MediaType = mediaType;
        UploadedAt = uploadedAt;
        FileSizeBytes = fileSizeBytes;
    }

    public static MediaReference Create(string fileName, string fileUrl, MediaType mediaType, long fileSizeBytes)
    {
        return new MediaReference(fileName, fileUrl, mediaType, DateTime.UtcNow, fileSizeBytes);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FileName;
        yield return FileUrl;
        yield return MediaType;
        yield return UploadedAt;
        yield return FileSizeBytes;
    }

    public override string ToString()
    {
        return $"{FileName} ({MediaType}, {FileSizeBytes / 1024.0:N2} KB)";
    }
}
