using Framwork.PagedList;
using Identity.Persistence.Context;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Get;
using Modules.FileStore.Application.Contract.DTOs.FileStores.GetAll;
using Modules.FileStore.Application.Contract.Interface.FileStores;
using Modules.FileStore.Application.Contract.Mappers;

namespace Modules.FileStore.Persistence.Repositories.FileStores;

public class FileStoreQueryRepository
    : QueryRepository<Domain.Entities.FileStores.FileStore>, IFileStoreQueryRepository
{
    private readonly FileStoreReadDbContext _dbContext;
    public FileStoreQueryRepository(FileStoreReadDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<PagedList<GetAllFileStoreResponseDto>> GetAllProjectedAsync(
        GetAllFileStoreRequestDto request,
        CancellationToken ct)
    {
        var query = _dbContext.FileStores
            .AsNoTracking()
            .WhereIf(request.FileProvider.HasValue,x=>x.FileProvider==request.FileProvider)
            .WhereIf(request.FileStoreCategory.HasValue,x=>x.FileStoreCategory == request.FileStoreCategory)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Q),
                x => (x.OriginalFileName != null && x.OriginalFileName.Contains(request.Q!)) ||
                     (x.StoredFileName != null && x.StoredFileName.Contains(request.Q!)) ||
                     (x.Description != null && x.Description.Contains(request.Q!)) ||
                     (x.FilePath != null && x.FilePath.Contains(request.Q!)));

        var result = await query.ToPagedListAsync(
            FileStoreMapper.ToGetAllDto(),
            request.PageNumber,
            request.PageSize,
            ct);

        return result;
    }

    public async Task<GetByIdFileStoreResponseDto?> GetByIdProjectedAsync(long id, CancellationToken ct)
    {
        return await _dbContext.FileStores
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(FileStoreMapper.ToGetByIdDto())
            .FirstOrDefaultAsync(ct);
    }

}
