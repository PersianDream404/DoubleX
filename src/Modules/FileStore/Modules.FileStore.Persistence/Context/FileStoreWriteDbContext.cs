using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Modules.FileStore.Persistence.Context;
public class FileStoreWriteDbContext : BaseDbContext
{
    public FileStoreWriteDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {
    
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    #region DbSet
    public DbSet<Modules.FileStore.Domain.Entities.FileStores.FileStore> FileStores { get; set; }

    #endregion


}
