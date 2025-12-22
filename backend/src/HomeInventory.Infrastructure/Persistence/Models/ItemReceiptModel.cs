namespace HomeInventory.Infrastructure.Persistence.Models;

public class ItemReceiptModel
{
    public Guid Id { get; init; }
    public Guid ItemId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public string MediaType { get; init; } = string.Empty;
    public DateTime UploadedAt { get; init; }
    public long FileSizeBytes { get; init; }
}
