namespace MyProject.Api.Configuration;

public sealed class CorsOptions
{
    public const string SectionName = "Api:Cors";
    public bool Enabled { get; set; }
    public string[] AllowedOrigins { get; set; } = [];
}

public sealed class RateLimitingOptions
{
    public const string SectionName = "Api:RateLimiting";
    public bool Enabled { get; set; } = true;
    public int PermitLimit { get; set; } = 100;
    public int WindowSeconds { get; set; } = 60;
}

public sealed class ApiRequestTimeoutOptions
{
    public const string SectionName = "Api:RequestTimeout";
    public bool Enabled { get; set; } = true;
    public int Seconds { get; set; } = 30;
}
