using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Personnel.Application;
using Modules.Personnel.Persistence;
using SharedKernel.Interface;

namespace Modules.Personnel.Presentation;

public class PersonnelModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersonnelInfrastructure(configuration);
        services.AddPersonnelApplicationServices();

        services.AddControllers()
            .AddApplicationPart(typeof(PersonnelModule).Assembly)
            .AddControllersAsServices();

        // services.Scan(scan =>
        // {
        //     scan.FromAssemblyOf<CreatePersonnelEndpoint.EndPoint>()
        //         .AddClasses(x => x.AssignableTo<IEndpoint>())
        //         .AsImplementedInterfaces()
        //         .WithSingletonLifetime();
        // });
    }
}
