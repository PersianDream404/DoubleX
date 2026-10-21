using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Base;
using System.Linq.Expressions;

namespace Identity.Persistence.Context;
public class FileStoreReadDbContext : BaseDbContext
{
    public FileStoreReadDbContext(DbContextOptions<BaseDbContext> options) : base(options)
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
