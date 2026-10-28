using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Modules.Finance.Application.Contract.Interfaces;
using Modules.Finance.Application.Contract.UseCase.Commands;
using Modules.Finance.Domain.Entities;
using Modules.Finance.Domain.Interfaces;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Personnel.Domain.Enums;
using Modules.Reception.Application.Contract.DTOs.Receptions.Get;
using Modules.Reception.Application.Contract.UseCase.Receptions.Queries.GetById;
using Modules.Reception.Domain.Entities;

namespace Modules.Finance.Application.UseCase.Commands;

public sealed class CalculateReceptionEarningsCommandHandler(
    IQueryBus queryBus,
    IFinanceCommandRepository commandRepository,
    IFinanceQueryRepository queryRepository)
    : ICommandHandler<CalculateReceptionEarningsCommand, int>
{
    public async Task<Result<int>> Handle(
        CalculateReceptionEarningsCommand command,
        CancellationToken cancellationToken)
    {
        var receptionResult = await queryBus.Send<GetReceptionByIdQuery, ReceptionDetailsDto>(
            new GetReceptionByIdQuery(command.ReceptionId, command.SalonId),
            cancellationToken);
        if (!receptionResult.IsSuccess)
        {
            return Result.NotFound("پذیرش متعلق به این سالن پیدا نشد.");
        }

        var reception = receptionResult.Value;
        if (reception.Status == ReceptionStatus.Cancelled)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(command.ReceptionId),
                ErrorMessage = "برای پذیرش لغوشده سهمی محاسبه نمی‌شود."
            });
        }

        var createdCount = 0;
        foreach (var service in reception.Services.Where(item =>
                     item.Status == ReceptionServiceStatus.Completed &&
                     (!command.ReceptionServiceId.HasValue ||
                      item.Id == command.ReceptionServiceId.Value)))
        {
            if (!service.PersonnelId.HasValue)
            {
                continue;
            }

            var personnelResult = await queryBus.Send<GetPersonnelByIdQuery, GetByIdPersonnelResponseDto>(
                new GetPersonnelByIdQuery(service.PersonnelId.Value),
                cancellationToken);
            if (!personnelResult.IsSuccess ||
                personnelResult.Value.SalonId != command.SalonId ||
                !personnelResult.Value.Status)
            {
                return Result.Invalid(new ValidationError
                {
                    Identifier = nameof(service.PersonnelId),
                    ErrorMessage = "پرسنل خدمت معتبر یا فعال نیست."
                });
            }

            var serviceDate = DateOnly.FromDateTime(reception.ReceptionDate.UtcDateTime);
            var contract = personnelResult.Value.Contracts.FirstOrDefault(item =>
                item.EffectiveFrom <= serviceDate &&
                (!item.EffectiveTo.HasValue || item.EffectiveTo.Value >= serviceDate));
            if (contract is null ||
                contract.CooperationType is not (CooperationType.Commission or CooperationType.FixedAndCommission))
            {
                continue;
            }

            var commissionRate = contract.DefaultCommissionRate;
            if (!commissionRate.HasValue ||
                commissionRate.Value is < 0 or > 100)
            {
                return Result.Invalid(new ValidationError
                {
                    Identifier = nameof(contract.DefaultCommissionRate),
                    ErrorMessage = "نرخ پورسانت در پروفایل پرسنل تعریف نشده یا نامعتبر است."
                });
            }

            var idempotencyKey = $"reception-service:{service.Id}:commission";
            if (await queryRepository.EarningExistsAsync(idempotencyKey, command.SalonId, cancellationToken))
            {
                continue;
            }

            var amount = decimal.Round(
                service.FinalAmount * commissionRate.Value / 100m,
                2,
                MidpointRounding.AwayFromZero);
            if (amount <= 0)
            {
                continue;
            }

            var earning = PersonnelEarning.Create(
                command.SalonId,
                service.PersonnelId.Value,
                reception.Id,
                service.Id,
                PersonnelEarningType.ServiceCommission,
                service.FinalAmount,
                commissionRate,
                amount,
                null,
                null,
                reception.ReceptionDate,
                idempotencyKey,
                $"پورسانت خدمت: {service.ServiceName}");

            if (!await commandRepository.AddEarningAsync(earning, cancellationToken))
            {
                if (await queryRepository.EarningExistsAsync(
                        idempotencyKey,
                        command.SalonId,
                        cancellationToken))
                {
                    continue;
                }

                return Result.Conflict("ثبت سهم خدمت انجام نشد؛ عملیات را دوباره بررسی کنید.");
            }

            createdCount++;
        }

        return Result.Success(createdCount);
    }
}

public sealed class CalculateReceptionEarningsCommandValidator
    : AbstractValidator<CalculateReceptionEarningsCommand>
{
    public CalculateReceptionEarningsCommandValidator()
    {
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.ReceptionId).GreaterThan(0);
        RuleFor(command => command.ReceptionServiceId)
            .GreaterThan(0)
            .When(command => command.ReceptionServiceId.HasValue);
    }
}
