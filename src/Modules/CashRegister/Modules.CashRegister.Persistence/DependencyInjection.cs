using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Persistence.Context;
using Modules.CashRegister.Persistence.Repositories;
using SharedKernel.Constants;

namespace Modules.CashRegister.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddCashRegisterInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(AppSetting.ConnectionString);
        services.AddDbContext<CashRegisterWriteDbContext>(options => options.UseSqlServer(connectionString));
        services.AddDbContext<CashRegisterReadDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ICashRegisterCommandRepository, CashRegisterCommandRepository>();
        services.AddScoped<ICashRegisterQueryRepository, CashRegisterQueryRepository>();
        return services;
    }
}
