using Modules.FileStore.Application.Contract.DTOs.FileUploader;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileStore.Application.Contract.Interface.Services;

public interface IFileStorageService
{
    Task<StoredFileResult> UploadAsync(
        UploadFileDto file,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string filePath,
        CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(
        string filePath,
        CancellationToken cancellationToken = default);

    bool Exists(string filePath);
}