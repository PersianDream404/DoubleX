using Identity.Application.Contract.DTOs.Authentications;
using Identity.Domain.Entities;
using Identity.Domain.Interface;
using Identity.Persistence.Context;
using Identity.Persistence.Repositories.Users;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Constants;
using SharedKernel.Interface;
using System.Text;

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

        services.Configure<JwtSetting>(options =>
            configuration.GetSection("Setting:Jwt").Bind(options));

        //services.Configure<S3Configuration>(options =>
        //    configuration.GetSection("S3").Bind(options));
        #endregion


        services.AddDbContext<IdentityWriteDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));

        //services.AddDbContext<IdentityReadDbContext>(opt =>
        //    opt.UseSqlServer(configuration.GetConnectionString(AppSetting.ConnectionString)));


        services.AddScoped<IUserCommandRepository, UserCommandRepository>();
        services.AddScoped<IUserQueryRepository, UserQueryRepository>();

        // Add Identity
        services.AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
        {
            options.Password.RequiredLength = 3;
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;

            options.User.RequireUniqueEmail = false;
        })
        .AddEntityFrameworkStores<IdentityWriteDbContext>()
        .AddDefaultTokenProviders();

        // JWT Config
        //var jwtSettings = configuration.GetSection("Jwt");
        //var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // در Production بهتره true باشد
            options.TokenValidationParameters = new TokenValidationParameters
            {
                //ValidateIssuer = true,
                //ValidateAudience = true,
                //ValidateLifetime = true,
                //ValidateIssuerSigningKey = true,

                //ValidIssuer = jwtSettings["Issuer"],
                //ValidAudience = jwtSettings["Audience"],
                //IssuerSigningKey = new SymmetricSecurityKey(key),
                //ClockSkew = TimeSpan.Zero

                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.GetSection("Setting:SecretKey").Value!))
            };
        });

        services.AddAuthorization();

        return services;
    }

    //private static void AddAuthenticationConfiguration(this IServiceCollection services, IConfiguration configuration)
    //{
    //    services.AddAuthorization();
    //    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    //    {
    //        options.TokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuer = false,
    //            ValidateAudience = false,
    //            ValidateLifetime = true,
    //            ValidateIssuerSigningKey = true,
    //            IssuerSigningKey = new SymmetricSecurityKey(
    //                Encoding.UTF8.GetBytes(configuration.GetSection("Setting:SecretKey").Value!))
    //        };
    //    });
    //}





}