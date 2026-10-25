using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Inventory.Application.Contract.Interfaces.Materials;
using Modules.Inventory.Application.Contract.Interfaces.Units;
using Modules.Inventory.Domain.Interfaces.Materials;
using Modules.Inventory.Domain.Interfaces.StockMovements;
using Modules.Inventory.Domain.Interfaces.Units;
using Modules.Inventory.Persistence.Context;
using Modules.Inventory.Persistence.Repositories.Materials;
using Modules.Inventory.Persistence.Repositories.StockMovements;
using Modules.Inventory.Persistence.Repositories.Units;
using SharedKernel.Constants;

namespace Modules.Inventory.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInventoryInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(AppSetting.ConnectionString);
        services.AddDbContext<InventoryWriteDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDbContext<InventoryReadDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<IUnitCommandRepository, UnitCommandRepository>();
        services.AddScoped<IMaterialCommandRepository, MaterialCommandRepository>();
        services.AddScoped<IStockMovementCommandRepository, StockMovementCommandRepository>();
        services.AddScoped<IUnitQueryRepository, UnitQueryRepository>();
        services.AddScoped<IMaterialQueryRepository, MaterialQueryRepository>();
        return services;
    }
}
