using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Modules.FileStore.Application.Contract.DTOs.FileUploader;
using Modules.FileStore.Application.Contract.Interface.Services;

namespace Modules.FileStore.Application.Services;

public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageOptions _options;
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(
        IOptions<FileStorageOptions> options,
        IWebHostEnvironment environment)
    {
        _options = options.Value;
        _environment = environment;
    }

    public async Task<StoredFileResult> UploadAsync(
        UploadFileDto file,
        CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid():N}{extension}";

        var categoryFolderName = file.Category.ToString();

        var rootFolder = Path.Combine(
            _environment.WebRootPath,
            _options.RootPath);

        var categoryFolder = Path.Combine(
            rootFolder,
            categoryFolderName);

        Directory.CreateDirectory(categoryFolder);

        var fullPhysicalPath = Path.Combine(
            categoryFolder,
            storedFileName);

        await using var destination = new FileStream(
            fullPhysicalPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);

        await file.Content.CopyToAsync(destination, cancellationToken);

        var relativePath = Path.Combine(
            _options.RootPath,
            categoryFolderName,
            storedFileName).Replace("\\", "/");

        return new StoredFileResult
        {
            OriginalFileName = file.FileName,
            StoredFileName = storedFileName,
            FileExtension = extension,
            ContentType = file.ContentType,
            FilePath = relativePath,
            FileSize = file.Length,
            FileStoreCategory=file.Category,
        };
    }

    public Task DeleteAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var fullPhysicalPath = GetPhysicalPath(filePath);

        if (File.Exists(fullPhysicalPath))
            File.Delete(fullPhysicalPath);

        return Task.CompletedTask;
    }

    public Task<Stream> OpenReadAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var fullPhysicalPath = GetPhysicalPath(filePath);

        Stream stream = new FileStream(
            fullPhysicalPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        return Task.FromResult(stream);
    }

    public bool Exists(string filePath)
    {
        var fullPhysicalPath = GetPhysicalPath(filePath);
        return File.Exists(fullPhysicalPath);
    }

    private string GetPhysicalPath(string filePath)
    {
        filePath = filePath.Replace("/", Path.DirectorySeparatorChar.ToString())
                           .Replace("\\", Path.DirectorySeparatorChar.ToString());

        return Path.Combine(_environment.WebRootPath, filePath);
    }
}
