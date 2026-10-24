namespace Modules.Customer.Application.Contract.DTOs.Customers.GetAll;

public class CustomerListItemDto
{
    public long Id { get; set; }
    public long SalonId { get; set; }
    public string FullName { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
    public string NationalCode { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? AllergyDescription { get; set; }
    public string? JobTitle { get; set; }
    public string? LandlinePhone { get; set; }
    public string MobilePhone { get; set; } = null!;
    public bool Status { get; set; }
}
