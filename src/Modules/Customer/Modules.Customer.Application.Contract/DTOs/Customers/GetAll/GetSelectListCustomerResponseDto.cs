namespace Modules.Customer.Application.Contract.DTOs.Customers.GetAll;

public sealed class GetSelectListCustomerResponseDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = null!;
    public bool Status { get; set; }
}
