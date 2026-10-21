using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Modules.Banner.Domain.Entities;
using SharedKernel.Base;
using System.Linq.Expressions;

namespace Modules.Banner.Persistence.Context;

public class BannerReadDbContext : BaseDbContext
{
    public BannerReadDbContext(DbContextOptions<BaseDbContext> options) : base(options)
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
