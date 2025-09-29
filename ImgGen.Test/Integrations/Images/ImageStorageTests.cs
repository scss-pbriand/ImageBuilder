using Domain.Images;
using FluentValidation;
using ImgGen.Application.Infrastructure;
using ImgGen.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace ImgGen.Test.Integrations.Images;

[TestClass]
public class ImageStorageTests
{
    private ServiceProvider? _serviceProvider;
    private ImageDbContext? _context;
    private ImageStorageService? _service;

    [TestInitialize]
    public async Task Setup()
    {
        var services = new ServiceCollection();
        
        // Use in-memory database for testing
        services.AddDbContext<ImageDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                   .ConfigureWarnings(warnings => warnings.Default(WarningBehavior.Ignore)));

        services.AddScoped<ImageStorageService>();
        services.AddScoped<IValidator<ImageMetaData>, ImageMetaDataValidator>();

        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<ImageDbContext>();
        _service = _serviceProvider.GetRequiredService<ImageStorageService>();

        await _context.Database.EnsureCreatedAsync();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
        _serviceProvider?.Dispose();
    }

    [TestMethod]
    public async Task StoreImageAsync_ValidImage_ReturnsMetadataId()
    {
        // Arrange
        var imageContent = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG header
        var fileName = "test.png";
        var mimeType = "image/png";
        var description = "Test image";

        // Act
        var metadataId = await _service!.StoreImageAsync(imageContent, fileName, mimeType, description);

        // Assert
        metadataId.ShouldNotBe(Guid.Empty);

        var metadata = await _service.GetImageMetadataByIdAsync(metadataId);
        metadata.ShouldNotBeNull();
        metadata.OriginalFileName.ShouldBe(fileName);
        metadata.MimeType.ShouldBe(mimeType);
        metadata.FileSizeBytes.ShouldBe(imageContent.Length);
        metadata.Description.ShouldBe(description);
    }

    [TestMethod]
    public async Task StoreImageAsync_InvalidMimeType_ThrowsValidationException()
    {
        // Arrange
        var imageContent = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var fileName = "test.invalid";
        var mimeType = "application/pdf"; // Not supported

        // Act & Assert
        await Should.ThrowAsync<ValidationException>(
            _service!.StoreImageAsync(imageContent, fileName, mimeType));
    }

    [TestMethod]
    public async Task GetImageMetadataAsync_WithPagination_ReturnsCorrectResults()
    {
        // Arrange
        var imageContent = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        
        // Store multiple images
        for (int i = 0; i < 5; i++)
        {
            await _service!.StoreImageAsync(imageContent, $"test{i}.png", "image/png", $"Test image {i}");
        }

        // Act
        var (images, totalCount) = await _service!.GetImageMetadataAsync(page: 1, pageSize: 3);

        // Assert
        totalCount.ShouldBe(5);
        images.Count().ShouldBe(3);
        images.All(i => i.MimeType == "image/png").ShouldBeTrue();
    }

    [TestMethod]
    public async Task GetImageContentAsync_ExistingImage_ReturnsContent()
    {
        // Arrange
        var imageContent = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        var metadataId = await _service!.StoreImageAsync(imageContent, "test.png", "image/png");

        // Act
        var retrievedContent = await _service.GetImageContentAsync(metadataId);

        // Assert
        retrievedContent.ShouldNotBeNull();
        retrievedContent.ShouldBe(imageContent);
    }

    [TestMethod]
    public async Task DeleteImageAsync_ExistingImage_RemovesImageAndMetadata()
    {
        // Arrange
        var imageContent = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        var metadataId = await _service!.StoreImageAsync(imageContent, "test.png", "image/png");

        // Act
        var deleted = await _service.DeleteImageAsync(metadataId);

        // Assert
        deleted.ShouldBeTrue();
        
        var metadata = await _service.GetImageMetadataByIdAsync(metadataId);
        metadata.ShouldBeNull();
        
        var content = await _service.GetImageContentAsync(metadataId);
        content.ShouldBeNull();
    }

    [TestMethod]
    public async Task UpdateImageMetadataAsync_ValidUpdate_UpdatesDescription()
    {
        // Arrange
        var imageContent = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        var metadataId = await _service!.StoreImageAsync(imageContent, "test.png", "image/png", "Original description");
        var newDescription = "Updated description";

        // Act
        var updated = await _service.UpdateImageMetadataAsync(metadataId, newDescription);

        // Assert
        updated.ShouldBeTrue();
        
        var metadata = await _service.GetImageMetadataByIdAsync(metadataId);
        metadata.ShouldNotBeNull();
        metadata.Description.ShouldBe(newDescription);
    }

    [TestMethod]
    public void GetSupportedMimeTypes_ReturnsExpectedTypes()
    {
        // Act
        var supportedTypes = ImageStorageService.GetSupportedMimeTypes();

        // Assert
        supportedTypes.ShouldContain("image/png");
        supportedTypes.ShouldContain("image/jpeg");
        supportedTypes.ShouldContain("image/gif");
        supportedTypes.ShouldContain("image/webp");
    }

    [TestMethod]
    public void IsSupportedMimeType_ValidTypes_ReturnsTrue()
    {
        // Assert
        ImageStorageService.IsSupportedMimeType("image/png").ShouldBeTrue();
        ImageStorageService.IsSupportedMimeType("IMAGE/PNG").ShouldBeTrue(); // Case insensitive
        ImageStorageService.IsSupportedMimeType("image/jpeg").ShouldBeTrue();
        ImageStorageService.IsSupportedMimeType("application/pdf").ShouldBeFalse();
    }
}