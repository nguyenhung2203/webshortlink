using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShortlink.Backend.Domain.Entities;

namespace WebShortlink.Backend.Infrastructure.Persistence.Configurations;

public sealed class LinkHourlyStatConfiguration : IEntityTypeConfiguration<LinkHourlyStat>
{
    public void Configure(EntityTypeBuilder<LinkHourlyStat> builder)
    {
        builder.ToTable("LinkHourlyStats");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.LinkId, x.HourBucketUtc }).IsUnique();
    }
}
