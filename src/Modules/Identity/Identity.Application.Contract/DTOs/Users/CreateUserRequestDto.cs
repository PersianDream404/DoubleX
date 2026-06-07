namespace Identity.Application.Contract.DTOs.Users;

public class CreateUserRequestDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? Password { get; set; } 
}
