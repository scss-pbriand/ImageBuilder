namespace ImgGen.Application.Infrastructure;

public class DbMigrationInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string MigrationName { get; set; } = string.Empty;
    public DateTime AppliedAtUtc { get; set; }
}