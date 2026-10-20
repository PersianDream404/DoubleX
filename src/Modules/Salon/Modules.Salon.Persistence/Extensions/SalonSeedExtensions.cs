using Modules.Personnel.Persistence.Context;

namespace Modules.Personnel.Persistence.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class SalonSeedExtensions
{
    public static async Task SeedSalonAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider
            .GetRequiredService<PersonnelWriteDbContext>();

    
        await context.Database.MigrateAsync(cancellationToken);


        if (await context.Set<Domain.Entities.Salon>().AnyAsync(cancellationToken))
            return;

        var salon =  Domain.Entities.Salon.Create("سالن زیبایی نمونه");

        context.Set<Domain.Entities.Salon>().Add(salon);

        await context.SaveChangesAsync(cancellationToken);
    }
}