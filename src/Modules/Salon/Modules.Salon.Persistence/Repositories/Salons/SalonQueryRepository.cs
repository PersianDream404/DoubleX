using Framwork.PagedList;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Modules.Personnel.Persistence.Context;
using Modules.Salon.Application.Contract.DTOs.Salons.Get;
using Modules.Salon.Application.Contract.DTOs.Salons.GetAll;

using Modules.Product.Application.Contract.Interface.Salons;
using Modules.Salon.Persistence.Mapper.Salons;

namespace Modules.Salon.Persistence.Repositories.Salons;

public class SalonQueryRepository
    : QueryRepository<Personnel.Domain.Entities.Salon>, ISalonQueryRepository
{
    private readonly PersonnelReadDbContext _dbContext;
    public SalonQueryRepository(PersonnelReadDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<PagedList<GetAllSalonResponseDto>> GetAllProjectedAsync(GetAllSalonRequestDto request, CancellationToken ct)
    {
        var query = _dbContext.Salons
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(request.Q), x => x.Name.Contains(request.Q!))
          
            ;

        var result = await query.ToPagedListAsync(
            SalonMapper.ToGetAllDto(),
            request.PageNumber,
            request.PageSize,
            ct);

        return result;
    }

    public async Task<PagedList<GetSelectListSalonResponseDto>> GetSelectListProjectedAsync(GetSelectListSalonRequestDto request, CancellationToken ct)
    {
        var query = _dbContext.Salons
            .AsNoTracking()
            .Where(x=>x.Status)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Q), x => x.Name.Contains(request.Q!));

        var result = await query.ToPagedListAsync(
            SalonMapper.ToGetSelectListDto(),
            request.PageNumber,
            request.PageSize,
            ct);

        return result;
    }
    public async Task<GetByIdSalonResponseDto?> GetByIdProjectedAsync(long id, CancellationToken ct)
    {
        return await _dbContext.Salons
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(SalonMapper.ToGetByIdDto())
            .FirstOrDefaultAsync(ct);
    }

}
