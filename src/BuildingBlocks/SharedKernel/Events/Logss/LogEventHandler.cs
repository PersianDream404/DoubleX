namespace SharedKernel.Events.Logs;

using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Events.Logs;

public sealed class LogEventHandler(
    ILogger<LogEventHandler> logger
) : INotificationHandler<LogEventRequest>
{
    public async Task Handle(
        LogEventRequest notification,
        CancellationToken ct)
    {
        logger.LogInformation("test");
        Console.WriteLine("****************************");
        Console.WriteLine("Log By Event");
        Console.WriteLine("****************************");
    }
}


