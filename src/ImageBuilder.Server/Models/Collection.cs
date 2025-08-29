namespace ImageBuilder.Server.Models;

public sealed record class Collection
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<Category> Categories { get; init; } = new();
}
