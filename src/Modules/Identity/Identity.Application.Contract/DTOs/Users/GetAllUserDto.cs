using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Contract.DTOs.Users;

public class GetAllUserRequestDto
{
    public string? Q { get; set; }
}
public class GetAllUserResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
