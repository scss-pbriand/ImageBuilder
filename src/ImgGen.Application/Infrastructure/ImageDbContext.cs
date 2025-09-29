using Domain.Images;
using Microsoft.EntityFrameworkCore;

namespace ImgGen.Application.Infrastructure;

/// <summary>
/// EF Core DbContext for image storage and metadata.
/// Separated from Marten document storage for binary data management.
/// </summary>
public class ImageDbContext : DbContext
{
    public ImageDbContext(DbContextOptions<ImageDbContext> options) : base(options)
    {
    }

    public DbSet<ImageData> ImageData { get; set; } = null!;
    public DbSet<ImageMetaData> ImageMetaData { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure schema
        modelBuilder.HasDefaultSchema("images");

        // Configure ImageData entity
        modelBuilder.Entity<ImageData>(entity =>
        {
            entity.ToTable("image_data");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired();

            entity.Property(e => e.Content)
                .HasColumnName("content")
                .IsRequired();

            // One-to-one relationship with ImageMetaData
            entity.HasOne(e => e.MetaData)
                .WithOne(m => m.ImageData)
                .HasForeignKey<ImageMetaData>(m => m.ImageDataId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ImageMetaData entity
        modelBuilder.Entity<ImageMetaData>(entity =>
        {
            entity.ToTable("image_metadata");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired();

            entity.Property(e => e.ImageDataId)
                .HasColumnName("image_data_id")
                .IsRequired();

            entity.Property(e => e.OriginalFileName)
                .HasColumnName("original_file_name")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.MimeType)
                .HasColumnName("mime_type")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.FileSizeBytes)
                .HasColumnName("file_size_bytes")
                .IsRequired();

            entity.Property(e => e.UploadedAt)
                .HasColumnName("uploaded_at")
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(1000);

            // Create indexes for common queries
            entity.HasIndex(e => e.MimeType)
                .HasDatabaseName("ix_image_metadata_mime_type");

            entity.HasIndex(e => e.UploadedAt)
                .HasDatabaseName("ix_image_metadata_uploaded_at");

            entity.HasIndex(e => e.ImageDataId)
                .HasDatabaseName("ix_image_metadata_image_data_id")
                .IsUnique();
        });
    }
}