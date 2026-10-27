using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Persistence.Repositories;
using Modules.Reception.Domain.Interfaces;
using Modules.Reception.Persistence.Context;
using SharedKernel.Constants;

namespace Modules.Reception.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddReceptionInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(AppSetting.ConnectionString);
        services.AddDbContext<ReceptionWriteDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDbContext<ReceptionReadDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<ReceptionCommandRepository>();
        services.AddScoped<IReceptionCommandRepository>(provider =>
            provider.GetRequiredService<ReceptionCommandRepository>());
        services.AddScoped<ICashRegisterPaymentOutboxRepository>(provider =>
            provider.GetRequiredService<ReceptionCommandRepository>());
        services.AddScoped<IReceptionQueryRepository, ReceptionQueryRepository>();
        return services;
    }
}
