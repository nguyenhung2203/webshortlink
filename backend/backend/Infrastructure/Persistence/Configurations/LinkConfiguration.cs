using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShortlink.Backend.Domain.Entities;

namespace WebShortlink.Backend.Infrastructure.Persistence.Configurations;

public sealed class LinkConfiguration : IEntityTypeConfiguration<Link>
{
    public void Configure(EntityTypeBuilder<Link> builder)
    {
        builder.ToTable("Links");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Slug).HasMaxLength(64).IsRequired();
        builder.Property(x => x.OriginalUrl).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Tag).HasMaxLength(100);
        builder.Property(x => x.PasswordHash).HasMaxLength(500);
        builder.Property(x => x.OgTitle).HasMaxLength(200);
        builder.Property(x => x.OgDescription).HasMaxLength(500);
        builder.Property(x => x.OgImageUrl).HasMaxLength(2048);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.DomainId, x.Slug }).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
    }
}
