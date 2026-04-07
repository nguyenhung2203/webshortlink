using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShortlink.Backend.Domain.Entities;

namespace WebShortlink.Backend.Infrastructure.Persistence.Configurations;

public sealed class ClickEventConfiguration : IEntityTypeConfiguration<WebShortlink.Backend.Domain.Entities.ClickEvent>
{
    public void Configure(EntityTypeBuilder<WebShortlink.Backend.Domain.Entities.ClickEvent> builder)
    {
        builder.ToTable("ClickEvents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IpAddress).HasMaxLength(64).IsRequired();
        builder.Property(x => x.MaskedIp).HasMaxLength(64).IsRequired();
        builder.Property(x => x.CountryCode).HasMaxLength(8);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.DeviceType).HasMaxLength(50);
        builder.Property(x => x.Browser).HasMaxLength(100);
        builder.Property(x => x.OperatingSystem).HasMaxLength(100);
        builder.Property(x => x.Referrer).HasMaxLength(1000);
        builder.Property(x => x.UserAgent).HasMaxLength(1000);
        builder.Property(x => x.FingerprintHash).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => new { x.LinkId, x.ClickedAtUtc });
        builder.HasIndex(x => x.ClickedAtUtc);
    }
}
