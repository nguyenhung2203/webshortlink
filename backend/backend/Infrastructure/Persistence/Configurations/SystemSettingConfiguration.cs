using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShortlink.Backend.Domain.Entities;

namespace WebShortlink.Backend.Infrastructure.Persistence.Configurations;

public sealed class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SettingKey).HasMaxLength(100).IsRequired();
        builder.Property(x => x.SettingValue).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.GroupName).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => x.SettingKey).IsUnique();
    }
}
