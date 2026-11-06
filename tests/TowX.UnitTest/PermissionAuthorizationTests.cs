using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity.Application.Contract.DTOs.Authentications;
using Identity.Application.Contract.Security;
using Identity.Application.Security;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Identity.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedKernel.Interface;
using SharedKernel.Security;
using Xunit;

namespace TowX.UnitTest;

public sealed class PermissionAuthorizationTests
{
    [Fact]
    public void Roles_ContainsOnlyTheFixedRoleNames()
    {
        Assert.All(Roles.All, role => Assert.True(Roles.IsKnown(role)));
        Assert.False(Roles.IsKnown("CustomRole"));
    }

    [Fact]
    public void PermissionRegistry_RejectsDuplicateAndInvalidPermissions()
    {
        Assert.Throws<InvalidOperationException>(() => new PermissionRegistry(
        [
            new StaticPermissionProvider("Reception.List"),
            new StaticPermissionProvider("Reception.List")
        ]));

        Assert.Throws<InvalidOperationException>(() => new PermissionRegistry(
        [
            new StaticPermissionProvider("Reception..List")
        ]));
    }

    [Fact]
    public void RolePermissions_RejectUnregisteredMappingEntries()
    {
        var registry = new PermissionRegistry(
        [
            new StaticPermissionProvider("Reception.Create")
        ]);

        Assert.Throws<InvalidOperationException>(() => RolePermissions.Validate(registry));
    }

    [Fact]
    public async Task CreateToken_EmitsDistinctRoleAndPermissionClaims()
    {
        var registry = CreateRegistry();
        var token = await CreateJwtService(registry).CreateTokenAsync(
            42,
            7,
            [Roles.Receptionist, Roles.Receptionist, Roles.Personnel],
            CancellationToken.None);
        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token.Token).Claims.ToArray();

        Assert.Equal(new[] { Roles.Personnel, Roles.Receptionist }, claims
            .Where(claim => claim.Type == "role")
            .Select(claim => claim.Value)
            .OrderBy(value => value, StringComparer.Ordinal));

        var permissions = claims
            .Where(claim => claim.Type == PermissionClaimTypes.Permission)
            .Select(claim => claim.Value)
            .ToArray();
        Assert.Equal(permissions.Length, permissions.Distinct(StringComparer.Ordinal).Count());
        Assert.Contains("Reception.Create", permissions);
        Assert.Contains("Appointment.List", permissions);
        Assert.Contains("Appointment.Details", permissions);
        Assert.DoesNotContain("Reception.Reports", permissions);
    }

    [Fact]
    public async Task CreateToken_RejectsUnknownRoles()
    {
        var jwtService = CreateJwtService(CreateRegistry());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            jwtService.CreateTokenAsync(42, 7, ["CustomRole"], CancellationToken.None));
    }

    [Fact]
    public async Task SuperAdminReceivesEveryRegisteredPermission()
    {
        var registry = CreateRegistry();
        var token = await CreateJwtService(registry).CreateTokenAsync(
            42, 7, [Roles.SuperAdmin], CancellationToken.None);
        var permissions = new JwtSecurityTokenHandler()
            .ReadJwtToken(token.Token)
            .Claims
            .Where(claim => claim.Type == PermissionClaimTypes.Permission)
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.Ordinal);

        Assert.True(registry.All.ToHashSet(StringComparer.Ordinal).SetEquals(permissions));
    }

    [Fact]
    public async Task PermissionPolicy_RequiresAuthenticationAndThePermissionClaim()
    {
        var registry = CreateRegistry();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(registry);
        services.AddPermissionAuthorization();
        using var provider = services.BuildServiceProvider();
        var authorization = provider.GetRequiredService<IAuthorizationService>();

        var allowed = await authorization.AuthorizeAsync(
            new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(PermissionClaimTypes.Permission, "Reception.Create")],
                "test")),
            resource: null,
            policyName: "Reception.Create");
        var missingPermission = await authorization.AuthorizeAsync(
            new ClaimsPrincipal(new ClaimsIdentity([], "test")),
            resource: null,
            policyName: "Reception.Create");
        var unauthenticated = await authorization.AuthorizeAsync(
            new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(PermissionClaimTypes.Permission, "Reception.Create")])),
            resource: null,
            policyName: "Reception.Create");

        Assert.True(allowed.Succeeded);
        Assert.False(missingPermission.Succeeded);
        Assert.False(unauthenticated.Succeeded);
        Assert.NotNull(provider.GetRequiredService<IOptions<AuthorizationOptions>>()
            .Value.GetPolicy("Appointment.Create"));
    }

    [Fact]
    public async Task IssuingANewTokenAfterRoleChangeUsesTheUpdatedPermissions()
    {
        var jwtService = CreateJwtService(CreateRegistry());
        var receptionistToken = new JwtSecurityTokenHandler()
            .ReadJwtToken((await jwtService.CreateTokenAsync(
                42, 7, [Roles.Receptionist], CancellationToken.None)).Token);
        var rolelessToken = new JwtSecurityTokenHandler()
            .ReadJwtToken((await jwtService.CreateTokenAsync(
                42, 7, [], CancellationToken.None)).Token);

        Assert.Contains(receptionistToken.Claims, claim =>
            claim.Type == PermissionClaimTypes.Permission && claim.Value == "Reception.Create");
        Assert.DoesNotContain(rolelessToken.Claims, claim =>
            claim.Type == PermissionClaimTypes.Permission);
    }

    [Fact]
    public async Task EffectivePermissionsApplyGrantsAndLetDeniesOverrideRolePermissions()
    {
        var registry = CreateRegistry();
        var overrides = new[]
        {
            UserPermissionOverride.Create(
                42, 7, "Appointment.Create", UserPermissionEffect.Grant, 1, DateTimeOffset.UtcNow),
            UserPermissionOverride.Create(
                42, 7, "Reception.Create", UserPermissionEffect.Deny, 1, DateTimeOffset.UtcNow)
        };
        var resolver = new EffectivePermissionResolver(
            new StaticPermissionOverrideRepository(overrides),
            registry);

        var permissions = await resolver.ResolveAsync(
            42, 7, [Roles.Receptionist], CancellationToken.None);

        Assert.Contains("Appointment.Create", permissions);
        Assert.DoesNotContain("Reception.Create", permissions);
    }

    [Fact]
    public async Task SuperAdminCanReadSalonAdminAccessAcrossSalons()
    {
        var registry = CreateRegistry();
        var repository = new StaticPermissionOverrideRepository(
            [],
            [
                new UserPermissionTarget(1, 0, [Roles.SuperAdmin]),
                new UserPermissionTarget(2, 8, [Roles.SalonAdmin])
            ]);
        var service = new UserPermissionManagementService(repository, registry);

        var result = await service.GetUserPermissionsAsync(1, 2, 0, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(8, result.Value.SalonId);
        Assert.Contains(Roles.SalonAdmin, result.Value.Roles);
    }

    [Fact]
    public async Task SalonAdminCannotReadUsersFromAnotherSalon()
    {
        var registry = CreateRegistry();
        var repository = new StaticPermissionOverrideRepository(
            [],
            [
                new UserPermissionTarget(1, 3, [Roles.SalonAdmin]),
                new UserPermissionTarget(2, 8, [Roles.Personnel])
            ]);
        var service = new UserPermissionManagementService(repository, registry);

        var result = await service.GetUserPermissionsAsync(1, 2, 3, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    private static PermissionRegistry CreateRegistry() =>
        new(
        [
            new StaticPermissionProvider(
                "Reception.Create",
                "Reception.List",
                "Reception.Details",
                "Reception.Update",
                "Reception.Cancel",
                "Reception.Reports"),
            new StaticPermissionProvider(
                "Appointment.Create",
                "Appointment.List",
                "Appointment.Details",
                "Appointment.Update",
                "Appointment.Cancel"),
            new StaticPermissionProvider(
                "Personnel.Create",
                "Personnel.List",
                "Personnel.Details",
                "Personnel.Update",
                "Personnel.Delete")
        ]);

    private static JwtService CreateJwtService(PermissionRegistry registry) =>
        new(Options.Create(new JwtSetting
        {
            SecretKey = "test-signing-key-that-is-at-least-thirty-two-characters"
        }), new RolePermissionResolver(registry));

    private sealed class RolePermissionResolver(PermissionRegistry registry)
        : IEffectivePermissionResolver
    {
        public Task<IReadOnlyCollection<string>> ResolveAsync(
            long userId,
            long salonId,
            IEnumerable<string> roles,
            CancellationToken cancellationToken) =>
            Task.FromResult(RolePermissions.GetPermissions(roles, registry));
    }

    private sealed class StaticPermissionOverrideRepository(
        IReadOnlyCollection<UserPermissionOverride> overrides,
        IReadOnlyCollection<UserPermissionTarget>? users = null)
        : IUserPermissionOverrideRepository
    {
        public Task<UserPermissionTarget?> GetTargetUserAsync(
            long userId,
            long salonId,
            CancellationToken cancellationToken)
        {
            var user = users?.FirstOrDefault(candidate =>
                candidate.UserId == userId && candidate.SalonId == salonId);
            return Task.FromResult(user);
        }

        public Task<UserPermissionTarget?> GetUserAsync(
            long userId,
            CancellationToken cancellationToken)
        {
            var user = users?.FirstOrDefault(candidate => candidate.UserId == userId);
            return Task.FromResult(user);
        }

        public Task<IReadOnlyCollection<UserPermissionOverride>> GetActiveOverridesAsync(
            long userId,
            long salonId,
            CancellationToken cancellationToken) =>
            Task.FromResult(overrides);

        public Task SaveOverrideAsync(
            long userId,
            long salonId,
            string permission,
            UserPermissionEffect effect,
            long actorUserId,
            DateTimeOffset now,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task RemoveOverrideAsync(
            long userId,
            long salonId,
            string permission,
            long actorUserId,
            DateTimeOffset now,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task ResetOverridesAsync(
            long userId,
            long salonId,
            long actorUserId,
            DateTimeOffset now,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }

    private sealed class StaticPermissionProvider(params string[] permissions)
        : IModulePermissionProvider
    {
        public IEnumerable<string> GetPermissions() => permissions;
    }
}
