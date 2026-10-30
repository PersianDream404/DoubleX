using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Persistence.Configuration;

public sealed class PersonnelCommissionOutboxConfiguration
    : IEntityTypeConfiguration<PersonnelCommissionOutbox>
{
    public void Configure(EntityTypeBuilder<PersonnelCommissionOutbox> builder)
    {
        builder.ToTable("PersonnelCommissionOutbox");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.EventId).IsRequired();
        builder.Property(item => item.OutboxStatus).IsRequired();
        builder.Property(item => item.LastError).HasMaxLength(2000);
        builder.HasIndex(item => new { item.ReceptionId, item.ReceptionServiceId }).IsUnique();
        builder.HasIndex(item => new { item.OutboxStatus, item.NextAttemptAt, item.Id });
        builder.HasIndex(item => item.EventId).IsUnique();
    }
}
