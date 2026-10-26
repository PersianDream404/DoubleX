using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Service.Application;
using Modules.Service.Persistence;
using Modules.Service.Persistence.Extensions;
using Modules.Service.Presentation.Endpoints;
using SharedKernel.Interface;

namespace Modules.Service.Presentation;

public sealed class ServiceModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddServiceInfrastructure(configuration);
        services.AddServiceApplicationServices();
        services.Scan(scan => scan
            .FromAssemblyOf<Endpoints.Services.GetAll.GetServicesEndpoint.EndPoint>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }

    public void RegisterBuilder(IApplicationBuilder builder) =>
        builder.ApplicationServices.ApplyServiceMigrationsAsync()
            .GetAwaiter()
            .GetResult();
}
