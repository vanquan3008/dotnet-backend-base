namespace MyProject.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task SaveAsync(
        string key,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}
