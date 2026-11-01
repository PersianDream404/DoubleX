using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.FileStore.Persistence.Context;


namespace Modules.FileStore.Persistence.Extensions;

public static class FileStoreMigrationExtensions
{
    public static async Task ApplyCustomerMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var fileContext = scope.ServiceProvider.GetRequiredService<FileStoreWriteDbContext>();
        await fileContext.Database.MigrateAsync(cancellationToken);

        var context = scope.ServiceProvider.GetRequiredService<FileStoreWriteDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}
