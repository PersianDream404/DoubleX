using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Modules.Personnel.Application.Common;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Enums;
using Modules.Personnel.Domain.Interface;
using Modules.Personnel.Domain.ValueObjects;
using Modules.Product.Application.Contract.Interface.Salons;

namespace Modules.Personnel.Application.UseCase.Personnel.Commands;

public class UpdatePersonnelCommandHandler(
    IPersonnelCommandRepository PersonnelCommandRepository,
    IPersonnelQueryRepository PersonnelQueryRepository,
    ISalonQueryRepository SalonQueryRepository)
    : ICommandHandler<UpdatePersonnelCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdatePersonnelCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (command.Id <= 0)
                return Result.Invalid(new ValidationError
                {
                    Identifier = nameof(command.Id),
                    ErrorMessage = "شناسه پرسنل نامعتبر است."
                });

            var existing = await PersonnelQueryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (existing is null)
                return Result.Error(Messages.PersonnelNotFound);

            var request = command.Request;
            if (request is null || request.PersonalInfo is null || request.JobInfo is null
                || request.FinancialInfo is null || request.HealthInfo is null
                || request.GuarantorInfo is null)
            {
                return Result.Invalid(new ValidationError
                {
                    Identifier = nameof(command.Request),
                    ErrorMessage = "اطلاعات پرسنل کامل نیست."
                });
            }

            if (request.SalonId <= 0
                || await SalonQueryRepository.GetByIdAsync(request.SalonId, cancellationToken) is null)
            {
                return Result.Invalid(new ValidationError
                {
                    Identifier = nameof(request.SalonId),
                    ErrorMessage = "سالن انتخاب‌شده یافت نشد."
                });
            }

            if (existing.SalonId != request.SalonId)
                return Result.Error(Messages.PersonnelNotFound);

            var salonResult = existing.AssignToSalon(request.SalonId);
            if (!salonResult.IsSuccess)
                return Result.Invalid(salonResult.ValidationErrors);

            existing.Status = request.Status;

            var personalInfoResult = PersonalInfo.Create(
                request.PersonalInfo.FullName,
                request.PersonalInfo.NationalCode,
                request.PersonalInfo.BirthDate,
                request.PersonalInfo.MaritalStatus,
                request.PersonalInfo.ChildrenCount,
                request.PersonalInfo.MobilePhone,
                request.PersonalInfo.LandlinePhone,
                request.PersonalInfo.AddressText,
                request.PersonalInfo.IsCloseToSalon,
                request.PersonalInfo.ClearPicture
                    ? null
                    : request.PersonalInfo.PictureUrl ?? existing.PersonalInfo.Picture?.Value);
            if (!personalInfoResult.IsSuccess)
                return Result.Invalid(personalInfoResult.ValidationErrors);

            existing.PersonalInfo.Update(
                personalInfoResult.Value.FullName,
                personalInfoResult.Value.NationalCode,
                personalInfoResult.Value.BirthDate,
                personalInfoResult.Value.MaritalStatus,
                personalInfoResult.Value.ChildrenCount ?? 0,
                personalInfoResult.Value.MobilePhone,
                personalInfoResult.Value.LandlinePhone,
                personalInfoResult.Value.Address,
                personalInfoResult.Value.Picture);

            var jobInfoUpdateResult = existing.JobInfo.Update(
                request.JobInfo.JobTitle,
                request.JobInfo.PersonnelCode,
                request.JobInfo.CooperationStartDate,
                request.JobInfo.WorkingShiftType,
                request.JobInfo.JobPictures ?? [],
                request.JobInfo.WorkHistory);
            if (!jobInfoUpdateResult.IsSuccess)
                return Result.Invalid(jobInfoUpdateResult.ValidationErrors);

            var financialInfoUpdateResult = existing.FinancialInfo.Update(
                request.FinancialInfo.CardNumber,
                request.FinancialInfo.AccountNumber);
            if (!financialInfoUpdateResult.IsSuccess)
                return Result.Invalid(financialInfoUpdateResult.ValidationErrors);

            var profile = await PersonnelQueryRepository.GetByIdDtoAsync(command.Id, cancellationToken);
            var activeContract = profile?.Contracts.SingleOrDefault(contract => contract.IsActive);
            var contractRequest = request.FinancialInfo;
            if (!contractRequest.EffectiveFrom.HasValue && activeContract is not null)
            {
                var requestedCooperationType =
                    PersonnelContractFactory.GetCooperationType(contractRequest.SalaryType);
                var termsUnchanged =
                    requestedCooperationType == activeContract.CooperationType &&
                    contractRequest.FixedSalaryAmount == activeContract.FixedSalaryAmount &&
                    contractRequest.CommissionRate == activeContract.DefaultCommissionRate &&
                    (!contractRequest.EffectiveTo.HasValue ||
                     contractRequest.EffectiveTo == activeContract.EffectiveTo);
                if (!termsUnchanged)
                {
                    return Result.Invalid(new ValidationError
                    {
                        Identifier = nameof(request.FinancialInfo.EffectiveFrom),
                        ErrorMessage = "برای تغییر شرایط همکاری، تاریخ شروع قرارداد جدید را مشخص کنید."
                    });
                }

                contractRequest = new FinancialInfoRequestDto(
                    request.FinancialInfo.SalaryType,
                    request.FinancialInfo.CardNumber,
                    request.FinancialInfo.AccountNumber)
                {
                    FixedSalaryAmount = request.FinancialInfo.FixedSalaryAmount,
                    CommissionRate = request.FinancialInfo.CommissionRate,
                    EffectiveFrom = activeContract.EffectiveFrom,
                    EffectiveTo = request.FinancialInfo.EffectiveTo ?? activeContract.EffectiveTo
                };
            }

            var contractResult = PersonnelContractFactory.Create(
                existing,
                contractRequest,
                request.JobInfo.CooperationStartDate);
            if (!contractResult.IsSuccess)
                return Result.Invalid(contractResult.ValidationErrors);

            if (!TryParseStatus(
                    request.HealthInfo.HealthStatus,
                    existing.HealthInfo.HealthStatus,
                    out var healthStatus)
                || !TryParseStatus(
                    request.HealthInfo.AllergyStatus,
                    existing.HealthInfo.AllergyStatus,
                    out var allergyStatus))
            {
                return Result.Invalid(new ValidationError
                {
                    Identifier = nameof(request.HealthInfo),
                    ErrorMessage = "وضعیت سلامت و حساسیت باید true یا false باشد."
                });
            }

            var healthInfoUpdateResult = existing.HealthInfo.Update(
                healthStatus,
                allergyStatus,
                request.HealthInfo.Diseases,
                request.HealthInfo.Allergies,
                request.Documents is null
                    ? existing.HealthInfo.PictureCourse
                    : GetDocumentPictures(request.Documents, DocumentType.HealthCourse),
                request.Documents is null
                    ? existing.HealthInfo.PictureTest
                    : GetDocumentPictures(
                        request.Documents,
                        DocumentType.MedicalDocument,
                        DocumentType.HealthTest));
            if (!healthInfoUpdateResult.IsSuccess)
                return Result.Invalid(healthInfoUpdateResult.ValidationErrors);

            var guarantorInfoUpdateResult = existing.GuarantorInfo.Update(
                request.GuarantorInfo.GuarantorName,
                request.GuarantorInfo.Relationship,
                request.GuarantorInfo.GuarantorPhone,
                request.GuarantorInfo.GuarantorLandlinePhone,
                request.Documents is null
                    ? existing.GuarantorInfo.PictureGuaranties
                    : GetDocumentPictures(
                        request.Documents,
                        DocumentType.GuarantorDocument));
            if (!guarantorInfoUpdateResult.IsSuccess)
                return Result.Invalid(guarantorInfoUpdateResult.ValidationErrors);

            return await PersonnelCommandRepository.UpdateWithContractAsync(
                    existing,
                    contractResult.Value,
                    cancellationToken)
                ? Result.Success(true)
                : Result.Conflict("بازهٔ قرارداد جدید با قراردادهای قبلی هم‌پوشانی دارد یا تاریخ شروع معتبر نیست.");
        }
        catch (Exception)
        {
            return Result.Error(Messages.ErrorInPersonnel);
        }
    }

    private static bool TryParseStatus(
        string? value,
        bool currentValue,
        out bool status)
    {
        if (value is null)
        {
            status = currentValue;
            return true;
        }

        if (bool.TryParse(value, out status))
            return true;

        status = currentValue;
        return false;
    }

    private static List<Picture> GetDocumentPictures(
        IReadOnlyList<DocumentRequestDto>? documents,
        params DocumentType[] documentTypes)
    {
        return documents?
            .Where(document =>
                documentTypes.Contains(document.DocumentType)
                && !string.IsNullOrWhiteSpace(document.Url))
            .Select(document => Picture.Create(document.Url!).Value)
            .ToList() ?? [];
    }
}

public sealed class UpdatePersonnelCommandValidator : AbstractValidator<UpdatePersonnelCommand>
{
    public UpdatePersonnelCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage(SharedValidationMessages.Required);

        RuleFor(command => command.Request)
            .NotNull()
            .WithMessage(SharedValidationMessages.Required);
    }
}
