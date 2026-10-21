using Framwork.PagedList;

namespace Modules.Salon.Application.Contract.DTOs.Salons.GetAll;

public class GetSelectListSalonRequestDto : PagedParamData
{
    public string? Q { get; set; }
}
public class GetSelectListSalonResponseDto
{

    public long Id { get; set; }

    /// <summary>
    /// عنوان برند
    /// </summary>
    public string Name { get; set; } = null!;

    // /// <summary>
    // /// تصویر برند
    // /// </summary>
    // public string? ImageUrl { get; set; }


    public bool Status { get; set; }


}