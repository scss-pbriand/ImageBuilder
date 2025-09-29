# EF Core Image Storage Implementation

This document describes the EF Core-based image storage solution implemented for Julie's Modular Art Generator.

## Overview

The image storage system uses Entity Framework Core with PostgreSQL to store image binary data and metadata separately, allowing for optimized queries and lazy loading of binary content.

## Architecture

### Entity Classes

- **`ImageData`**: Stores the binary content of images (`byte[]`)
- **`ImageMetaData`**: Stores metadata about images including:
  - Original filename
  - MIME type
  - File size in bytes
  - Upload date
  - Optional description

### Database Schema

The system creates tables in the `images` schema:

- `images.image_data`: Binary content storage
- `images.image_metadata`: Metadata with foreign key to image_data

### Key Features

1. **Separated Storage**: Binary data and metadata are stored separately for optimized queries
2. **Lazy Loading**: Binary content is only loaded when explicitly requested
3. **Validation**: File type and size validation (max 10MB, supported formats: JPEG, PNG, GIF, BMP, WebP, TIFF)
4. **Pagination**: Optimized for browsing 10-20 images at a time
5. **Transactional**: All operations are wrapped in database transactions
6. **Cascade Delete**: Deleting metadata automatically removes associated binary data

## Usage Examples

### Service Registration

```csharp
// In Program.cs
builder.Services.AddImageStorage(builder.Configuration);
```

### Basic Operations

```csharp
// Inject the service
public class ImageController : Controller
{
    private readonly ImageStorageService _imageService;
    
    public ImageController(ImageStorageService imageService)
    {
        _imageService = imageService;
    }
    
    // Store an image
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (!ImageStorageService.IsSupportedMimeType(file.ContentType))
        {
            return BadRequest("Unsupported file type");
        }
        
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        
        var metadataId = await _imageService.StoreImageAsync(
            stream.ToArray(),
            file.FileName,
            file.ContentType,
            "Uploaded image"
        );
        
        return Ok(new { Id = metadataId });
    }
    
    // Get paginated image list (metadata only)
    public async Task<IActionResult> List(int page = 1, int pageSize = 20)
    {
        var (images, totalCount) = await _imageService.GetImageMetadataAsync(page, pageSize);
        
        return Ok(new { Images = images, TotalCount = totalCount });
    }
    
    // Get image content for display
    public async Task<IActionResult> Image(Guid id)
    {
        var (metadata, content) = await _imageService.GetFullImageAsync(id);
        
        if (metadata == null || content == null)
            return NotFound();
            
        return File(content, metadata.MimeType, metadata.OriginalFileName);
    }
    
    // Delete an image
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _imageService.DeleteImageAsync(id);
        
        if (!deleted)
            return NotFound();
            
        return Ok();
    }
}
```

### Advanced Queries

```csharp
// Filter by MIME type
var (pngImages, count) = await _imageService.GetImageMetadataAsync(
    page: 1, 
    pageSize: 10, 
    mimeTypeFilter: "image/png"
);

// Get only metadata for efficient browsing
var metadata = await _imageService.GetImageMetadataByIdAsync(imageId);

// Get binary content separately when needed
var content = await _imageService.GetImageContentAsync(imageId);

// Update metadata
await _imageService.UpdateImageMetadataAsync(imageId, "New description");
```

## Database Migrations

To apply the initial migration:

```bash
cd src/ImgGen.Application
dotnet ef database update --context ImageDbContext
```

To create new migrations:

```bash
dotnet ef migrations add MigrationName --context ImageDbContext
```

## Configuration

The system uses the same PostgreSQL connection string as the main application (`DefaultConnection`). The EF Core context is configured with:

- Lazy loading proxies for navigation properties
- Retry on failure (3 attempts, 30-second delay)
- Detailed errors in development
- Migrations history stored in `images.__EFMigrationsHistory`

## Performance Considerations

1. **Pagination**: Always use pagination when listing images
2. **Lazy Loading**: Binary content is only loaded when accessed via navigation properties
3. **Indexes**: The system creates indexes on `mime_type`, `uploaded_at`, and `image_data_id`
4. **Connection Pooling**: Uses the same connection pool as the main application

## Validation Rules

- **File Size**: Maximum 10MB per image
- **MIME Types**: Only image types are supported (JPEG, PNG, GIF, BMP, WebP, TIFF)
- **Filename**: Maximum 255 characters
- **Description**: Maximum 1000 characters

## Error Handling

The service throws:
- `ValidationException` for invalid data (file type, size, etc.)
- `InvalidOperationException` for database errors
- Standard EF Core exceptions for other database issues

## Testing

The implementation includes comprehensive unit tests using EF Core's in-memory database provider. Tests cover:

- Image storage and retrieval
- Validation (file types, sizes)
- Pagination
- Metadata updates
- Deletion (cascade)
- Error scenarios

Run tests with:
```bash
dotnet test --filter "ImageStorageTests"
```

## Integration with Existing System

This EF Core image storage system supplements the existing Marten document store:

- **Marten**: Continues to handle user data, configuration, and other documents
- **EF Core**: Handles binary image storage and metadata
- **PostgreSQL**: Single database with separate schemas (`imggen` for Marten, `images` for EF Core)

Both systems can coexist and use the same connection string and database instance.