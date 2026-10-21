using Framwork.PagedList;
using Modules.Salon.Application.Contract.DTOs.Salons.Get;
using Modules.Salon.Application.Contract.DTOs.Salons.GetAll;

using SharedKernel.Interface.Repositories;


namespace Modules.Product.Application.Contract.Interface.Salons;

public interface ISalonQueryRepository : IQueryRepository<Personnel.Domain.Entities.Salon>
{
    Task<PagedList<GetAllSalonResponseDto>> GetAllProjectedAsync(GetAllSalonRequestDto request, CancellationToken ct);
    Task<PagedList<GetSelectListSalonResponseDto>> GetSelectListProjectedAsync(GetSelectListSalonRequestDto request, CancellationToken ct);
    Task<GetByIdSalonResponseDto?> GetByIdProjectedAsync(long id, CancellationToken ct);
}
