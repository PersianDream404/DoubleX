using Framwork.PagedList;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using SharedKernel.Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Personnel.Application.Contract.Interfaces;

public interface IPersonnelQueryRepository : IQueryRepository<Modules.Personnel.Domain.Entities.Personnel>
{
    Task<GetByIdPersonnelResponseDto?> GetByIdDtoAsync(long id, CancellationToken ct = default);
    Task<PagedList<GetAllPersonalResponseDto>> GetAllProjectedAsync(GetAllPersonnelRequestDto request, CancellationToken ct);
    Task<IReadOnlyList<PersonnelReceptionLookupDto>> GetReceptionLookupsAsync(
        long salonId,
        IReadOnlyCollection<long> personnelIds,
        CancellationToken cancellationToken);
}
