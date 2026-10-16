namespace Identity.Application.Contract.DTOs.Users;

public class RegisterUserRequestDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Mobile { get; set; } = null!;
    public string Password { get; set; }=null!;
}
public class RegisterUserResponseDto
{
    public long Id { get; set; }
    public List<string> Roles { get; set; } = [];
}
