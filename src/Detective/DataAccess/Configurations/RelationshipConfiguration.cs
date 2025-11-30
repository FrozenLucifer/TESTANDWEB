using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class RelationshipConfiguration : IEntityTypeConfiguration<RelationshipDb>
{
    public void Configure(EntityTypeBuilder<RelationshipDb> builder)
    {
        builder.Property(a => a.Person1Id).IsRequired();
        builder.Property(a => a.Person2Id).IsRequired();

        builder.HasKey(a => new { a.Person1Id, a.Person2Id });

        builder.Property(a => a.Type).IsRequired().HasConversion<string>();
    }
}