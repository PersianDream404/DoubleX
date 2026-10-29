using Identity.Domain.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedKernel.Base;
using SharedKernel.Constants;
using System.Linq.Expressions;

namespace Identity.Persistence.Context;
public class IdentityWriteDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
    public IdentityWriteDbContext(DbContextOptions<IdentityWriteDbContext> options) : base(options)
    {
    
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Identity");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    #region DbSet
    public DbSet<ApplicationUser> Users { get; set; }



    #endregion


}

public class IdentityWriteDbContextFactory : IDesignTimeDbContextFactory<IdentityWriteDbContext>
{
    public IdentityWriteDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityWriteDbContext>();
        optionsBuilder.UseSqlServer(AppSetting.ConnectionStringText);

        return new IdentityWriteDbContext(optionsBuilder.Options);
    }
}
