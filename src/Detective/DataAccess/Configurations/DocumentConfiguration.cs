using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<DocumentDb>
{
    public void Configure(EntityTypeBuilder<DocumentDb> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Type).IsRequired().HasConversion<string>();
        builder.Property(a => a.Payload).HasColumnType("json").IsRequired();

        builder.HasOne(a => a.Person).WithMany(a => a.Documents).HasForeignKey(a => a.PersonId);
    }
}