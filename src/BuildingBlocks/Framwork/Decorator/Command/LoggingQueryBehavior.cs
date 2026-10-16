using System;
using System.Collections.Generic;
using System.Text;

namespace Framwork.Decorator.Command;

using Ardalis.Result;
using Framwork.Bus.Command;
using Microsoft.Extensions.Logging;



public class LoggingCommandBehavior<TCommand, TResponse>
    : ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ILogger<LoggingCommandBehavior<TCommand, TResponse>> _logger;

    public LoggingCommandBehavior(
        ILogger<LoggingCommandBehavior<TCommand, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(
        TCommand Command,
        CancellationToken cancellationToken,
        Func<Task<Result<TResponse>>> next)
    {
        _logger.LogInformation("Handling Command {Command}", typeof(TCommand).Name);

        var result = await next();

        _logger.LogInformation("Handled Command {Command}", typeof(TCommand).Name);

        return result;
    }
}
