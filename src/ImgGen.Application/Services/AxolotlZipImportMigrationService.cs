using Domain.Images;
using ImgGen.Application.Infrastructure;
using Marten;
using System.IO.Compression;
using Humanizer;

namespace ImgGen.Application.Services;

public class AxolotlZipImportMigrationService
{
    private const string MigrationName = "AxolotlZipImport";
    private readonly string _zipPath;

    private readonly IDocumentSession _session;
    private readonly ImageStorageService _imageStorageService;

    public AxolotlZipImportMigrationService(ImageStorageService imageStorageService, IDocumentSession session, string zipPath)
    {
        _zipPath = zipPath;
        _session = session;
        _imageStorageService = imageStorageService;
    }

    public async Task RunMigrationIfNeededAsync(CancellationToken cancellationToken = default)
    {
        var migration = await _session.Query<DbMigrationInfo>()
            .FirstOrDefaultAsync(m => m.MigrationName == MigrationName, cancellationToken);

        if (migration != null)
        {
            // Migration already applied
            return;
        }

        if (!File.Exists(_zipPath))
            throw new FileNotFoundException($"Zip file not found: {_zipPath}");

        var tempExtractDir = Path.Combine(Path.GetTempPath(), $"axolotl_import_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempExtractDir);
        ZipFile.ExtractToDirectory(_zipPath, tempExtractDir);



        // Process each folder as a category
        foreach (var categoryDir in Directory.GetDirectories(tempExtractDir))
        {
            var categoryName = Path.GetFileName(categoryDir).Humanize(LetterCasing.Title);
            var category = new ImageCategory
            {
                Id = Guid.NewGuid(),
                Name = categoryName,
                ProbabilityWeight = 1.0,
                InsertionOrder = 0,
                ImageAssets = new List<ImageAsset>()
            };

            foreach (var imageFile in Directory.GetFiles(categoryDir))
            {
                var imageTypeName = HumanizeName(Path.GetFileNameWithoutExtension(imageFile));
                var asset = new ImageAsset
                {
                    Id = Guid.NewGuid(),
                    Name = imageTypeName,
                    FileName = Path.GetFileName(imageFile),
                    MimeType = GetMimeType(imageFile),
                    Content = await File.ReadAllBytesAsync(imageFile, cancellationToken),
                    CreatedAt = DateTime.UtcNow
                };
                category.ImageAssets.Add(asset);
            }

            // Add category to context
            // You may need to adjust this to fit your domain model
            // For now, just add to context if you have a DbSet<ImageCategory>
            // _context.ImageCategories.Add(category);
        }

        // Save migration info
        _context.DbMigrationInfos.Add(new DbMigrationInfo
        {
            MigrationName = MigrationName,
            AppliedAtUtc = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(cancellationToken);

        // Clean up temp directory
        Directory.Delete(tempExtractDir, true);
    }

    private static string HumanizeName(string name)
    {
        // Split camel case, underscores, hyphens, and capitalize each word
        var words = System.Text.RegularExpressions.Regex
            .Replace(name, "([a-z])([A-Z])", "$1 $2")
            .Replace("_", " ")
            .Replace("-", " ")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(" ", words.Select(w => char.ToUpper(w[0]) + w.Substring(1)));
    }

    private static string GetMimeType(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".tiff" => "image/tiff",
            _ => "application/octet-stream"
        };
    }
}
