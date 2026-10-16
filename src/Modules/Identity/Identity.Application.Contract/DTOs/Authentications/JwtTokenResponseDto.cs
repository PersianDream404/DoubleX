namespace Identity.Application.Contract.DTOs.Authentications;

public class JwtTokenResponseDto
{
    public string Token { get; set; } = null!;
    public DateTime ExpireDate { get; set; }
}

public class JwtSetting
{

    public string SecretKey { get; set; } = string.Empty;
}