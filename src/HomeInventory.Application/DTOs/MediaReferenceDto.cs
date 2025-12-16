namespace HomeInventory.Application.DTOs;

public class MediaReferenceDto
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public long FileSizeBytes { get; set; }
}
