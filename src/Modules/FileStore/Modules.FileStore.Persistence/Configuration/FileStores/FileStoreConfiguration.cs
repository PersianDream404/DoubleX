namespace Modules.Product.Persistence.Configuration.FileStores;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.FileStore.Domain.Enums;

public sealed class FileStoreConfiguration
    : IEntityTypeConfiguration<Modules.FileStore.Domain.Entities.FileStores.FileStore>
{
    public void Configure(EntityTypeBuilder<Modules.FileStore.Domain.Entities.FileStores.FileStore> builder)
    {
        builder.ToTable("FileStores");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.StoredFileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.FileExtension)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.FilePath)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.FileStoreCategory)
            .HasDefaultValue(FileStoreCategory.None);

        builder.Property(x => x.FileProvider)
            .HasDefaultValue(FileProvider.Local);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.UploadDate)
              .HasDefaultValueSql("GETDATE()");

        builder.HasIndex(x => x.StoredFileName)
            .IsUnique();

        builder.HasIndex(x => x.FilePath);
    }
}
