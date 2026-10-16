using System;
using System.Collections.Generic;
using System.Text;

namespace Framwork.Decorator.Query;

using Ardalis.Result;
using Framwork.Bus.Query;
using Microsoft.Extensions.Logging;



public class LoggingQueryBehavior<TQuery, TResponse>
    : IQueryBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly ILogger<LoggingQueryBehavior<TQuery, TResponse>> _logger;

    public LoggingQueryBehavior(
        ILogger<LoggingQueryBehavior<TQuery, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(
        TQuery query,
        CancellationToken cancellationToken,
        Func<Task<Result<TResponse>>> next)
    {
        _logger.LogInformation("Handling query {Query}", typeof(TQuery).Name);

        var result = await next();

        _logger.LogInformation("Handled query {Query}", typeof(TQuery).Name);

        return result;
    }
}
