using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Finance.Application;
using Modules.Finance.Persistence;
using Modules.Finance.Persistence.Extensions;
using Modules.Finance.Presentation.Security;
using SharedKernel.Interface;

namespace Modules.Finance.Presentation;

public sealed class FinanceModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddFinanceInfrastructure(configuration);
        services.AddFinanceApplicationServices();
        services.AddSingleton<IModulePermissionProvider, FinancePermissionProvider>();
        services.Scan(scan => scan
            .FromAssemblyOf<FinanceModule>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }

    public void RegisterBuilder(IApplicationBuilder builder) =>
        builder.ApplicationServices.ApplyFinanceMigrationsAsync()
            .GetAwaiter()
            .GetResult();
}
