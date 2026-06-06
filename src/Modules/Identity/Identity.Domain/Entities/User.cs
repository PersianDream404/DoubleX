using SharedKernel.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Identity.Domain.Entities;

public class User : BaseEntityIdentity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public ICollection<UserRole> UserRole { get; set; } = [];
}
