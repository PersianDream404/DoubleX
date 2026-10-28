using Ardalis.Result;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Enums;

namespace Modules.Personnel.Application.UseCase.Personnel;

internal static class PersonnelContractFactory
{
    public static CooperationType? GetCooperationType(SalaryType salaryType) =>
        salaryType switch
        {
            SalaryType.Fixed => CooperationType.FixedSalary,
            SalaryType.Commission => CooperationType.Commission,
            SalaryType.FixedAndCommission => CooperationType.FixedAndCommission,
            SalaryType.ChairRental => CooperationType.ChairRental,
            SalaryType.RoomRental => CooperationType.RoomRental,
            _ => null
        };

    public static Result<PersonnelContract> Create(
        global::Modules.Personnel.Domain.Entities.Personnel personnel,
        FinancialInfoRequestDto request,
        DateOnly fallbackEffectiveFrom)
    {
        var cooperationType = GetCooperationType(request.SalaryType);
        if (!cooperationType.HasValue)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(request.SalaryType),
                ErrorMessage = "نوع همکاری باید ثابت، پورسانتی یا ترکیبی باشد."
            });
        }

        return PersonnelContract.Create(
            personnel,
            cooperationType.Value,
            request.FixedSalaryAmount,
            request.CommissionRate,
            request.EffectiveFrom ?? fallbackEffectiveFrom,
            request.EffectiveTo);
    }
}
