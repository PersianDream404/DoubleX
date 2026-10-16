using Framwork.PagedList;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Modules.Banner.Application.Contract.DTOs.Banners.Get;
using Modules.Banner.Application.Contract.DTOs.Banners.GetAll;
using Modules.Banner.Domain.Entities;
using Modules.Banner.Persistence.Context;
using Modules.Banner.Persistence.Mapper.Banners;
using Modules.Product.Application.Contract.Interface.Banners;

namespace Modules.Banner.Persistence.Repositories.Banners;

public class BannerQueryRepository
    : QueryRepository<BannerEntity>, IBannerQueryRepository
{
    private readonly BannerReadDbContext _dbContext;
    public BannerQueryRepository(BannerReadDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<PagedList<GetAllBannerResponseDto>> GetAllProjectedAsync(GetAllBannerRequestDto request, CancellationToken ct)
    {
        var query = _dbContext.Banners
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(request.Q), x => x.Title.Contains(request.Q!))
            .WhereIf(request.Type.HasValue, x => x.Type==request.Type)
            ;

        var result = await query.ToPagedListAsync(
            BannerMapper.ToGetAllDto(),
            request.PageNumber,
            request.PageSize,
            ct);

        return result;
    }

    public async Task<PagedList<GetSelectListBannerResponseDto>> GetSelectListProjectedAsync(GetSelectListBannerRequestDto request, CancellationToken ct)
    {
        var query = _dbContext.Banners
            .AsNoTracking()
            .Where(x=>x.Status)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Q), x => x.Title.Contains(request.Q!));

        var result = await query.ToPagedListAsync(
            BannerMapper.ToGetSelectListDto(),
            request.PageNumber,
            request.PageSize,
            ct);

        return result;
    }
    public async Task<GetByIdBannerResponseDto?> GetByIdProjectedAsync(long id, CancellationToken ct)
    {
        return await _dbContext.Banners
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(BannerMapper.ToGetByIdDto())
            .FirstOrDefaultAsync(ct);
    }

}
