using DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace DataAccess;

public class Context : DbContext
{
    public DbSet<PersonDb> Persons { get; set; }
    public DbSet<ContactDb> Contacts { get; set; }
    public DbSet<DocumentDb> Documents { get; set; }
    public DbSet<PropertyDb> Properties { get; set; }
    public DbSet<RelationshipDb> Relationships { get; set; }
    public DbSet<UserDb> Users { get; set; }

    public DbSet<CharacteristicDb> Characteristics { get; set; }
    public DbSet<TwoFactorCodeDb> TwoFactorCodes { get; set; }

    public Context()
    {
    }

    public Context(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new RelationshipConfiguration());
        modelBuilder.ApplyConfiguration(new ContactConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentConfiguration());
        modelBuilder.ApplyConfiguration(new PropertyConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CharacteristicConfiguration());
        modelBuilder.ApplyConfiguration(new TwoFactorCodeConfiguration());
    }
}