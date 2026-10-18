using Ardalis.Result;
using Framwork.Bus.Command;
using Mapster;
using Modules.Personnel.Application.Common;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;
using Modules.Personnel.Domain.Interface;
using SharedKernel.Helper;

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
            var existing = await PersonnelQueryRepository.GetByIdAsync(command.Id, cancellationToken);
            if (existing is null)
                return Result.Error(Messages.PersonnelNotFound);

            //existing.Update(
            //    command.Request.PersonalInfo.Adapt<Modules.Personnel.Domain.Entities.PersonalInfo>(),
            //    command.Request.JobInfo.Adapt<Modules.Personnel.Domain.Entities.JobInfo>(),
            //    command.Request.FinancialInfo.Adapt<Modules.Personnel.Domain.Entities.FinancialInfo>(),
            //    command.Request.HealthInfo.Adapt<Modules.Personnel.Domain.Entities.HealthInfo>(),
            //    command.Request.GuarantorInfo.Adapt<Modules.Personnel.Domain.Entities.GuarantorInfo>(),
            //    command.Request.Documents.Select(x => x.Adapt<Modules.Personnel.Domain.Entities.PersonnelDocument>()).ToList());

            await PersonnelCommandRepository.UpdateAsync(existing, cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return Result.Error(Messages.ErrorInPersonnel);
        }
    }
}
