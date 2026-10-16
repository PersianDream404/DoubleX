using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Banner.Application;
using Modules.Banner.Persistence;
using Modules.Banner.Presentation;
using SharedKernel.Interface;
using Modules.Banner.Application;
using Modules.Banner.Presentation.Endpoints.Banners.Write;
namespace Modules.Banner.Presentation;


public class BannerModule : IModule
{


    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddBannerInfrastructure(configuration);
        services.AddBannerApplicationServices();

        services.Scan(scan =>
        {
            scan.FromAssemblyOf<CreateBannerEndpoint.EndPoint>()
                .AddClasses(x => x.AssignableTo<IEndpoint>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime();
        });


    }
}
