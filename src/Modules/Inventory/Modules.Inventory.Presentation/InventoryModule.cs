using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Inventory.Application;
using Modules.Inventory.Persistence;
using Modules.Inventory.Persistence.Extensions;
using Modules.Inventory.Presentation.Endpoints;
using SharedKernel.Interface;

namespace Modules.Inventory.Presentation;

public sealed class InventoryModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddInventoryInfrastructure(configuration);
        services.AddInventoryApplicationServices();

        services.Scan(scan => scan
            .FromAssemblyOf<CreateUnitEndpoint.EndPoint>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }

    public void RegisterBuilder(IApplicationBuilder builder) =>
        builder.ApplicationServices.ApplyInventoryMigrationsAsync()
            .GetAwaiter()
            .GetResult();
}
