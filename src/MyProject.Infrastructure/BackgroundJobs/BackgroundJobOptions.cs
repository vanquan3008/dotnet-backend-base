namespace MyProject.Infrastructure.BackgroundJobs;

public sealed class BackgroundJobOptions
{
    public const string SectionName = "BackgroundJobs";
    public bool Enabled { get; set; }
    public string Queue { get; set; } = "default";
}
