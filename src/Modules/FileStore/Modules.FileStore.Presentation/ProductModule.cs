using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.FileStore.Persistence;
using Modules.FileStore.Presentation.Endpoints.FileStores.Write;
using SharedKernel.Interface;
using SmeOpsHub.SharedKernel;

namespace Modules.FileStore.Presentation;

public class FileStoreModule : IModule
{


    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityInfrastructure(configuration);
        services.AddIdentityApplication();

        services.Scan(scan =>
        {
            scan.FromAssemblyOf<CreateFileStoreEndpoint.EndPoint>()
                .AddClasses(x => x.AssignableTo<IEndpoint>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime();
        });


    }
}
