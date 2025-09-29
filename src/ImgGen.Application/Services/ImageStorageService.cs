using Domain.Images;
using FluentValidation;
using ImgGen.Application.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ImgGen.Application.Services;

/// <summary>
/// Service for managing image storage and metadata using EF Core.
/// Provides optimized queries for metadata with lazy loading of binary data.
/// </summary>
public class ImageStorageService
{
    private readonly ImageDbContext _context;
    private readonly IValidator<ImageMetaData> _validator;

    public ImageStorageService(ImageDbContext context, IValidator<ImageMetaData> validator)
    {
        _context = context;
        _validator = validator;
    }

    /// <summary>
    /// Store an image with its metadata. Transactional operation.
    /// </summary>
    public async Task<Guid> StoreImageAsync(byte[] imageContent, string originalFileName, string mimeType, string? description = null, CancellationToken cancellationToken = default)
    {
        // Create image data record
        var imageData = new ImageData
        {
            Id = Guid.NewGuid(),
            Content = imageContent
        };

        // Create metadata record
        var metadata = new ImageMetaData
        {
            Id = Guid.NewGuid(),
            ImageDataId = imageData.Id,
            OriginalFileName = originalFileName,
            MimeType = mimeType.ToLowerInvariant(),
            FileSizeBytes = imageContent.Length,
            Description = description,
            UploadedAt = DateTime.UtcNow
        };

        // Validate metadata
        var validationResult = await _validator.ValidateAsync(metadata, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Try to use transaction if supported, otherwise save directly
        if (_context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            try
            {
                // Save to database
                _context.ImageData.Add(imageData);
                _context.ImageMetaData.Add(metadata);
                
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return metadata.Id;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        else
        {
            // For in-memory database, save without transaction
            _context.ImageData.Add(imageData);
            _context.ImageMetaData.Add(metadata);
            
            await _context.SaveChangesAsync(cancellationToken);
            return metadata.Id;
        }
    }

    /// <summary>
    /// Get paginated metadata without loading binary content (optimized for browsing)
    /// </summary>
    public async Task<(IEnumerable<ImageMetaData> Images, int TotalCount)> GetImageMetadataAsync(
        int page = 1, 
        int pageSize = 20, 
        string? mimeTypeFilter = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var query = _context.ImageMetaData.AsQueryable();

        // Apply MIME type filter if specified
        if (!string.IsNullOrWhiteSpace(mimeTypeFilter))
        {
            query = query.Where(m => m.MimeType == mimeTypeFilter.ToLowerInvariant());
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Get paginated results ordered by upload date (newest first)
        var images = await query
            .OrderByDescending(m => m.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (images, totalCount);
    }

    /// <summary>
    /// Get image metadata by ID without loading binary content
    /// </summary>
    public async Task<ImageMetaData?> GetImageMetadataByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ImageMetaData
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    /// <summary>
    /// Get image binary content by metadata ID
    /// </summary>
    public async Task<byte[]?> GetImageContentAsync(Guid metadataId, CancellationToken cancellationToken = default)
    {
        var metadata = await _context.ImageMetaData
            .Include(m => m.ImageData)
            .FirstOrDefaultAsync(m => m.Id == metadataId, cancellationToken);

        return metadata?.ImageData?.Content;
    }

    /// <summary>
    /// Get full image with metadata and content
    /// </summary>
    public async Task<(ImageMetaData? Metadata, byte[]? Content)> GetFullImageAsync(Guid metadataId, CancellationToken cancellationToken = default)
    {
        var metadata = await _context.ImageMetaData
            .Include(m => m.ImageData)
            .FirstOrDefaultAsync(m => m.Id == metadataId, cancellationToken);

        return (metadata, metadata?.ImageData?.Content);
    }

    /// <summary>
    /// Update image metadata (does not modify binary content)
    /// </summary>
    public async Task<bool> UpdateImageMetadataAsync(Guid id, string? description = null, CancellationToken cancellationToken = default)
    {
        var metadata = await GetImageMetadataByIdAsync(id, cancellationToken);
        if (metadata == null) return false;

        if (description != null)
        {
            metadata.Description = description;
        }

        // Validate changes
        var validationResult = await _validator.ValidateAsync(metadata, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Delete image and its metadata (transactional)
    /// </summary>
    public async Task<bool> DeleteImageAsync(Guid metadataId, CancellationToken cancellationToken = default)
    {
        var metadata = await _context.ImageMetaData
            .Include(m => m.ImageData)
            .FirstOrDefaultAsync(m => m.Id == metadataId, cancellationToken);

        if (metadata == null) return false;

        // Try to use transaction if supported, otherwise save directly
        if (_context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            try
            {
                // EF Core will handle cascade delete due to the relationship configuration
                _context.ImageMetaData.Remove(metadata);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        else
        {
            // For in-memory database, save without transaction
            _context.ImageMetaData.Remove(metadata);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    /// <summary>
    /// Get supported MIME types
    /// </summary>
    public static IEnumerable<string> GetSupportedMimeTypes()
    {
        return new[]
        {
            "image/jpeg",
            "image/jpg", 
            "image/png",
            "image/gif",
            "image/bmp",
            "image/webp",
            "image/tiff"
        };
    }

    /// <summary>
    /// Check if a MIME type is supported
    /// </summary>
    public static bool IsSupportedMimeType(string mimeType)
    {
        return GetSupportedMimeTypes().Contains(mimeType.ToLowerInvariant());
    }
}