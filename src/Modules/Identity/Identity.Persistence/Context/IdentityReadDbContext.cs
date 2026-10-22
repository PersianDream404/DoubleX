using Identity.Domain.Entities;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Base;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Design;
using SharedKernel.Constants;

namespace Identity.Persistence.Context;
public class IdentityReadDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
    public IdentityReadDbContext(DbContextOptions<IdentityReadDbContext> options) : base(options)
    {
    
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    
    public class IdentityReadDbContextFactory : IDesignTimeDbContextFactory<IdentityReadDbContext>
    {
        public IdentityReadDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityReadDbContext>();
        
            // اینجا کانکشن استرینگ دیتابیس خود را برای محیط توسعه (Development) قرار دهید
            optionsBuilder.UseSqlServer(AppSetting.ConnectionStringText);

            return new IdentityReadDbContext(optionsBuilder.Options);
        }
    }
    #region DbSet


    public DbSet<ApplicationUser> Users { get; set; }
    


    //public DbSet<JobSeeker> JobSeeker { get; set; }


    #endregion


}
