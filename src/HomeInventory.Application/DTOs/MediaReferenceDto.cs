namespace HomeInventory.Application.DTOs;

public class MediaReferenceDto
{
    public string FileName { get; init; } = string.Empty;
    
    public string FileUrl { get; init; } = string.Empty;
    
    public string MediaType { get; init; } = string.Empty;
    
    public DateTime UploadedAt { get; init; }
    
    public long FileSizeBytes { get; init; }
}
