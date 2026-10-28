using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Modules.Finance.Application.Contract.UseCase.Commands;
using Modules.Finance.Domain.Entities;
using Modules.Finance.Domain.Interfaces;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Personnel.Domain.Enums;

namespace Modules.Finance.Application.UseCase.Commands;

public sealed class CalculateFixedSalaryCommandHandler(
    IQueryBus queryBus,
    IFinanceCommandRepository commandRepository)
    : ICommandHandler<CalculateFixedSalaryCommand, long>
{
    public async Task<Result<long>> Handle(
        CalculateFixedSalaryCommand command,
        CancellationToken cancellationToken)
    {
        var personnelResult = await queryBus.Send<GetPersonnelByIdQuery, GetByIdPersonnelResponseDto>(
            new GetPersonnelByIdQuery(command.PersonnelId),
            cancellationToken);
        if (!personnelResult.IsSuccess ||
            personnelResult.Value.SalonId != command.SalonId ||
            !personnelResult.Value.Status)
        {
            return Result.NotFound("پرسنل فعال متعلق به این سالن پیدا نشد.");
        }

        var contract = personnelResult.Value.Contracts.FirstOrDefault(item =>
            item.EffectiveFrom <= command.PeriodStart &&
            (!item.EffectiveTo.HasValue || item.EffectiveTo.Value >= command.PeriodEnd));
        if (contract is null ||
            contract.CooperationType is not (CooperationType.FixedSalary or CooperationType.FixedAndCommission))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(command.PersonnelId),
                ErrorMessage = "برای این پرسنل حقوق ثابت تعریف نشده است."
            });
        }

        if (!contract.FixedSalaryAmount.HasValue || contract.FixedSalaryAmount.Value <= 0)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(command.PeriodStart),
                ErrorMessage = "مبلغ حقوق و بازهٔ درخواست باید دقیقاً با بازهٔ معتبر قرارداد در پروفایل پرسنل تطبیق داشته باشد."
            });
        }

        var idempotencyKey =
            $"fixed-salary:{command.PersonnelId}:{command.PeriodStart:yyyyMMdd}:{command.PeriodEnd:yyyyMMdd}";
        var earning = PersonnelEarning.Create(
            command.SalonId,
            command.PersonnelId,
            null,
            null,
            PersonnelEarningType.FixedSalary,
            contract.FixedSalaryAmount.Value,
            null,
            contract.FixedSalaryAmount.Value,
            command.PeriodStart,
            command.PeriodEnd,
            new DateTimeOffset(command.PeriodEnd.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero),
            idempotencyKey,
            "حقوق ثابت دورهٔ اعتبار قرارداد");

        if (!await commandRepository.AddEarningAsync(earning, cancellationToken))
        {
            return Result.Conflict("حقوق این دوره قبلاً محاسبه شده یا ثبت آن ناموفق بوده است.");
        }

        return Result.Success(earning.Id);
    }
}

public sealed class CalculateFixedSalaryCommandValidator
    : AbstractValidator<CalculateFixedSalaryCommand>
{
    public CalculateFixedSalaryCommandValidator()
    {
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.PersonnelId).GreaterThan(0);
        RuleFor(command => command.PeriodEnd)
            .GreaterThanOrEqualTo(command => command.PeriodStart);
    }
}
