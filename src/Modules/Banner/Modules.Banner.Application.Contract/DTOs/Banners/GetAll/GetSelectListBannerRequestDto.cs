using Framwork.PagedList;

namespace Modules.Banner.Application.Contract.DTOs.Banners.GetAll;

public class GetSelectListBannerRequestDto : PagedParamData
{
    public string? Q { get; set; }
}
public class GetSelectListBannerResponseDto
{

    public long Id { get; set; }

    /// <summary>
    /// عنوان برند
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// تصویر برند
    /// </summary>
    public string? ImageUrl { get; set; }


    public bool Status { get; set; }


}