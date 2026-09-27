namespace MyProject.Infrastructure.FileStorage;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";
    public bool Enabled { get; set; }
    public string Provider { get; set; } = "Local"; // Local, S3, AzureBlob, etc.
    public string LocalRoot { get; set; } = "storage";
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = "ap-southeast-1";
    public string? ServiceUrl { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
}
