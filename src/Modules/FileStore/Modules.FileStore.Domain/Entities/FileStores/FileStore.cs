using Modules.FileStore.Domain.Enums;
using SharedKernel.Base;

namespace Modules.FileStore.Domain.Entities.FileStores;

/// <summary>
/// نگهداری اطلاعات فایل‌های آپلود شده در سیستم
/// </summary>
public class FileStore : BaseEntityEmpty
{
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
    /// <summary>
    /// توضیحات
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// اندازه فایل بر حسب بایت
    /// </summary>
    public long FileSize { get; set; }
    public FileStoreCategory FileStoreCategory { get; set; }
    public FileProvider FileProvider { get; set; }

    /// <summary>
    /// وضعیت فعال بودن فایل
    /// </summary>
    public bool IsActive { get; set; }

    public DateTime UploadDate { get; set; }
}
