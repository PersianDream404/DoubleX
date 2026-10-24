using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerEntity = Modules.Customer.Domain.Entities.Customer;
using SalonEntity = Modules.Customer.Persistence.Context.CustomerSalonReference;

namespace Modules.Customer.Persistence.Configuration;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<CustomerEntity>
{
    public void Configure(EntityTypeBuilder<CustomerEntity> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.FullName)
            .HasMaxLength(250)
            .IsRequired();
        builder.Property(customer => customer.BirthDate)
            .HasColumnType("date")
            .IsRequired();
        builder.Property(customer => customer.NationalCode)
            .HasMaxLength(10)
            .IsRequired();
        builder.Property(customer => customer.Address)
            .HasMaxLength(1000)
            .IsRequired();
        builder.Property(customer => customer.AllergyDescription)
            .HasMaxLength(1000);
        builder.Property(customer => customer.JobTitle)
            .HasMaxLength(150);
        builder.Property(customer => customer.LandlinePhone)
            .HasMaxLength(20);
        builder.Property(customer => customer.MobilePhone)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne<SalonEntity>()
            .WithMany()
            .HasForeignKey(customer => customer.SalonId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(customer => new { customer.SalonId, customer.NationalCode });
    }
}
