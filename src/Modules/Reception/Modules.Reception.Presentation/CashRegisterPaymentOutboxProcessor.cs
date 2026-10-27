using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;
using Modules.Reception.Application.Contract.Interfaces;

namespace Modules.Reception.Presentation;

public sealed class CashRegisterPaymentOutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<CashRegisterPaymentOutboxProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var processedAny = await ProcessPendingBatchAsync(stoppingToken);
            if (!processedAny)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }

    private async Task<bool> ProcessPendingBatchAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICashRegisterPaymentOutboxRepository>();
        var commandBus = scope.ServiceProvider.GetRequiredService<ICommandBus>();
        var messages = await repository.GetPendingAsync(
            50, DateTimeOffset.UtcNow, cancellationToken);
        foreach (var message in messages)
        {
            try
            {
                var result = await commandBus.Send<RegisterReceptionPaymentTransactionCommand, bool>(
                    new RegisterReceptionPaymentTransactionCommand(
                        message.SalonId,
                        message.CashRegisterId,
                        message.CashRegisterSessionId,
                        message.ReceptionId,
                        message.UserId,
                        message.Amount,
                        message.OccurredAt,
                        message.IdempotencyKey,
                        message.Description),
                    cancellationToken);
                if (result.IsSuccess)
                {
                    await repository.MarkProcessedAsync(
                        message.Id, DateTimeOffset.UtcNow, cancellationToken);
                    continue;
                }

                var error = result.GetErrorMessage();
                logger.LogWarning(
                    "Cash register payment outbox item {OutboxId} could not be applied: {Error}",
                    message.Id,
                    error);
                await repository.MarkFailedAsync(
                    message.Id, error, DateTimeOffset.UtcNow, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Cash register payment outbox item {OutboxId} failed unexpectedly.",
                    message.Id);
                await repository.MarkFailedAsync(
                    message.Id, exception.Message, DateTimeOffset.UtcNow, cancellationToken);
            }
        }

        return messages.Count > 0;
    }
}
