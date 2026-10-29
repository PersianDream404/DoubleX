using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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

    public sealed class BannerWriteDbContextFactory
        : IDesignTimeDbContextFactory<BannerWriteDbContext>
    {
        public BannerWriteDbContext CreateDbContext(string[] args)
        {
            var connectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__BaseConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Set ConnectionStrings__BaseConnection to create Banner migrations.");
            }

            var options = new DbContextOptionsBuilder<BaseDbContext>()
                .UseSqlServer(connectionString)
                .Options;
            return new BannerWriteDbContext(options);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Banner");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    #region DbSet
    public DbSet<BannerEntity> Banners { get; set; }




    #endregion


}
