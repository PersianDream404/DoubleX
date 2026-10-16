using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Modules.Banner.Domain.Enums;


public enum BannerType
{
    [Display(Name = "متفرقه")]
    None = 0,
    [Display(Name = "TopBar")]
    TopBar = 1,
    [Display(Name = "Banner")]
    Banner = 2
}
