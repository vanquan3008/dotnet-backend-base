namespace MyProject.Infrastructure.Messaging;

public sealed class MessagingOptions
{
    public const string SectionName = "Messaging";
    public bool Enabled { get; set; }
    public string Host { get; set; } = "localhost";
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int RetryCount { get; set; } = 3;
    public int RetryIntervalSeconds { get; set; } = 2;
    public int ConcurrentMessageLimit { get; set; } = 8;
}
