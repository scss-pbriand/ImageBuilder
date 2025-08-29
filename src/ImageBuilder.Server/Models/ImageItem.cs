namespace ImageBuilder.Server.Models;

public sealed record class ImageItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FileName { get; init; } = string.Empty;
    public byte[] Data { get; init; } = Array.Empty<byte>();
}
