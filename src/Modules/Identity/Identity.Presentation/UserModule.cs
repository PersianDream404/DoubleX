using Identity.Presentation.Users.Create;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Persistence.Extensions;
using SharedKernel.Interface;
using SmeOpsHub.SharedKernel;

namespace Identity.Application;

public class UserModule : IModule
{
    public string Name => "User";
    public int Order => 100;

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityInfrastructure(configuration);
        services.AddIdentityApplication();

        services.Scan(scan =>
        {
            scan.FromAssemblyOf<RegisterUserEndpoint.EndPoint>()
                .AddClasses(x => x.AssignableTo<IEndpoint>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime();
        });

        // services.AddScoped<Identity.Application.Contract.Services.IAuthenticationService, AuthenticationService>();

    }
    public void RegisterBuilder(IApplicationBuilder builder) =>
    builder.ApplicationServices.ApplyIdentityMigrationsAsync()
        .GetAwaiter()
        .GetResult();
    //public void MapEndpoints(IEndpointRouteBuilder endpoints)
    //{
    //    endpoints.MapAreaControllerRoute(
    //        name: "user-area",
    //        areaName: "User",
    //        pattern: "User/{controller=User}/{action=Index}/{id?}");


    //}

    //public void ConfigureMenu(IMenuBuilder menu)
    //{
    //    menu.AddMenuItem(new MenuItem(
    //        key: "User",
    //        displayName: "کاربران",
    //        url: "/User/Index",
    //        order: Order));
    //}
}
