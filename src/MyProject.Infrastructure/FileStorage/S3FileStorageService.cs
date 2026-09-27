using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.FileStorage;

internal sealed class S3FileStorageService(
    IAmazonS3 client,
    IOptions<FileStorageOptions> options) : IFileStorageService
{
    public async Task SaveAsync(
        string key,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = options.Value.BucketName,
            Key = key,
            InputStream = content,
            ContentType = contentType
        };
        await client.PutObjectAsync(request, cancellationToken);
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        => client.DeleteObjectAsync(options.Value.BucketName, key, cancellationToken);
}
