using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class CharacteristicConfiguration : IEntityTypeConfiguration<CharacteristicDb>
{
    public void Configure(EntityTypeBuilder<CharacteristicDb> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.PersonId)
            .IsRequired();

        builder.Property(c => c.AuthorUsername)
            .IsRequired();

        builder.Property(c => c.Appearance)
            .HasMaxLength(1000);

        builder.Property(c => c.Personality)
            .HasMaxLength(1000);

        builder.Property(c => c.MedicalConditions)
            .HasMaxLength(1000);

        builder.HasOne(c => c.Person)
            .WithMany(p => p.Characteristics)
            .HasForeignKey(c => c.PersonId);

        builder.HasOne(c => c.Author)
            .WithMany(u => u.Characteristics)
            .HasPrincipalKey(u => u.Username)
            .HasForeignKey(c => c.AuthorUsername);
    }
}