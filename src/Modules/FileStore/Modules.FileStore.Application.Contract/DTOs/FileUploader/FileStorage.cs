using Modules.FileStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileStore.Application.Contract.DTOs.FileUploader;

public sealed class FileStorageOptions
{
    public string RootPath { get; set; } = "uploads";
}

public  class StoredFileResult
{
    public string OriginalFileName { get; set; } = default!;

    public string StoredFileName { get; set; } = default!;

    public string FileExtension { get; set; } = default!;

    public string ContentType { get; set; } = default!;

    public string FilePath { get; set; } = default!;

    public long FileSize { get; set; }
    public FileStoreCategory FileStoreCategory { get; set; }
}

public  class UploadFileDto
{
    public string FileName { get; set; } = default!;

    public string ContentType { get; set; } = default!;

    public long Length { get; set; }

    public Stream Content { get; set; } = default!;

    public FileStoreCategory Category { get; set; }
}