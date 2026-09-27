using Microsoft.Extensions.Options;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.FileStorage;

internal sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string root;

    public LocalFileStorageService(IOptions<FileStorageOptions> options)
    {
        root = Path.GetFullPath(options.Value.LocalRoot);
        Directory.CreateDirectory(root);
    }

    public async Task SaveAsync(
        string key,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var path = ResolvePath(key);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var file = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            81920,
            FileOptions.Asynchronous);
        await content.CopyToAsync(file, cancellationToken);
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var path = ResolvePath(key);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        return Task.CompletedTask;
    }

    private string ResolvePath(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        var path = Path.GetFullPath(Path.Combine(root, key));
        var rootPrefix = root.EndsWith(Path.DirectorySeparatorChar)
            ? root
            : root + Path.DirectorySeparatorChar;
        if (!path.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("File key must stay inside the configured storage root.", nameof(key));
        }
        return path;
    }
}
