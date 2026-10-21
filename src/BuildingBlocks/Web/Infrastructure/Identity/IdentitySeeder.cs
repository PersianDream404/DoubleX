//using Microsoft.AspNetCore.Identity;
//using SmeOpsHub.Infrastructure.Identity;
//using SmeOpsHub.SharedKernel.Security;

//namespace SmeOpsHub.Web.Infrastructure.Identity;

//public static class IdentitySeeder
//{
//    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
//    {
//        using var scope = services.CreateScope();

//        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//        // Roles
//        foreach (var role in new[] { AppRoles.Admin, AppRoles.Manager, AppRoles.User })
//        {
//            if (!await roleManager.RoleExistsAsync(role))
//                await roleManager.CreateAsync(new IdentityRole(role));
//        }

//        // Optional: Admin user
//        var email = config["SeedAdmin:Email"];
//        var password = config["SeedAdmin:Password"];

//        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
//        {
//            var admin = await userManager.FindByEmailAsync(email);
//            if (admin is null)
//            {
//                admin = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
//                var result = await userManager.CreateAsync(admin, password);
//                if (!result.Succeeded)
//                    throw new Exception("Admin seed failed: " + string.Join("; ", result.Errors.Select(e => e.Description)));
//            }

//            if (!await userManager.IsInRoleAsync(admin, AppRoles.Admin))
//                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
//        }
//    }
//}
