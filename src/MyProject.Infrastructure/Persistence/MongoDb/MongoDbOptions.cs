namespace MyProject.Infrastructure.Persistence.MongoDb;

public sealed class MongoDbOptions
{
    public const string SectionName = "MongoDb";
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}
