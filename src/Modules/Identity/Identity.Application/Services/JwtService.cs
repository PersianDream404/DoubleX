using System.Text;

using Identity.Application.Contract.DTOs.Authentications;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SharedKernel.Interface;

namespace Identity.Application.Services;
public class JwtToken
{
    public string Token { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
}

public static class CustomClaims
{
    public const string User = "UserId";

    public const string AccessLevel = "AccessLevel";
}

public class JwtService(IOptions<JwtSetting> options) : IJwtService,IScopedDependency
{

    public JwtTokenResponseDto CreateToken(long Id, List<string> roles)
    {
        var expiredDate = DateTime.UtcNow.AddDays(1);

        var secretKey = options.Value.SecretKey;

        var claims = new List<Claim>
        {
            new(CustomClaims.User, Id.ToString()),

        };
        foreach (var role in roles)
        {
            claims.Add(new(CustomClaims.AccessLevel, nameof(role)));
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

            return claims.FindFirst(s => s.Type == claimType)?.ValueType ?? string.Empty;
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
            // پاکسازی توکن
            token = token.Replace("Bearer", "", StringComparison.OrdinalIgnoreCase).Trim();

            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            // اعتبارسنجی توکن
            var principal = handler.ValidateToken(token, validations, out _);

            // استخراج Claim اصلی
            var firstClaim = principal.Claims.FirstOrDefault();
            if (firstClaim == null)
                return (new List<string>(), 0);

            // تعیین نوع کاربر
            var userType = firstClaim.Type switch
            {
                CustomClaims.User => AppRoles.User,
                _ => "None"
            };

            // استخراج شناسه
            int.TryParse(firstClaim.Value, out int userId);

            return (new List<string> { userType }, userId);
        }
        catch
        {
            return (new List<string>(), 0);
        }
    }


}