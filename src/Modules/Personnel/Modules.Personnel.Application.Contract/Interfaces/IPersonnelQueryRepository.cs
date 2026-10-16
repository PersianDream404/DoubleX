using Framwork.PagedList;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using SharedKernel.Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Personnel.Application.Contract.Interfaces;

public interface IPersonnelQueryRepository : IQueryRepository<Domain.Entities.Personnel>
{
    Task<GetPersonnelDto?> GetByIdDtoAsync(long id, CancellationToken ct = default);
    Task<PagedList<GetAllPersonalResponseDto>> GetAllProjectedAsync(GetAllPersonnelRequestDto request, CancellationToken ct);
}
