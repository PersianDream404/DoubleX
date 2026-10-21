using Framwork.PagedList;
using Modules.FileStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileStore.Application.Contract.DTOs.FileStores.Get;


public class GetByIdFileStoreResponseDto
{
    public long Id { get; set; }
    /// <summary>
    /// نام اصلی فایل هنگام آپلود توسط کاربر
    /// </summary>
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// نام فایل ذخیره شده در سرور (معمولاً به صورت یکتا مانند GUID)
    /// </summary>
    public string? StoredFileName { get; set; }

    /// <summary>
    /// پسوند فایل (مانند .jpg یا .pdf)
    /// </summary>
    public string? FileExtension { get; set; }

    /// <summary>
    /// نوع محتوای فایل (MIME Type) مانند image/png
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// مسیر ذخیره‌سازی فایل در سیستم
    /// </summary>
    public string? FilePath { get; set; }
    public string? Description { get; set; }

    public FileStoreCategory FileStoreCategory { get; set; }
    public FileProvider FileProvider { get; set; }

    /// <summary>
    /// وضعیت فعال بودن فایل
    /// </summary>
    public bool IsActive { get; set; }
}
