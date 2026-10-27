using Framwork.PagedList;

namespace Modules.CashRegister.Application.Contract.DTOs.CashRegisters;

public sealed class CashRegisterListRequestDto : PagedParamData
{
    public long? SalonId { get; set; }
    public string? Q { get; set; }
    public bool? IsActive { get; set; }
}
