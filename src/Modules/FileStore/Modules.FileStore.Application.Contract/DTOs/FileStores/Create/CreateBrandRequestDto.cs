using Modules.FileStore.Application.Contract.DTOs.FileUploader;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileStore.Application.Contract.DTOs.FileStores.Create;

public class CreateFileStoreRequestDto: UploadFileDto
{

  

}
public class CreateFileStoreResponseDto
{

    public long Id { get; set; }
    public string Path { get; set; } = null!;

}