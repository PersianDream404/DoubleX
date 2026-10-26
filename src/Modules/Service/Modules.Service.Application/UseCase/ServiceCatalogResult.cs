using Ardalis.Result;
using Modules.Service.Application.Contract.Enums;

namespace Modules.Service.Application.UseCase;

internal static class ServiceCatalogResult
{
    public static Result<bool> ToResult(ServiceOperationStatus status) => status switch
    {
        ServiceOperationStatus.Succeeded => Result.Success(true),
        ServiceOperationStatus.NotFound => Result.NotFound("مورد موردنظر پیدا نشد."),
        ServiceOperationStatus.Conflict => Result.Conflict("با این اطلاعات رکورد تکراری وجود دارد."),
        ServiceOperationStatus.InvalidOwnership => Result.Error("خدمت یا دسته‌بندی به سالن انتخاب‌شده تعلق ندارد یا فعال نیست."),
        ServiceOperationStatus.InactiveMaterial => Result.Error("ماده انتخاب‌شده غیرفعال است."),
        ServiceOperationStatus.InvalidTariffValidity => Result.Invalid(
            new ValidationError("پایان اعتبار تعرفه باید بعد از شروع اعتبار باشد.")),
        ServiceOperationStatus.InvalidPersonnel => Result.Invalid(
            new ValidationError("پرسنل فعال و متعلق به سالن انتخاب‌شده نیست.")),
        _ => Result.Error("عملیات انجام نشد.")
    };
}
