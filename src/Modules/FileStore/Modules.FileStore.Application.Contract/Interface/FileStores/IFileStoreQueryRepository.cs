using Framwork.PagedList;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Get;
using Modules.FileStore.Application.Contract.DTOs.FileStores.GetAll;
using SharedKernel.Interface.Repositories;

namespace Modules.FileStore.Application.Contract.Interface.FileStores;

public interface IFileStoreQueryRepository : IQueryRepository<Domain.Entities.FileStores.FileStore>
{
    Task<PagedList<GetAllFileStoreResponseDto>> GetAllProjectedAsync(GetAllFileStoreRequestDto request, CancellationToken ct);
    Task<GetByIdFileStoreResponseDto?> GetByIdProjectedAsync(long id, CancellationToken ct);
}
