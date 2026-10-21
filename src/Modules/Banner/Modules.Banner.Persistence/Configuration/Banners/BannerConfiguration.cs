namespace Modules.Banner.Persistence.Configuration.Banners;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Banner.Domain.Entities;
using Modules.Banner.Domain.Enums;

public sealed class BannerConfiguration : IEntityTypeConfiguration<BannerEntity>
{
    public void Configure(EntityTypeBuilder<BannerEntity> builder)
    {
        builder.ToTable("Banners");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasDefaultValue(BannerType.None);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Url)
            .HasMaxLength(500);

        builder.Property(x => x.Order)
            .IsRequired();

    }
}
