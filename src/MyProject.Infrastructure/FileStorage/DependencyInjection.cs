using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.FileStorage;

internal static class FileStorageDependencyInjection
{
    public static IServiceCollection AddFileStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(FileStorageOptions.SectionName);
        if (!section.GetValue<bool>(nameof(FileStorageOptions.Enabled)))
        {
            return services;
        }

        var provider = section[nameof(FileStorageOptions.Provider)] ?? "Local";
        services.AddOptions<FileStorageOptions>()
            .Bind(section)
            .Validate(options => options.Provider is "Local" or "S3",
                "FileStorage:Provider must be Local or S3.")
            .Validate(options => options.Provider != "Local" || !string.IsNullOrWhiteSpace(options.LocalRoot),
                "FileStorage:LocalRoot is required for the Local provider.")
            .Validate(options => options.Provider != "S3" || !string.IsNullOrWhiteSpace(options.BucketName),
                "FileStorage:BucketName is required for the S3 provider.")
            .ValidateOnStart();

        if (provider.Equals("S3", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<IAmazonS3>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<FileStorageOptions>>().Value;
                var clientOptions = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(options.Region)
                };
                if (!string.IsNullOrWhiteSpace(options.ServiceUrl))
                {
                    clientOptions.ServiceURL = options.ServiceUrl;
                    clientOptions.ForcePathStyle = true;
                }
                return string.IsNullOrWhiteSpace(options.AccessKey)
                    ? new AmazonS3Client(clientOptions)
                    : new AmazonS3Client(options.AccessKey, options.SecretKey, clientOptions);
            });
            services.AddSingleton<IFileStorageService, S3FileStorageService>();
        }
        else
        {
            services.AddSingleton<IFileStorageService, LocalFileStorageService>();
        }
        return services;
    }
}
