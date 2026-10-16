using Framwork.PagedList;
using Modules.FileStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileStore.Application.Contract.DTOs.FileStores.GetAll;

public class GetAllFileStoreRequestDto : PagedParamData
{
    public string? Q { get; set; }
    public FileStoreCategory? FileStoreCategory { get; set; }
    public FileProvider? FileProvider { get; set; }
}
public class GetAllFileStoreResponseDto
{
    public long Id { get; set; }
    public string? FilePath { get; set; }
    public string? Description { get; set; }

    public FileStoreCategory FileStoreCategory { get; set; }
    public FileProvider FileProvider { get; set; }

    /// <summary>
    /// وضعیت فعال بودن فایل
    /// </summary>
    public bool IsActive { get; set; }

}
