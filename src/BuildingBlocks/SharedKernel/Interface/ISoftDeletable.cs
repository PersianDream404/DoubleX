using SharedKernel.Enums;

namespace SharedKernel.Interface;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    string? DeletedBy { get; }

    void SoftDelete(string? deletedBy);
    void Restore();
}
public interface IJwtService : IScopedDependency
{
   // JwtToken CreateToken(int Id, List<RoleType> roles);
    string GetClaim(string token, string claimType);
    (List<RoleType> roles, int id) ExteractToken(string token);
}
