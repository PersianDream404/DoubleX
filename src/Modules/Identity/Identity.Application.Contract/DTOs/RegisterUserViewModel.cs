using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Application.Contract.DTOs;

public class RegisterUserViewModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}
