using System.ComponentModel.DataAnnotations;
using Modules.Customer.Application.Contract.Resources.Customers;

namespace Modules.Customer.Application.Contract.DTOs.Customers;

public class CustomerRequestDto
{
    [Display(Name = nameof(CustomerFieldNames.FullName), ResourceType = typeof(CustomerFieldNames))]
    [Required, MaxLength(250)]
    public string FullName { get; set; } = null!;

    [Display(Name = nameof(CustomerFieldNames.BirthDate), ResourceType = typeof(CustomerFieldNames))]
    public DateOnly BirthDate { get; set; }

    [Display(Name = nameof(CustomerFieldNames.NationalCode), ResourceType = typeof(CustomerFieldNames))]
    [Required, StringLength(10, MinimumLength = 10)]
    public string NationalCode { get; set; } = null!;

    [Display(Name = nameof(CustomerFieldNames.Address), ResourceType = typeof(CustomerFieldNames))]
    [Required, MaxLength(1000)]
    public string Address { get; set; } = null!;

    [Display(Name = nameof(CustomerFieldNames.AllergyDescription), ResourceType = typeof(CustomerFieldNames))]
    [MaxLength(1000)]
    public string? AllergyDescription { get; set; }

    [Display(Name = nameof(CustomerFieldNames.JobTitle), ResourceType = typeof(CustomerFieldNames))]
    [MaxLength(150)]
    public string? JobTitle { get; set; }

    [Display(Name = nameof(CustomerFieldNames.LandlinePhone), ResourceType = typeof(CustomerFieldNames))]
    [MaxLength(20)]
    public string? LandlinePhone { get; set; }

    [Display(Name = nameof(CustomerFieldNames.MobilePhone), ResourceType = typeof(CustomerFieldNames))]
    [Required, MaxLength(20)]
    public string MobilePhone { get; set; } = null!;

    [Display(Name = nameof(CustomerFieldNames.SalonId), ResourceType = typeof(CustomerFieldNames))]
    [Range(1, long.MaxValue)]
    public long SalonId { get; set; }
}
