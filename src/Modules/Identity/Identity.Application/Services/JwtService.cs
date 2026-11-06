using System.Text;

using Identity.Application.Contract.DTOs.Authentications;
using Identity.Application.Contract.Security;
using Identity.Application.Security;
using Identity.Domain.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SharedKernel.Interface;
using SharedKernel.Security;

namespace Identity.Application.Services;
public class JwtToken
{
    public string Token { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
}

public static class CustomClaims
{
    public const string User = "UserId";
    public const string SalonId = "SalonId";
}

public class JwtService(
    IOptions<JwtSetting> options,
    IEffectivePermissionResolver permissionResolver) : IJwtService, IScopedDependency
{

    public async Task<JwtTokenResponseDto> CreateTokenAsync(
        long userId,
        long salonId,
        List<string> roles,
        CancellationToken cancellationToken)
    {
        var expiredDate = DateTime.UtcNow.AddMinutes(15);

        var secretKey = options.Value.SecretKey;
        var permissions = await permissionResolver.ResolveAsync(
            userId, salonId, roles, cancellationToken);

        var claims = new List<Claim>
        {
            new(CustomClaims.User, userId.ToString()),
            new(CustomClaims.SalonId, salonId.ToString())
        };

        foreach (var role in roles.Distinct(StringComparer.Ordinal))
        {
            if (!Roles.IsKnown(role))
            {
                throw new InvalidOperationException($"Unknown role '{role}' cannot be used to issue a token.");
            }

            claims.Add(new(ClaimTypes.Role, role));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new(PermissionClaimTypes.Permission, permission));
        }

        var token = GenerateToken(claims, expiredDate, secretKey);

        return new JwtTokenResponseDto
        {
            Token = token,
            ExpireDate = expiredDate
        };
    }

    public string GetClaim(string token, string claimType)
    {
        try
        {
            token = token.Replace("Bearer", "", StringComparison.OrdinalIgnoreCase).Trim();
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var claims = handler.ValidateToken(token, validations, out var tokenSecure);

            return claims.FindFirst(s => s.Type == claimType)?.Value ?? string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    private string GenerateToken(List<Claim> claims, DateTime expiredDate, string secretKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiredDate,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public (List<string> roles, long id) ExteractToken(string token)
    {
        try
        {
            token = token.Replace("Bearer", "", StringComparison.OrdinalIgnoreCase).Trim();

            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var principal = handler.ValidateToken(token, validations, out _);
            if (!long.TryParse(principal.FindFirstValue(CustomClaims.User), out var userId))
            {
                return ([], 0);
            }

            return (principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList(), userId);
        }
        catch
        {
            return (new List<string>(), 0);
        }
    }


}