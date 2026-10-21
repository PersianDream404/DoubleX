namespace Identity.Application.Contract.DTOs.Users;

public class RegisterUserRequestDto
{
    public RegisterUserSalonRequestDto? Salon { get; set; }
    public string? FullName { get; set; }
    public string Mobile { get; set; } = null!;
    public string Password { get; set; }=null!;
}

public class RegisterUserSalonRequestDto
{
    public long Id { get; set; }

}
public class RegisterUserResponseDto
{
    public long Id { get; set; }
    public long SalonId { get; set; }
    public List<string> Roles { get; set; } = [];
}
