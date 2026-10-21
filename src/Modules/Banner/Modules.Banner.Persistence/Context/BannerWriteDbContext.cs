using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Modules.Banner.Domain.Entities;

namespace Modules.Banner.Persistence.Context;
public class BannerWriteDbContext : BaseDbContext
{
    //public BannerWriteDbContext(DbContextOptions<BannerWriteDbContext> options) : base(options)
    //{
    //}

    public BannerWriteDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {
    
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    #region DbSet
    public DbSet<BannerEntity> Banners { get; set; }




    #endregion


}
