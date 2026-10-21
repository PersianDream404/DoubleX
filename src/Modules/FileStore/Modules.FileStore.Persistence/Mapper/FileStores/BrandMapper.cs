using Modules.FileStore.Application.Contract.DTOs.FileStores.Get;
using Modules.FileStore.Application.Contract.DTOs.FileStores.GetAll;
using Modules.FileStore.Domain.Entities;
using Modules.FileStore.Domain.Enums;
using System;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Modules.FileStore.Application.Contract.Mappers;

public static class FileStoreMapper
{
    public static Expression<Func<Modules.FileStore.Domain.Entities.FileStores.FileStore, GetByIdFileStoreResponseDto>> ToGetByIdDto()
    {
        return x => new GetByIdFileStoreResponseDto
        {
            Id = x.Id,
            OriginalFileName = x.OriginalFileName,
            StoredFileName = x.StoredFileName,
            FileExtension = x.FileExtension,
            ContentType = x.ContentType,
            FilePath = x.FilePath,
            Description = x.Description,
            FileStoreCategory = x.FileStoreCategory,
            FileProvider = x.FileProvider,
            IsActive = x.IsActive
        };
    }

    public static Expression<Func<Modules.FileStore.Domain.Entities.FileStores.FileStore, GetAllFileStoreResponseDto>> ToGetAllDto()
    {
        return x => new GetAllFileStoreResponseDto
        {
            Id=x.Id,
            FilePath = x.FilePath,
            Description = x.Description,
            FileStoreCategory = x.FileStoreCategory,
            FileProvider = x.FileProvider,
            IsActive = x.IsActive
        };
    }
}
