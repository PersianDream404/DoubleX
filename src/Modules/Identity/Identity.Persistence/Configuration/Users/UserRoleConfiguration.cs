using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Persistence.Configuration.Users;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {

        builder.ToTable("Users");


        builder.HasKey(x => x.Id);


        builder.Property(p => p.RoleName)
            .IsRequired()
            .HasMaxLength(50);



        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRole)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

