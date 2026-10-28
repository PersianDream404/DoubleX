using Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Modules.Identity.Persistence.Extensions;

public static class IdentityMigrationExtensions
{
    public static async Task ApplyIdentityMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityWriteDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}
