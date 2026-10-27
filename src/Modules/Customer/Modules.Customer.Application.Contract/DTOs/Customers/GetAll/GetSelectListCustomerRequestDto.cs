using Framwork.PagedList;

namespace Modules.Customer.Application.Contract.DTOs.Customers.GetAll;

public sealed class GetSelectListCustomerRequestDto : PagedParamData
{
    public long? SalonId { get; set; }
    public string? Q { get; set; }
}
