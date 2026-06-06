//using FAPN.Contract.Enums;
//using FAPN.Domain.Models.Users;
//using Karbia.Domain.Models.JobSeekers;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace Karbia.Infrastructure.Persistence.Configurations.Users;

//public class UserConfiguration : IEntityTypeConfiguration<User>
//{
//    public void Configure(EntityTypeBuilder<User> builder)
//    {
//        // نام جدول
//        builder.ToTable("Users");

//        // کلید اصلی
//        builder.HasKey(x => x.Id);
//        //builder.Ignore(_ => _.FullName);


//        // اطلاعات هویتی
//        builder.Property(p => p.FirstName)
//            .IsRequired()
//            .HasMaxLength(100);

//        builder.Property(p => p.LastName)
//            .IsRequired()
//            .HasMaxLength(100);

//        builder.Property(p => p.NationalCode)
//            //  .IsRequired()
//            .HasMaxLength(10);

//        builder.HasIndex(p => new { p.NationalCode})
           
//            .HasDatabaseName("IX_People_NationalCode");

//        builder.Property(p => p.FatherName)
//      //  .IsRequired()
//         .HasMaxLength(200);

//        builder.Property(p => p.BirthDate)
//            .HasColumnType("date");

//        builder.Property(p => p.BirthPlace)
//            .HasMaxLength(100);

//        builder.Property(p => p.IssuePlace)
//             .HasMaxLength(200);

//        builder.Property(p => p.Gender)
//            .HasConversion<string>()
//            .HasMaxLength(10);

//        builder.Property(p => p.MaritalStatus)
//             .HasDefaultValue(MaritalStatus.Unknown)
//             ;
//        builder.Property(p => p.DependentRelation)
//             .HasDefaultValue(DependentRelation.Unknown)
//             ;

//        builder.Property(p => p.MilitaryStatus)
//             .HasDefaultValue(MilitaryStatus.Unknown)
//             ;
//        builder.Property(p => p.PhoneNumber)
//            .HasMaxLength(15);

//        builder.Property(p => p.MobileNumber)
//            .HasMaxLength(15);

//        builder.Property(p => p.Email)
//            .HasMaxLength(150);

//        //builder.Property(x => x.FullName)
//        //    .HasMaxLength(200);

//        builder.Property(x => x.UserName)
//                .IsRequired()
//                .HasMaxLength(100);

//        builder.Property(x => x.Password)
//            .IsRequired()
//            .HasMaxLength(500);

//        builder.Property(x => x.Salt)
//            .IsRequired()
//            .HasMaxLength(500);

//        // مقدار enum (UserType)
//        builder.Property(x => x.UserType)
//            .IsRequired()
//            .HasConversion<string>(); // تبدیل Enum به مقدار عددی در دیتابیس

//        // اگر خواستی رابطه‌ای با JobSeeker داشته باشی (یک‌به‌یک)
//        // می‌توانی این بخش را فعال کنی
//        //builder.HasOne<JobSeeker>()
//        //    .WithOne(x => x.User)
//        //    .HasForeignKey<JobSeeker>(x => x.UserId)
//        //    .OnDelete(DeleteBehavior.Cascade);

//        //builder.HasMany(x => x.SocialMediaAccount)
//        //    .WithOne(x => x.User)
//        //    .HasForeignKey(x => x.UserId)
//        //    .OnDelete(DeleteBehavior.Cascade);
//    }
//}

