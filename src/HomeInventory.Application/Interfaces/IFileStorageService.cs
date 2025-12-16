namespace HomeInventory.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<Stream> GetFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    string GetFileUrl(string fileName);
}
