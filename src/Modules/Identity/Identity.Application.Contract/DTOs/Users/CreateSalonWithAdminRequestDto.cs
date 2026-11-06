namespace Identity.Application.Contract.DTOs.Users;

public sealed class CreateSalonWithAdminRequestDto
{
    public string SalonName { get; set; } = null!;
    public string AdminFullName { get; set; } = null!;
    public string AdminMobile { get; set; } = null!;
    public string AdminPassword { get; set; } = null!;
}

public sealed record CreateSalonWithAdminResponseDto(
    long SalonId,
    long AdminUserId,
    string Role);
