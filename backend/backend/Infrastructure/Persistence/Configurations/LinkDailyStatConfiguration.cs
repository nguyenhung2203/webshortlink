using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShortlink.Backend.Domain.Entities;

namespace WebShortlink.Backend.Infrastructure.Persistence.Configurations;

public sealed class LinkDailyStatConfiguration : IEntityTypeConfiguration<LinkDailyStat>
{
    public void Configure(EntityTypeBuilder<LinkDailyStat> builder)
    {
        builder.ToTable("LinkDailyStats");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.LinkId, x.StatDate }).IsUnique();
    }
}
