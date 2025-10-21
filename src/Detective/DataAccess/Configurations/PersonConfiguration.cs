using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<PersonDb>
{
    public void Configure(EntityTypeBuilder<PersonDb> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever().IsRequired();
        builder.HasIndex(a => a.Id).IsUnique();

        builder.Property(a => a.FullName);
        builder.Property(a => a.Sex).HasConversion<string>();
        builder.Property(a => a.BirthDate);

        builder.HasMany(a => a.Documents)
            .WithOne(a => a.Person)
            .HasForeignKey(a => a.PersonId);

        builder.HasMany(a => a.Contacts)
            .WithOne(a => a.Person)
            .HasForeignKey(a => a.PersonId);

        builder.HasMany(a => a.Properties)
            .WithOne(a => a.Person)
            .HasForeignKey(a => a.PersonId);

        builder.HasMany(a => a.RelationshipsAsPerson1)
            .WithOne(a => a.Person1)
            .HasForeignKey(a => a.Person1Id).IsRequired();

        builder.HasMany(a => a.RelationshipsAsPerson2)
            .WithOne(a => a.Person2)
            .HasForeignKey(a => a.Person2Id).IsRequired();
    }
}