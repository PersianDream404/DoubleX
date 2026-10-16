using Framwork.PagedList;
using Modules.Banner.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Banner.Application.Contract.DTOs.Banners.Get;


public class GetByIdBannerResponseDto
{

    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Url { get; set; }
    public int Order { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public BannerType Type { get; set; }
    public bool Status { get; set; }

}
