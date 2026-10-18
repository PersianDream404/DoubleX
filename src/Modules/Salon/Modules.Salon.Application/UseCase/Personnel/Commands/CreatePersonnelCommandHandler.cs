using Ardalis.Result;
using Framwork.Bus.Command;
using Mapster;
using MediatR;
using Modules.Personnel.Application.Common;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Interface;
using Modules.Personnel.Domain.ValueObjects;
using SharedKernel.Helper;

namespace Modules.Personnel.Application.UseCase.Personnel.Commands;

public class CreatePersonnelCommandHandler(
    IPersonnelCommandRepository PersonnelCommandRepository)
    : ICommandHandler<CreatePersonnelCommand, long>
{
    public async Task<Result<long>> Handle(CreatePersonnelCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var personnel = new Domain.Entities.Personnel();

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
                false);

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
                jobInfoRequest.Skills,
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

            var healthInfoResult = HealthInfo.Create(
                false,
                false, null, null, [],null);

            if (!healthInfoResult.IsSuccess)
                return Result.Invalid(healthInfoResult.ValidationErrors);

            var updateHealthInfoResult = personnel.UpdateHealthInfo(healthInfoResult.Value);
            if (!updateHealthInfoResult.IsSuccess)
                return updateHealthInfoResult;


            var guarantorInfoRequest = command.Request.GuarantorInfo;

            var guarantorInfoResult = GuarantorInfo.Create(
                guarantorInfoRequest.GuarantorName,
                guarantorInfoRequest.Relationship,
                guarantorInfoRequest.GuarantorPhone, null, []);

            if (!guarantorInfoResult.IsSuccess)
                return Result.Invalid(guarantorInfoResult.ValidationErrors);

            var updateGuarantorInfoResult = personnel.UpdateGuarantorInfo(guarantorInfoResult.Value);
            if (!updateGuarantorInfoResult.IsSuccess)
                return updateGuarantorInfoResult;


            await PersonnelCommandRepository.AttachAsync(personnel, cancellationToken);

            return personnel.Id;


        }
        catch (Exception ex)
        {
            return Result.Error(Messages.ErrorInPersonnel);
        }
    }
}
