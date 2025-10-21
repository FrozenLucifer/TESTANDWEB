using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<ContactDb>
{
    public void Configure(EntityTypeBuilder<ContactDb> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.PersonId, a.Type, a.Info })
            .IsUnique();

        builder.Property(a => a.Type).IsRequired().HasConversion<string>();
        builder.Property(a => a.Info).IsRequired();

        builder.HasOne(a => a.Person).WithMany(a => a.Contacts).HasForeignKey(a => a.PersonId);
    }
}