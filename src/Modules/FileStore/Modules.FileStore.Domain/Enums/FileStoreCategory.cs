using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.FileStore.Domain.Enums;

public enum  FileStoreCategory
{
    None,
    Product,
    ProductGallery,
    Banner,
    Blog
}
public enum FileProvider
{
    Local = 1,

    Minio = 2,

    AzureBlob = 3,

    AwsS3 = 4
}