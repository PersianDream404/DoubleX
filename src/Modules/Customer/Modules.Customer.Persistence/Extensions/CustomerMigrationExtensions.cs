using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Customer.Persistence.Context;
using Modules.Personnel.Persistence.Context;

namespace Modules.Customer.Persistence.Extensions;

public static class CustomerMigrationExtensions
{
    public static async Task ApplyCustomerMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var salonContext = scope.ServiceProvider.GetRequiredService<PersonnelWriteDbContext>();
        await salonContext.Database.MigrateAsync(cancellationToken);

        var context = scope.ServiceProvider.GetRequiredService<CustomerWriteDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}
