using Identity.Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Base;
using System.Linq.Expressions;

namespace Identity.Persistence.Context;
public class ReadDbContext : BaseDbContext
{
    public ReadDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {
    
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    #region DbSet


    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }


    //public DbSet<JobSeeker> JobSeeker { get; set; }


    #endregion


}
