using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ImgGen.Application.Infrastructure;

/// <summary>
/// Design-time factory for EF Core migrations
/// </summary>
public class ImageDbContextFactory : IDesignTimeDbContextFactory<ImageDbContext>
{
    public ImageDbContext CreateDbContext(string[] args)
    {
        // Look for appsettings in the web project directory
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "ImgGen.Mud"));
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ImageDbContext>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Database=imggen_dev;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.MigrationsHistoryTable("__EFMigrationsHistory", "images");
        });

        return new ImageDbContext(optionsBuilder.Options);
    }
}