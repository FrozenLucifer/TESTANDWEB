using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<PropertyDb>
{
    public void Configure(EntityTypeBuilder<PropertyDb> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).IsRequired();
        builder.Property(a => a.Cost);

        builder.HasOne(a => a.Person).WithMany(a => a.Properties).HasForeignKey(a => a.PersonId);
    }
}