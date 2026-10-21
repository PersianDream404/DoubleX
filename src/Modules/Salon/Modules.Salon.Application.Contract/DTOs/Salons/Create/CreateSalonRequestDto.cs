
using System;
using System.ComponentModel.DataAnnotations;
using Modules.Banner.Application.Contract.Resources.Banners;

namespace Modules.Salon.Application.Contract.DTOs.Salons.Create;

public class CreateSalonRequestDto
{
    [Display(Name = nameof(SalonFieldNames.Name), ResourceType = typeof(SalonFieldNames))]
    public string Name { get; set; } = null!;


}
