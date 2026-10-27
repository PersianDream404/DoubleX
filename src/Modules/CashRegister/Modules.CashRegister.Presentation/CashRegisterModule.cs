using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.CashRegister.Application;
using Modules.CashRegister.Persistence;
using Modules.CashRegister.Persistence.Extensions;
using SharedKernel.Interface;

namespace Modules.CashRegister.Presentation;

public sealed class CashRegisterModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCashRegisterInfrastructure(configuration);
        services.AddCashRegisterApplicationServices();
        services.Scan(scan => scan
            .FromAssemblyOf<CashRegisterModule>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }

    public void RegisterBuilder(IApplicationBuilder builder) =>
        builder.ApplicationServices.ApplyCashRegisterMigrationsAsync()
            .GetAwaiter()
            .GetResult();
}
