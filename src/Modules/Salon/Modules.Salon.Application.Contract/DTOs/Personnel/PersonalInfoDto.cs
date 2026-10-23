namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record PersonalInfoDto(
    string FullName,
    string NationalCode,
    DateOnly BirthDate,
    string MaritalStatus,
    int ChildrenCount,
    string MobilePhone,
    string? LandlinePhone,
    string AddressText,
    bool IsCloseToSalon,
    string? PictureUrl);

public sealed record GetAllPersonalResponseDto(
    long Id,
    string FullName,
    string NationalCode,
    string JobTitle,
    string CardNumber,
    DateOnly BirthDate,
    string MobilePhone);
