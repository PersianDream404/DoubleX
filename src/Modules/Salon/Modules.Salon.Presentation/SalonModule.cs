using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Personnel.Application;
using Modules.Personnel.Persistence;
using Modules.Personnel.Persistence.Extensions;
using Modules.Personnel.Presentation.Endpoints.Personnels.Write;
using SharedKernel.Interface;

namespace Modules.Personnel.Presentation;

public class SalonModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSalonInfrastructure(configuration);
        services.AddSalonApplicationServices();

        services.AddControllers()
            .AddApplicationPart(typeof(SalonModule).Assembly)
            .AddControllersAsServices();

        services.Scan(scan =>
        {
            scan.FromAssemblyOf<CreatePersonnelEndpoint.EndPoint>()
                .AddClasses(x => x.AssignableTo<IEndpoint>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime();
        });
    }

    public async void RegisterBuilder(IApplicationBuilder builder)
    {
        await builder.ApplicationServices.SeedSalonAsync();
    }
}
