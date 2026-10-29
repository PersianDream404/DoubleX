using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Persistence.Context;
public class IdentityReadDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
    public IdentityReadDbContext(DbContextOptions<IdentityReadDbContext> options) : base(options)
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
    


    //public DbSet<JobSeeker> JobSeeker { get; set; }


    #endregion


}
