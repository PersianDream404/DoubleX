using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Base;
using System.Linq.Expressions;

namespace Infrastructure.Context;
public class IdentityDbContext : IdentityApplicationDbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);


        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var deletedProperty = Expression.Property(parameter, nameof(BaseEntity.Deleted));
                var filter = Expression.Lambda(deletedProperty, parameter);

                // e => !e.Deleted
                var notDeleted = Expression.Lambda(
                    Expression.Equal(deletedProperty, Expression.Constant(false)),
                    parameter
                );

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(notDeleted);
            }
        }



    }
    #region DbSet
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    //public DbSet<JobSeeker> JobSeeker { get; set; }


    #endregion


}
