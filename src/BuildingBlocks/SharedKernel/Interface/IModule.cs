
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Interface;

// Contract that all modules implement so the Web host can wire them up
public interface IModule
{
    /// <summary>
    /// Name of the module, used for display and logging.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Order used for sorting modules in navigation / UI.
    /// Lower values come first.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Register module-specific services and DbContexts.
    /// </summary>
    void RegisterServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// Configure endpoints/routes for this module (e.g. MVC Area routes).
    /// </summary>
    void MapEndpoints(IEndpointRouteBuilder endpoints);

    /// <summary>
    /// Contribute navigation items (e.g. menu links) to the host application.
    /// </summary>
    void ConfigureMenu(IMenuBuilder menu);
}
