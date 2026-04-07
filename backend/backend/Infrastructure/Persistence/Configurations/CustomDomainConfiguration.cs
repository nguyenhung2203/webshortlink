using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShortlink.Backend.Domain.Entities;

namespace WebShortlink.Backend.Infrastructure.Persistence.Configurations;

public sealed class CustomDomainConfiguration : IEntityTypeConfiguration<CustomDomain>
{
    public void Configure(EntityTypeBuilder<CustomDomain> builder)
    {
        builder.ToTable("Domains");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Host).HasMaxLength(255).IsRequired();
        builder.Property(x => x.VerificationToken).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.Host).IsUnique();
    }
}
