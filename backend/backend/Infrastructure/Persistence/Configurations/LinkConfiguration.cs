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
        builder.Property(x => x.WrapperTitle).HasMaxLength(200);
        builder.Property(x => x.WrapperDescription).HasMaxLength(1000);
        builder.Property(x => x.WrapperImageUrl).HasMaxLength(2048);
        builder.Property(x => x.ContinueButtonText).HasMaxLength(120);
        builder.Property(x => x.WarningText).HasMaxLength(500);
        builder.Property(x => x.WrapperTheme).HasMaxLength(50);
        builder.Property(x => x.BrandName).HasMaxLength(120);
        builder.Property(x => x.BrandLogoUrl).HasMaxLength(2048);
        builder.Property(x => x.CtaTitle).HasMaxLength(200);
        builder.Property(x => x.CtaDescription).HasMaxLength(500);
        builder.Property(x => x.CtaButtonText).HasMaxLength(120);
        builder.Property(x => x.CtaButtonUrl).HasMaxLength(2048);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.DomainId, x.Slug }).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.CreatedAtUtc });
    }
}
