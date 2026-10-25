using Framwork.PagedList;

namespace Modules.Inventory.Application.Contract.DTOs.Materials;

public sealed class GetMaterialsRequestDto : PagedParamData
{
    public string? Q { get; set; }
    public bool? IsActive { get; set; }
}
