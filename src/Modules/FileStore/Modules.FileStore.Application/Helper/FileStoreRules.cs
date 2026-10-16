using Modules.FileStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileStore.Application.Helper;

public static class FileStoreRules
{
    private static readonly Dictionary<FileStoreCategory, string[]> AllowedExtensions =
        new()
        {
            { FileStoreCategory.Product, [".jpg", ".jpeg", ".png", ".webp"] },
            { FileStoreCategory.ProductGallery, [".jpg", ".jpeg", ".png", ".webp"] },
            { FileStoreCategory.Banner, [".jpg", ".jpeg", ".png", ".webp"] },
            { FileStoreCategory.Blog, [".jpg", ".jpeg", ".png", ".webp", ".pdf"] }
        };

    private static readonly Dictionary<FileStoreCategory, long> MaxFileSize =
        new()
        {
            { FileStoreCategory.Product, 5 * 1024 * 1024 },
            { FileStoreCategory.ProductGallery, 10 * 1024 * 1024 },
            { FileStoreCategory.Banner, 5 * 1024 * 1024 },
            { FileStoreCategory.Blog, 20 * 1024 * 1024 }
        };

    public static bool IsValidExtension(
        FileStoreCategory category,
        string extension)
    {
        return AllowedExtensions.TryGetValue(category, out var extensions)
               && extensions.Contains(extension.ToLowerInvariant());
    }

    public static bool IsValidSize(
        FileStoreCategory category,
        long fileSize)
    {
        return MaxFileSize.TryGetValue(category, out var maxSize)
               && fileSize <= maxSize;
    }

    public static long GetMaxFileSize(FileStoreCategory category)
    {
        return MaxFileSize.GetValueOrDefault(category);
    }
}