using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Dashboard.Application;
using Modules.Dashboard.Persistence;
using SharedKernel.Interface;

namespace Modules.Dashboard.Presentation;

public sealed class DashboardModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDashboardInfrastructure();
        services.AddDashboardApplicationServices();
        services.Scan(scan => scan
            .FromAssemblyOf<DashboardModule>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }

    public void RegisterBuilder(IApplicationBuilder builder)
    {
    }
}
