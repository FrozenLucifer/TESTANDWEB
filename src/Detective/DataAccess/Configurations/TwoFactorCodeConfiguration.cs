using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations;

public class TwoFactorCodeConfiguration : IEntityTypeConfiguration<TwoFactorCodeDb>
{
    public void Configure(EntityTypeBuilder<TwoFactorCodeDb> builder)
    {
        builder.HasKey(a => a.Username);
        builder.Property(a => a.Code).IsRequired();
        builder.Property(a => a.ExpiresAt).IsRequired();
    }
}