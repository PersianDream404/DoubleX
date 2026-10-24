using System.ComponentModel.DataAnnotations;
using Modules.Customer.Application.Contract.Resources.Customers;

namespace Modules.Customer.Application.Contract.DTOs.Customers.Update;

public sealed class UpdateCustomerRequestDto
    : Modules.Customer.Application.Contract.DTOs.Customers.CustomerRequestDto
{
    [Display(Name = nameof(CustomerFieldNames.Id), ResourceType = typeof(CustomerFieldNames))]
    public long Id { get; set; }
}
