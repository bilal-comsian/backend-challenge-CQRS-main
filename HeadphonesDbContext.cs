csharp headphones-market.core.Api/Data/HeadphonesDbContext.cs
using Microsoft.EntityFrameworkCore;
using headphones_market.core.Api.Model;

namespace headphones_market.core.Api.Data;

public class HeadphonesDbContext : DbContext
{
    public HeadphonesDbContext(DbContextOptions<HeadphonesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Headphone> Headphones { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Headphone>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Manufacturer).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            // other property configurations can be added as needed
        });
    }
}