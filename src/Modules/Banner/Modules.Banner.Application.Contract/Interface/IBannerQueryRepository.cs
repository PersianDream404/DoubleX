using Framwork.PagedList;
using Modules.Banner.Application.Contract.DTOs.Banners.Get;
using Modules.Banner.Application.Contract.DTOs.Banners.GetAll;
using Modules.Banner.Domain.Entities;
using SharedKernel.Interface.Repositories;


namespace Modules.Product.Application.Contract.Interface.Banners;

public interface IBannerQueryRepository : IQueryRepository<BannerEntity>
{
    Task<PagedList<GetAllBannerResponseDto>> GetAllProjectedAsync(GetAllBannerRequestDto request, CancellationToken ct);
    Task<PagedList<GetSelectListBannerResponseDto>> GetSelectListProjectedAsync(GetSelectListBannerRequestDto request, CancellationToken ct);
    Task<GetByIdBannerResponseDto?> GetByIdProjectedAsync(long id, CancellationToken ct);
}
