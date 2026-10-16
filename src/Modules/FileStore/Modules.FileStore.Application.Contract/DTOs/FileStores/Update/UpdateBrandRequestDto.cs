using Modules.FileStore.Application.Contract.DTOs.FileStores.Create;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileStore.Application.Contract.DTOs.FileStores.Update;

public class UpdateFileStoreRequestDto: CreateFileStoreRequestDto
{
    public long Id { get; set; }
}
