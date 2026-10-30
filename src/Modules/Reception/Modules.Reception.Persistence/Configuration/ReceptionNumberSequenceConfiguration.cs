using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modules.Reception.Persistence.Configuration;

internal sealed class ReceptionNumberSequence
{
    public long SalonId { get; set; }
    public long LastNumber { get; set; }
}

internal sealed class ReceptionNumberSequenceConfiguration
    : IEntityTypeConfiguration<ReceptionNumberSequence>
{
    public void Configure(EntityTypeBuilder<ReceptionNumberSequence> builder)
    {
        builder.ToTable("ReceptionNumberSequences");
        builder.HasKey(sequence => sequence.SalonId);
        builder.Property(sequence => sequence.SalonId).ValueGeneratedNever();
        builder.Property(sequence => sequence.LastNumber).IsRequired();
    }
}
