using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImageBuilder.Server.Models;

namespace ImageBuilder.Server.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ImageItem> Images => Set<ImageItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>()
            .Property(c => c.Probability)
            .HasDefaultValue(0);
    }
}
