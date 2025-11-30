using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserDb>
{
    public void Configure(EntityTypeBuilder<UserDb> builder)
    {
        builder.HasKey(a => a.Username);
        
        builder.HasAlternateKey(a=>a.Email);

        builder.Property(a => a.Password).IsRequired();

        builder.Property(a => a.Type).IsRequired().HasConversion<string>();
    }
}