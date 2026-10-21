using Framwork.PagedList;

using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Salon.Application.Contract.DTOs.Salons.GetAll;

public class GetAllSalonRequestDto : PagedParamData
{
    public string? Q { get; set; }

}
public class GetAllSalonResponseDto
{

    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Status { get; set; }
}
