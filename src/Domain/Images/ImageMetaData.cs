using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Domain.Images;

/// <summary>
/// Stores metadata about images without the binary content.
/// Optimized for lightweight queries and pagination.
/// </summary>
public class ImageMetaData
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to ImageData containing the binary content
    /// </summary>
    [Required]
    public Guid ImageDataId { get; set; }

    /// <summary>
    /// Original filename when uploaded
    /// </summary>
    [Required]
    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME type (e.g., "image/png", "image/jpeg")
    /// </summary>
    [Required]
    [StringLength(100)]
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    [Required]
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// When the image was uploaded
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional description/tags
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Navigation property to binary data - lazy loaded
    /// </summary>
    public virtual ImageData? ImageData { get; set; }
}

/// <summary>
/// Validator for ImageMetaData with file type and size validation
/// </summary>
public class ImageMetaDataValidator : AbstractValidator<ImageMetaData>
{
    private static readonly string[] AllowedMimeTypes = [
        "image/jpeg",
        "image/jpg", 
        "image/png",
        "image/gif",
        "image/bmp",
        "image/webp",
        "image/tiff"
    ];

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

    public ImageMetaDataValidator()
    {
        RuleFor(x => x.ImageDataId)
            .NotEmpty().WithMessage("ImageDataId is required.");

        RuleFor(x => x.OriginalFileName)
            .NotEmpty().WithMessage("Original filename is required.")
            .MaximumLength(255).WithMessage("Filename must not exceed 255 characters.");

        RuleFor(x => x.MimeType)
            .NotEmpty().WithMessage("MIME type is required.")
            .Must(BeAllowedMimeType).WithMessage($"MIME type must be one of: {string.Join(", ", AllowedMimeTypes)}");

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0).WithMessage("File size must be greater than 0.")
            .LessThanOrEqualTo(MaxFileSizeBytes).WithMessage($"File size must not exceed {MaxFileSizeBytes / (1024 * 1024)}MB.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
    }

    private static bool BeAllowedMimeType(string mimeType)
    {
        return AllowedMimeTypes.Contains(mimeType.ToLowerInvariant());
    }
}