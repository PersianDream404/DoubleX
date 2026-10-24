using SharedKernel.Base;

namespace Modules.Customer.Domain.Entities;

public sealed class Customer : BaseEntityIdentity
{
    public string FullName { get; private set; } = null!;
    public DateOnly BirthDate { get; private set; }
    public string NationalCode { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string? AllergyDescription { get; private set; }
    public string? JobTitle { get; private set; }
    public string? LandlinePhone { get; private set; }
    public string MobilePhone { get; private set; } = null!;
    public long SalonId { get; private set; }

    private Customer()
    {
    }

    private Customer(
        string fullName,
        DateOnly birthDate,
        string nationalCode,
        string address,
        string? allergyDescription,
        string? jobTitle,
        string? landlinePhone,
        string mobilePhone,
        long salonId)
    {
        FullName = fullName;
        BirthDate = birthDate;
        NationalCode = nationalCode;
        Address = address;
        AllergyDescription = allergyDescription;
        JobTitle = jobTitle;
        LandlinePhone = landlinePhone;
        MobilePhone = mobilePhone;
        SalonId = salonId;
    }

    public static Customer Create(
        string fullName,
        DateOnly birthDate,
        string nationalCode,
        string address,
        string? allergyDescription,
        string? jobTitle,
        string? landlinePhone,
        string mobilePhone,
        long salonId) =>
        new(
            fullName,
            birthDate,
            nationalCode,
            address,
            allergyDescription,
            jobTitle,
            landlinePhone,
            mobilePhone,
            salonId);

    public void Update(
        string fullName,
        DateOnly birthDate,
        string nationalCode,
        string address,
        string? allergyDescription,
        string? jobTitle,
        string? landlinePhone,
        string mobilePhone,
        long salonId)
    {
        FullName = fullName;
        BirthDate = birthDate;
        NationalCode = nationalCode;
        Address = address;
        AllergyDescription = allergyDescription;
        JobTitle = jobTitle;
        LandlinePhone = landlinePhone;
        MobilePhone = mobilePhone;
        SalonId = salonId;
    }
}
