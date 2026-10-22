using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Domain.Interface;
using Modules.Personnel.Persistence.Context;
using Modules.Personnel.Persistence.Repositories;
using Modules.Product.Application.Contract.Interface.Salons;
using Modules.Salon.Domain.Interface;
using Modules.Salon.Persistence.Repositories.Salons;
using SharedKernel.Constants;

namespace Modules.Personnel.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddSalonInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PersonnelWriteDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));

        services.AddDbContext<PersonnelReadDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));

        services.AddScoped<IPersonnelCommandRepository, PersonnelCommandRepository>();
        services.AddScoped<IPersonnelQueryRepository, PersonnelQueryRepository>();
        
        services.AddScoped<ISalonCommandRepository, SalonCommandRepository>();
        services.AddScoped<ISalonQueryRepository, SalonQueryRepository>();

        return services;
    }
}
