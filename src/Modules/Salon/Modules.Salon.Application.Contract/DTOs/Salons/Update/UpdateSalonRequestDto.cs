using Modules.Salon.Application.Contract.DTOs.Salons.Create;

using System.ComponentModel.DataAnnotations;
using Modules.Banner.Application.Contract.Resources.Banners;

namespace Modules.Salon.Application.Contract.DTOs.Salons.Update;

public class UpdateSalonRequestDto: CreateSalonRequestDto
{
    [Display(
    Name = nameof(SalonFieldNames.Id),
    ResourceType = typeof(SalonFieldNames)
)]
    public long Id { get; set; }
}
