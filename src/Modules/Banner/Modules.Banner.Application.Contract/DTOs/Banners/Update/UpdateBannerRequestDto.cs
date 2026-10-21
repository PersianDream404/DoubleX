using Modules.Banner.Application.Contract.DTOs.Banners.Create;
using Modules.Banner.Application.Contract.Resources.Banners;
using System.ComponentModel.DataAnnotations;

namespace Modules.Banner.Application.Contract.DTOs.Banners.Update;

public class UpdateBannerRequestDto: CreateBannerRequestDto
{
    [Display(
    Name = nameof(BannerFieldNames.Id),
    ResourceType = typeof(BannerFieldNames)
)]
    public long Id { get; set; }
}
