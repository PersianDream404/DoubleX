using Ardalis.Result;
using Framwork.Bus.Command;
using Mapster;
using MediatR;
using Modules.Personnel.Application.Common;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;
using Modules.Product.Application.Contract.Interface.Salons;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Enums;
using Modules.Personnel.Domain.Interface;
using Modules.Personnel.Domain.ValueObjects;
using SharedKernel.Helper;

namespace Modules.Personnel.Application.UseCase.Personnel.Commands;

public class CreatePersonnelCommandHandler(
    IPersonnelCommandRepository PersonnelCommandRepository,
    ISalonQueryRepository SalonQueryRepository)
    : ICommandHandler<CreatePersonnelCommand, long>
{
    public async Task<Result<long>> Handle(CreatePersonnelCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (await SalonQueryRepository.GetByIdAsync(command.Request.SalonId, cancellationToken) is null)
                return Result.Invalid(new ValidationError
                {
                    Identifier = nameof(command.Request.SalonId),
                    ErrorMessage = "سالن انتخاب‌شده یافت نشد."
                });

            var personnel = new Domain.Entities.Personnel();
            personnel.Status = command.Request.Status;

            var salonResult = personnel.AssignToSalon(command.Request.SalonId);
            if (!salonResult.IsSuccess)
                return Result.Invalid(salonResult.ValidationErrors);

            var personalInfoRequest = command.Request.PersonalInfo;

            var personalInfoResult = PersonalInfo.Create(
                personalInfoRequest.FullName,
                personalInfoRequest.NationalCode,
                personalInfoRequest.BirthDate,
                personalInfoRequest.MaritalStatus,
                personalInfoRequest.ChildrenCount,
                personalInfoRequest.MobilePhone,
                personalInfoRequest.LandlinePhone,
                personalInfoRequest.AddressText,
                personalInfoRequest.IsCloseToSalon,
                personalInfoRequest.PictureUrl ??
                    command.Request.Documents?
                        .FirstOrDefault(document =>
                            document.DocumentType == DocumentType.Photo)
                        ?.Url);

            if (!personalInfoResult.IsSuccess)
                return Result.Invalid(personalInfoResult.ValidationErrors);

            var updatepersonalInfoResult = personnel.UpdatePersonalInfo(personalInfoResult.Value);
            if (!updatepersonalInfoResult.IsSuccess)
                return updatepersonalInfoResult;

            var jobInfoRequest = command.Request.JobInfo;

            var jobInfoResult = JobInfo.Create(
                jobInfoRequest.JobTitle,
                jobInfoRequest.PersonnelCode,
                jobInfoRequest.CooperationStartDate,
                jobInfoRequest.WorkingShiftType,
                jobInfoRequest.JobPictures is { Count: > 0 }
                    ? jobInfoRequest.JobPictures
                    : jobInfoRequest.Skills is { Count: > 0 }
                        ? jobInfoRequest.Skills
                        : GetDocumentUrls(
                            command.Request.Documents,
                            DocumentType.TrainingCertificate),
                jobInfoRequest.WorkHistory);

            if (!jobInfoResult.IsSuccess)
                return Result.Invalid(jobInfoResult.ValidationErrors);

            var updateJobInfoResult = personnel.UpdateJobInfo(jobInfoResult.Value);
            if (!updateJobInfoResult.IsSuccess)
                return updateJobInfoResult;


            var financialInfoRequest = command.Request.FinancialInfo;

            var financialInfoResult = FinancialInfo.Create(
                financialInfoRequest.SalaryType,
                financialInfoRequest.CardNumber,
                financialInfoRequest.AccountNumber);

            if (!financialInfoResult.IsSuccess)
                return Result.Invalid(financialInfoResult.ValidationErrors);

            var updateFinancialInfoResult = personnel.UpdateFinancialInfo(financialInfoResult.Value);
            if (!updateFinancialInfoResult.IsSuccess)
                return updateFinancialInfoResult;


            var healthInfoRequest = command.Request.HealthInfo;

            if (!TryParseStatus(healthInfoRequest.HealthStatus, out var healthStatus)
                || !TryParseStatus(healthInfoRequest.AllergyStatus, out var allergyStatus))
            {
                return Result.Invalid(new ValidationError
                {
                    Identifier = nameof(command.Request.HealthInfo),
                    ErrorMessage = "وضعیت سلامت و حساسیت باید true یا false باشد."
                });
            }

            var healthInfoResult = HealthInfo.Create(
                healthStatus,
                allergyStatus,
                healthInfoRequest.Diseases,
                healthInfoRequest.Allergies,
                GetDocumentPictures(
                    command.Request.Documents,
                    DocumentType.HealthCourse),
                GetDocumentPictures(
                    command.Request.Documents,
                    DocumentType.MedicalDocument,
                    DocumentType.HealthTest));

            if (!healthInfoResult.IsSuccess)
                return Result.Invalid(healthInfoResult.ValidationErrors);

            var updateHealthInfoResult = personnel.UpdateHealthInfo(healthInfoResult.Value);
            if (!updateHealthInfoResult.IsSuccess)
                return updateHealthInfoResult;


            var guarantorInfoRequest = command.Request.GuarantorInfo;

            var guarantorInfoResult = GuarantorInfo.Create(
                guarantorInfoRequest.GuarantorName,
                guarantorInfoRequest.Relationship,
                guarantorInfoRequest.GuarantorPhone,
                guarantorInfoRequest.GuarantorLandlinePhone,
                GetDocumentPictures(
                    command.Request.Documents,
                    DocumentType.GuarantorDocument));

            if (!guarantorInfoResult.IsSuccess)
                return Result.Invalid(guarantorInfoResult.ValidationErrors);

            var updateGuarantorInfoResult = personnel.UpdateGuarantorInfo(guarantorInfoResult.Value);
            if (!updateGuarantorInfoResult.IsSuccess)
                return updateGuarantorInfoResult;


            await PersonnelCommandRepository.AddAsync(personnel, cancellationToken);

            return Result.Success(personnel.Id);


        }
        catch (Exception)
        {
            return Result.Error(Messages.ErrorInPersonnel);
        }
    }

    private static bool TryParseStatus(string? value, out bool status)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            status = false;
            return true;
        }

        return bool.TryParse(value, out status);
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

    private static List<string> GetDocumentUrls(
        IReadOnlyList<DocumentRequestDto>? documents,
        params DocumentType[] documentTypes)
    {
        return documents?
            .Where(document =>
                documentTypes.Contains(document.DocumentType)
                && !string.IsNullOrWhiteSpace(document.Url))
            .Select(document => document.Url!)
            .ToList() ?? [];
    }
}
