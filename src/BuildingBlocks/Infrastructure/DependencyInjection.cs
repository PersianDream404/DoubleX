using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Constants;
using SharedKernel.Interface;
using SharedKernel.Interface.Repositories;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
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


        services.AddDbContext<BaseDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));

        //  services.AddScoped<IUnitOfWork, UnitOfWork>();
        //services.AddScoped<IFileUploaderService, MinioFileUploaderService>();

        //services.AddScoped<IUsersRepository, UsersRepository>();
        //services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
        services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));


        services.Scan(scan => scan
            .FromAssemblyOf<UnitOfWork>()
            .AddClasses(classes => classes.AssignableTo<IScopedDependency>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonDependency>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            .AddClasses(classes => classes.AssignableTo<ITransientDependency>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }







}