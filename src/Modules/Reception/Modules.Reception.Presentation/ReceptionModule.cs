using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Reception.Application;
using Modules.Reception.Persistence;
using Modules.Reception.Persistence.Extensions;
using SharedKernel.Interface;

namespace Modules.Reception.Presentation;

public sealed class ReceptionModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddReceptionInfrastructure(configuration);
        services.AddReceptionApplicationServices();
        services.AddHostedService<CashRegisterPaymentOutboxProcessor>();
        services.Scan(scan => scan
            .FromAssemblyOf<ReceptionModule>()
            .AddClasses(classes => classes.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }

    public void RegisterBuilder(IApplicationBuilder builder) =>
        builder.ApplicationServices.ApplyReceptionMigrationsAsync()
            .GetAwaiter()
            .GetResult();
}
