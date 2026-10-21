using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Get;
using Modules.FileStore.Application.Contract.DTOs.FileStores.GetAll;

namespace Modules.FileStore.Application.Contract.UseCase.FileStores.Queries;

public record GetAllFileStoreQuery(GetAllFileStoreRequestDto request) : 
    IQuery<PagedList<GetAllFileStoreResponseDto>>;
public record GetByIdFileStoreQuery(long Id) : IQuery<GetByIdFileStoreResponseDto>;
