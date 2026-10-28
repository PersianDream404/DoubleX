using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Modules.Finance.Application.Contract.DTOs;
using Modules.Finance.Application.Contract.Interfaces;
using Modules.Finance.Application.Contract.UseCase.Commands;
using Modules.Finance.Domain.Entities;
using Modules.Finance.Domain.Interfaces;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;

namespace Modules.Finance.Application.UseCase.Commands;

public sealed class RegisterPersonnelSettlementCommandHandler(
    IQueryBus queryBus,
    IFinanceCommandRepository commandRepository,
    IFinanceQueryRepository queryRepository)
    : ICommandHandler<RegisterPersonnelSettlementCommand, long>
{
    public async Task<Result<long>> Handle(
        RegisterPersonnelSettlementCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var personnelResult = await queryBus.Send<GetPersonnelByIdQuery, GetByIdPersonnelResponseDto>(
            new GetPersonnelByIdQuery(request.PersonnelId),
            cancellationToken);
        if (!personnelResult.IsSuccess ||
            personnelResult.Value.SalonId != command.SalonId ||
            !personnelResult.Value.Status)
        {
            return Result.NotFound("پرسنل فعال متعلق به این سالن پیدا نشد.");
        }

        if (await queryRepository.SettlementExistsAsync(
                request.IdempotencyKey,
                command.SalonId,
                cancellationToken))
        {
            return Result.Conflict("این تسویه قبلاً ثبت شده است.");
        }

        var settlement = PersonnelSettlement.Create(
            command.SalonId,
            request.PersonnelId,
            command.UserId,
            request.EarningType,
            request.Amount,
            DateTimeOffset.UtcNow,
            request.PaymentMethod,
            request.IdempotencyKey,
            request.Description);
        if (!await commandRepository.AddSettlementWithinBalanceAsync(settlement, cancellationToken))
        {
            return Result.Conflict("مبلغ تسویه از ماندهٔ قابل پرداخت بیشتر است یا تسویهٔ تکراری ثبت شده است.");
        }

        return Result.Success(settlement.Id);
    }
}

public sealed class RegisterPersonnelSettlementCommandValidator
    : AbstractValidator<RegisterPersonnelSettlementCommand>
{
    public RegisterPersonnelSettlementCommandValidator()
    {
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        RuleFor(command => command.Request.PersonnelId).GreaterThan(0);
        RuleFor(command => command.Request.EarningType).IsInEnum();
        RuleFor(command => command.Request.Amount)
            .GreaterThan(0)
            .Must(amount => decimal.Round(amount, 2) == amount);
        RuleFor(command => command.Request.PaymentMethod).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Request.IdempotencyKey).NotEmpty().MaximumLength(100);
        RuleFor(command => command.Request.Description).MaximumLength(1000);
    }
}
