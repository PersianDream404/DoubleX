using Modules.Personnel.Domain.Enums;

namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

using System;

public sealed class PersonalInfoRequestDto
{
    public string FullName { get; init; }
    public string NationalCode { get; init; }
    public DateOnly BirthDate { get; init; } = DateOnly.FromDateTime(DateTime.Now);
    public MaritalStatus MaritalStatus { get; init; }
    public int ChildrenCount { get; init; }
    public string MobilePhone { get; init; }
    public string? LandlinePhone { get; init; }
    public string AddressText { get; init; }
    public bool IsCloseToSalon { get; init; }

    public PersonalInfoRequestDto(
        string fullName,
        string nationalCode,
        DateOnly birthDate,
        MaritalStatus maritalStatus,
        int childrenCount,
        string mobilePhone,
        string? landlinePhone,
        string addressText,
        bool isCloseToSalon)
    {
        FullName = fullName;
        NationalCode = nationalCode;
        BirthDate = birthDate;
        MaritalStatus = maritalStatus;
        ChildrenCount = childrenCount;
        MobilePhone = mobilePhone;
        LandlinePhone = landlinePhone;
        AddressText = addressText;
        IsCloseToSalon = isCloseToSalon;
    }

    public PersonalInfoRequestDto()
    {
    }
}