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
    public DbSet<Keyboard> Keyboards { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!; // base table for TPT

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map base type to Products table (TPT)
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.ImageFileName).HasMaxLength(500);
            entity.Property(e => e.Weight).HasMaxLength(100);
        });

        // Derived Headphone table with specific columns
        modelBuilder.Entity<Headphone>(entity =>
        {
            entity.ToTable("Headphones");
            entity.Property(e => e.Manufacturer).HasMaxLength(200);
            entity.Property(e => e.Color).HasMaxLength(100);
            entity.Property(e => e.Type).HasMaxLength(100);
            entity.Property(e => e.BatteryLife).HasMaxLength(100);
            entity.Property(e => e.NoiseCancellationType).HasMaxLength(100);
        });

        // Derived Keyboard table with specific columns
        modelBuilder.Entity<Keyboard>(entity =>
        {
            entity.ToTable("Keyboards");
            // IsMechanical is a simple bool, no extra config required
        });
    }
}