using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Modules.Personnel.Application.Common;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Interface;

namespace Modules.Personnel.Application.UseCase.Personnel.Commands;

public class UpdatePersonnelCommandHandler(
    IPersonnelCommandRepository PersonnelCommandRepository,
    IPersonnelQueryRepository PersonnelQueryRepository)
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
                request.PersonalInfo.PictureUrl);
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
                personalInfoResult.Value.Picture ?? existing.PersonalInfo.Picture);

            var jobInfoUpdateResult = existing.JobInfo.Update(
                request.JobInfo.JobTitle,
                request.JobInfo.PersonnelCode,
                request.JobInfo.CooperationStartDate,
                request.JobInfo.WorkingShiftType,
                request.JobInfo.Skills,
                request.JobInfo.WorkHistory);
            if (!jobInfoUpdateResult.IsSuccess)
                return Result.Invalid(jobInfoUpdateResult.ValidationErrors);

            var financialInfoUpdateResult = existing.FinancialInfo.Update(
                request.FinancialInfo.SalaryType,
                request.FinancialInfo.CardNumber,
                request.FinancialInfo.AccountNumber);
            if (!financialInfoUpdateResult.IsSuccess)
                return Result.Invalid(financialInfoUpdateResult.ValidationErrors);

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
                request.HealthInfo.Diseases ?? existing.HealthInfo.Diseases,
                request.HealthInfo.Allergies ?? existing.HealthInfo.Allergies,
                existing.HealthInfo.PictureCourse,
                existing.HealthInfo.PictureTest);
            if (!healthInfoUpdateResult.IsSuccess)
                return Result.Invalid(healthInfoUpdateResult.ValidationErrors);

            var guarantorInfoUpdateResult = existing.GuarantorInfo.Update(
                request.GuarantorInfo.GuarantorName,
                request.GuarantorInfo.Relationship,
                request.GuarantorInfo.GuarantorPhone,
                request.GuarantorInfo.GuarantorLandlinePhone,
                existing.GuarantorInfo.PictureGuaranties);
            if (!guarantorInfoUpdateResult.IsSuccess)
                return Result.Invalid(guarantorInfoUpdateResult.ValidationErrors);

            await PersonnelCommandRepository.UpdateAsync(existing, cancellationToken);
            return Result.Success(true);
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
