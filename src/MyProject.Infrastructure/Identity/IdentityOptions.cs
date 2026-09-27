namespace MyProject.Infrastructure.Identity;

public sealed class IdentityOptions
{
    public const string SectionName = "Identity";
    public bool Enabled { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 60;
}
