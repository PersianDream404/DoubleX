using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Enums;
using Modules.Personnel.Domain.ValueObjects;
using System.Text.Json;

namespace Modules.Personnel.Persistence.Configuration;

public sealed class PersonnelConfiguration : IEntityTypeConfiguration<Domain.Entities.Personnel>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Personnel> builder)
    {


        // ۱. جدول اصلی پرسنل
        builder.ToTable("Personnels");

        builder.HasKey(x => x.Id);

        // ۲. جدول مجزا برای اطلاعات شخصی (PersonalInfo)
        builder.OwnsOne(x => x.PersonalInfo, personal =>
        {
            personal.ToTable("PersonnelPersonalInfos"); // قرارگیری در جدول جداگانه

            personal.Property(x => x.FullName).HasMaxLength(250).IsRequired();
            personal.Property(x => x.BirthDate).IsRequired();
            personal.Property(x => x.ChildrenCount).IsRequired();
            personal.Property(x => x.MaritalStatus).HasConversion<string>().IsRequired();

            personal.OwnsOne(x => x.NationalCode, national =>
            {
                national.Property(p => p.Value).HasColumnName("NationalCode").HasMaxLength(10).IsRequired();
            });

            personal.OwnsOne(x => x.MobilePhone, mobile =>
            {
                mobile.Property(p => p.Value).HasColumnName("MobilePhone").HasMaxLength(20).IsRequired();
            });

            personal.OwnsOne(x => x.LandlinePhone, phone =>
            {
                phone.Property(p => p.Value).HasColumnName("LandlinePhone").HasMaxLength(20);
            });
            personal.OwnsOne(x => x.Picture, pic =>
            {
                pic.ToTable("PersonnelPicture");
            });
            personal.Navigation(x => x.Picture).IsRequired(false);
            personal.OwnsOne(x => x.Address, address =>
            {
                address.Property(p => p.Text).HasColumnName("AddressText").HasMaxLength(1000);
                address.Property(p => p.IsCloseToSalon).HasColumnName("IsCloseToSalon");
            });
        });

        // ۳. جدول مجزا برای اطلاعات شغلی (JobInfo)
        builder.OwnsOne(x => x.JobInfo, job =>
        {
            job.ToTable("PersonnelJobInfos"); // قرارگیری در جدول جداگانه

            job.Property(x => x.JobTitle).HasMaxLength(150).IsRequired();
            job.Property(x => x.PersonnelCode).HasMaxLength(100).IsRequired();
            job.Property(x => x.CooperationStartDate).IsRequired();
            job.Property(x => x.WorkingShiftType).HasConversion<string>().IsRequired();
            job.Property(x => x.WorkHistory).HasMaxLength(2000);

            // تنظیمات مربوط به ذخیره‌سازی Skills به صورت JSON
            var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = false };

            var skillsConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, jsonSerializerOptions),
                v => JsonSerializer.Deserialize<List<string>>(v, jsonSerializerOptions) ?? new List<string>()
            );

            var skillsComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c == null ? new List<string>() : c.ToList()
            );

            //job.Property(x => x.Skills)
            //    .HasConversion(skillsConverter)
            //    .Metadata.SetValueComparer(skillsComparer);

            //job.Property(x => x.Skills)
            //    .HasColumnName("Skills");
        });



        builder.OwnsOne(x => x.FinancialInfo, finances =>
        {
            finances.ToTable("PersonnelFinancialInfos");

            var cardNumberConverter = new ValueConverter<CardNumber, string>(
                cardNumber => cardNumber.Value,
                value => CardNumber.Create(value).Value
            );

            finances.Property(x => x.CardNumber)
                .HasConversion(cardNumberConverter)
                .HasColumnName("CardNumber")
                .HasMaxLength(16)
                .IsRequired();

            finances.Property(x => x.AccountNumber)
                .HasMaxLength(50);
        });

        builder.HasMany(x => x.Contracts)
            .WithOne(contract => contract.Personnel)
            .HasForeignKey(contract => contract.PersonnelId)
            .OnDelete(DeleteBehavior.Restrict);

        // ۵. جدول مجزا برای اطلاعات سلامتی (HealthInfo)
        builder.OwnsOne(x => x.HealthInfo, health =>
        {
            health.ToTable("PersonnelHealthInfo"); // قرارگیری در جدول جداگانه

            health.OwnsMany(x => x.PictureCourse, pic =>
            {
                pic.ToTable("PersonnelHealthCoursePicture");

                pic.WithOwner()
                    .HasForeignKey("PersonnelId");

                pic.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                pic.HasKey("Id");

                // سایر Propertyهای مربوط به تصویر را اینجا تنظیم کن.
            });
            
            health.OwnsMany(x => x.PictureTest, pic =>
            {
                pic.ToTable("PersonnelHealthTestPicture");

                pic.WithOwner()
                    .HasForeignKey("PersonnelId");

                pic.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                pic.HasKey("Id");

                // سایر Propertyهای مربوط به تصویر را اینجا تنظیم کن.
            });
            health.Property(x => x.HealthStatus).HasMaxLength(500);
            health.Property(x => x.AllergyStatus).HasMaxLength(500);
        });

        // ۶. جدول مجزا برای اطلاعات ضامن (GuarantorInfo)
        builder.OwnsOne(x => x.GuarantorInfo, guarantor =>
        {
            guarantor.ToTable("PersonnelGuarantorInfo");

            guarantor.Property(x => x.GuarantorName)
                .HasMaxLength(200)
                .IsRequired();

            guarantor.Property(x => x.Relationship)
                .HasMaxLength(150)
                .IsRequired();

            guarantor.OwnsOne(x => x.GuarantorMobilePhone, phone =>
            {
                phone.Property(p => p.Value)
                    .HasColumnName("GuarantorMobilePhone")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            guarantor.OwnsOne(x => x.GuarantorLandlinePhone, phone =>
            {
                phone.Property(p => p.Value)
                    .HasColumnName("GuarantorLandlinePhone")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            guarantor.OwnsMany(x => x.PictureGuaranties, pic =>
            {
                pic.ToTable("PersonnelGuarantorPictures");

                pic.WithOwner()
                    .HasForeignKey("PersonnelId");

                pic.Property<int>("Id")
                    .ValueGeneratedOnAdd();

                pic.HasKey("Id");

                // سایر Propertyهای مربوط به تصویر را اینجا تنظیم کن.
            });
        });

        builder.HasOne(x => x.Salon)
            .WithMany(x=>x.Personnels)
            .HasForeignKey(x => x.SalonId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }

}
