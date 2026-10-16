using Modules.Banner.Application.Contract.Resources.Banners;
using Modules.Banner.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using static Modules.Banner.Application.Contract.Resources.Banners.BannerFieldNames;

namespace Modules.Banner.Application.Contract.DTOs.Banners.Create;

public class CreateBannerRequestDto
{
    [Display(Name = nameof(BannerFieldNames.Title), ResourceType = typeof(BannerFieldNames))]
    public string Title { get; set; } = null!;

    [Display(Name = nameof(BannerFieldNames.Description), ResourceType = typeof(BannerFieldNames))]
    public string? Description { get; set; }

    [Display(Name = nameof(BannerFieldNames.ImageUrl), ResourceType = typeof(BannerFieldNames))]
    public string? ImageUrl { get; set; }

    [Display(Name = nameof(BannerFieldNames.Url), ResourceType = typeof(BannerFieldNames))]
    public string? Url { get; set; }

    [Display(Name = nameof(BannerFieldNames.Order), ResourceType = typeof(BannerFieldNames))]
    public int Order { get; set; }

    [Display(Name = nameof(BannerFieldNames.StartDate), ResourceType = typeof(BannerFieldNames))]
    public DateTime? StartDate { get; set; }

    [Display(Name = nameof(BannerFieldNames.EndDate), ResourceType = typeof(BannerFieldNames))]
    public DateTime? EndDate { get; set; }


    [Display(Name = nameof(BannerFieldNames.Type), ResourceType = typeof(BannerFieldNames))]
    public BannerType Type { get; set; }
}
