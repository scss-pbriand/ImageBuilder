using System.ComponentModel.DataAnnotations;

namespace Domain.Images;

/// <summary>
/// Stores the binary content of images in the database via EF Core.
/// Separated from metadata to support lazy loading.
/// </summary>
public class ImageData
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Binary content of the image
    /// </summary>
    [Required]
    public byte[] Content { get; set; } = [];

    /// <summary>
    /// Navigation property to metadata - lazy loaded
    /// </summary>
    public virtual ImageMetaData? MetaData { get; set; }
}