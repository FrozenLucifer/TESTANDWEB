using Microsoft.EntityFrameworkCore;
using Shared;

namespace EfCoreApp;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
                               ?? "Host=localhost;Database=benchmark_db;Username=postgres;Password=password;Port=5432";
        optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>();
        modelBuilder.Entity<Category>();
        modelBuilder.Entity<Order>();
        
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.CategoryId);
            entity.HasIndex(p => p.CreatedAt);
        });
        
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(o => o.ProductId);
            entity.HasIndex(o => o.CreatedAt);
        });
    }
}