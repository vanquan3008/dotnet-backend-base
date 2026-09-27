namespace MyProject.Infrastructure.Caching;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "src:";
    public int DefaultExpirationMinutes { get; set; } = 30;
}
