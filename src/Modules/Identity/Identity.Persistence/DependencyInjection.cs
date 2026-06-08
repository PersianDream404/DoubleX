using Identity.Domain.Entities;
using Identity.Domain.Interface;
using Identity.Persistence.Context;
using Identity.Persistence.Repositories.Users;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Constants;
using SharedKernel.Interface;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
          .AddPersistence(configuration)
        ;

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        #region AppSetting Bind

        //services.Configure<FarazSmsOptions>(options =>
        //    configuration.GetSection("FarazSms").Bind(options));

        //services.Configure<S3Configuration>(options =>
        //    configuration.GetSection("S3").Bind(options));
        #endregion


        services.AddDbContext<WriteDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));

        services.AddDbContext<ReadDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));


        services.AddScoped<IUserCommandRepository, UserCommandRepository>();
        services.AddScoped<IUserQueryRepository, UserQueryRepository>();



        return services;
    }







}