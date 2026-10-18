using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Personnel.Persistence.Configuration;

using global::Modules.Personnel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SalonConfiguration : IEntityTypeConfiguration<Salon>
{
    public void Configure(EntityTypeBuilder<Salon> builder)
    {
        builder.ToTable("Salons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasMany(x => x.Personnels)
            .WithOne(x => x.Salon)
            .HasForeignKey(x => x.SalonId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}