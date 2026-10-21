using Mapster;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Create;
using Modules.FileStore.Application.Contract.DTOs.FileStores.Update;
using Modules.FileStore.Application.Contract.DTOs.FileUploader;

namespace Modules.FileStore.Application.Common.Mapping;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        //#region Products

        //TypeAdapterConfig<CreateProductRequestDto, Product>.NewConfig()
        //     .Map(dest => dest.ProductSelectedCategories, src => src.CategoriesIds.Select(x=>new ProductSelectedCategory { ProductCategoryId=x}))
        //     .Map(dest => dest.ProductDiscounts, src => src.DiscountsIds.Select(x=>new ProductDiscountUse { ProductDiscountId=x}))
        //     .Map(dest => dest.ProductSelectedColors, src => src.SelectedColorsIds.Select(x=>new ProductSelectedColors { ProductColorId=x}))
        //    ;
        //#endregion
        TypeAdapterConfig<CreateFileStoreRequestDto, UploadFileDto>
            .NewConfig()
             .Map(dest => dest.Category, src => src.Category)
            .Ignore(dest => dest.Content);

        TypeAdapterConfig<UpdateFileStoreRequestDto, UploadFileDto>
            .NewConfig()
            .Ignore(dest => dest.Content);

        TypeAdapterConfig<UploadFileDto, FileStore.Domain.Entities.FileStores.FileStore>
            .NewConfig()
             .Map(dest => dest.FileStoreCategory, src => src.Category)
             .Map(dest => dest.UploadDate, src => DateTime.Now);

    }
}
