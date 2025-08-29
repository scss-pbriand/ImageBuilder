namespace ImageBuilder.Server.Models;

public sealed record class Category
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public int Probability { get; init; }
    public List<ImageItem> Images { get; init; } = new();
}
