using Framwork.Extensions;
using Identity.Domain.Entities;
using Identity.Domain.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modules.Identity.Persistence.Extensions;

internal static class SuperAdminSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        var fullName = configuration["Identity:BootstrapSuperAdmin:FullName"];
        var mobile = configuration["Identity:BootstrapSuperAdmin:Mobile"];
        var password = configuration["Identity:BootstrapSuperAdmin:Password"];
        var hasAnyValue = !string.IsNullOrWhiteSpace(fullName) ||
                          !string.IsNullOrWhiteSpace(mobile) ||
                          !string.IsNullOrWhiteSpace(password);
        if (!hasAnyValue)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(mobile) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Bootstrap SuperAdmin requires FullName, Mobile, and Password settings.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var normalizedMobile = mobile.NormalizePhoneNumber();
        var user = await userManager.FindByNameAsync(normalizedMobile);
        if (user is not null)
        {
            if (!await userManager.IsInRoleAsync(user, Roles.SuperAdmin))
            {
                throw new InvalidOperationException(
                    "The configured Bootstrap SuperAdmin mobile belongs to an existing non-SuperAdmin account. " +
                    "Use a new mobile number or assign the role through a trusted administrative process.");
            }

            return;
        }

        user = ApplicationUser.Register(normalizedMobile, fullName, salonId: 0);
        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Could not create bootstrap SuperAdmin: {errors}");
        }

        var roleResult = await userManager.AddToRoleAsync(user, Roles.SuperAdmin);
        if (!roleResult.Succeeded)
        {
            var deleteResult = await userManager.DeleteAsync(user);
            var roleErrors = string.Join("; ", roleResult.Errors.Select(error => error.Description));
            var cleanupErrors = deleteResult.Succeeded
                ? string.Empty
                : " The partially created account could not be removed.";
            throw new InvalidOperationException(
                $"Could not assign SuperAdmin role: {roleErrors}.{cleanupErrors}");
        }
    }
}
