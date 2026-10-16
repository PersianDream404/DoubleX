using Framwork.PagedList;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Modules.Banner.Persistence.Mapper.Banners;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Interface;
using Modules.Personnel.Persistence.Context;

namespace Modules.Personnel.Persistence.Repositories;

public sealed class PersonnelQueryRepository : QueryRepository<Domain.Entities.Personnel>,  IPersonnelQueryRepository
{
    private readonly PersonnelReadDbContext _dbContext;

    public PersonnelQueryRepository(PersonnelReadDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<GetPersonnelDto?> GetByIdDtoAsync(long id, CancellationToken ct = default)
    {
        return await _dbContext.Personnels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => x.Adapt<GetPersonnelDto>())
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PagedList<GetAllPersonalResponseDto>> GetAllProjectedAsync(GetAllPersonnelRequestDto request, CancellationToken ct)
    {
        var query = _dbContext.Personnels
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(request.Q), x => x.PersonalInfo.FullName.Contains(request.Q!));

        var result = await query.ToPagedListAsync(
            PersonalMapper.ToGetAllDto(),
            request.PageNumber,
            request.PageSize,
            ct);

        return result;
    }
}
