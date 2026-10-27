using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.CashRegister.Persistence.Context;

namespace Modules.CashRegister.Persistence.Extensions;

public static class CashRegisterMigrationExtensions
{
    public static async Task ApplyCashRegisterMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<CashRegisterWriteDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}
