using Framwork.PagedList;

namespace Modules.Inventory.Application.Contract.DTOs.Units;

public sealed class GetUnitsRequestDto : PagedParamData
{
    public string? Q { get; set; }
    public bool? IsActive { get; set; }
}
