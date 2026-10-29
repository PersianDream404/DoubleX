using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedKernel.Constants;

namespace Modules.FileStore.Persistence.Context;
public class FileStoreWriteDbContext : BaseDbContext
{
    public FileStoreWriteDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {
    
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("FileStore");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
    #region DbSet
    public DbSet<Modules.FileStore.Domain.Entities.FileStores.FileStore> FileStores { get; set; }

    #endregion


}

public sealed class FileStoreWriteDbContextFactory
    : IDesignTimeDbContextFactory<FileStoreWriteDbContext>
{
    public FileStoreWriteDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__BaseConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set ConnectionStrings__BaseConnection to create FileStore migrations.");
        }

        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        return new FileStoreWriteDbContext(options);
    }
}
