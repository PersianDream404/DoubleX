using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modules.Finance.Application.Contract.UseCase.Commands;
using Modules.Reception.Application.Contract.Interfaces;

namespace Modules.Reception.Presentation;

public sealed class PersonnelCommissionOutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<PersonnelCommissionOutboxProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processedAny = await ProcessPendingBatchAsync(stoppingToken);
                if (!processedAny)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Personnel commission outbox processing failed.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task<bool> ProcessPendingBatchAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider
            .GetRequiredService<IPersonnelCommissionOutboxRepository>();
        var commandBus = scope.ServiceProvider.GetRequiredService<ICommandBus>();
        var messages = await repository.GetPendingAsync(
            50,
            DateTimeOffset.UtcNow,
            cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var result = await commandBus.Send<CalculateReceptionEarningsCommand, int>(
                    new CalculateReceptionEarningsCommand(
                        message.SalonId,
                        message.ReceptionId,
                        message.ReceptionServiceId),
                    cancellationToken);
                if (result.IsSuccess)
                {
                    await repository.MarkProcessedAsync(
                        message.Id,
                        DateTimeOffset.UtcNow,
                        cancellationToken);
                    logger.LogInformation(
                        "Processed personnel commission outbox event {EventId} for reception service {ReceptionServiceId}; created {CreatedCount} earning(s).",
                        message.EventId,
                        message.ReceptionServiceId,
                        result.Value);
                    continue;
                }

                var error = result.GetErrorMessage();
                logger.LogWarning(
                    "Personnel commission outbox item {OutboxId} could not be processed: {Error}",
                    message.Id,
                    error);
                await repository.MarkFailedAsync(
                    message.Id,
                    error,
                    DateTimeOffset.UtcNow,
                    cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Personnel commission outbox item {OutboxId} failed unexpectedly.",
                    message.Id);
                await repository.MarkFailedAsync(
                    message.Id,
                    exception.Message,
                    DateTimeOffset.UtcNow,
                    cancellationToken);
            }
        }

        return messages.Count > 0;
    }
}
