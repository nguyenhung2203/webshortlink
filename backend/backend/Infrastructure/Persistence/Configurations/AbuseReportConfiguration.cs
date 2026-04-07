using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShortlink.Backend.Domain.Entities;

namespace WebShortlink.Backend.Infrastructure.Persistence.Configurations;

public sealed class AbuseReportConfiguration : IEntityTypeConfiguration<AbuseReport>
{
    public void Configure(EntityTypeBuilder<AbuseReport> builder)
    {
        builder.ToTable("AbuseReports");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reason).HasMaxLength(500).IsRequired();
        builder.HasIndex(x => new { x.LinkId, x.IsResolved });
    }
}
