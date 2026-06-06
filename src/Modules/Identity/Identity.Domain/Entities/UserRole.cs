using SharedKernel.Base;

namespace Identity.Domain.Entities;

public class UserRole : BaseEntityIdentity
{
    public string ?RoleName { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
