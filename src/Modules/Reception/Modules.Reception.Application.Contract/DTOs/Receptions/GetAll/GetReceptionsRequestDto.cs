using Framwork.PagedList;

namespace Modules.Reception.Application.Contract.DTOs.Receptions.GetAll;

public sealed class GetReceptionsRequestDto : PagedParamData
{
    public long? SalonId { get; set; }
    public long? CustomerId { get; set; }
    public bool? UnpaidOnly { get; set; }
}
