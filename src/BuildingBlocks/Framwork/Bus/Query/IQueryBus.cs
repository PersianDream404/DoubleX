using Ardalis.Result;
using Microsoft.Extensions.DependencyInjection;

namespace Framwork.Bus.Query;

public interface IQueryBus
{
    Task<Result<TResponse>> Send<TQuery, TResponse>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>;
}
public class QueryBus : IQueryBus
{
    private readonly IServiceProvider _provider;

    public QueryBus(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<Result<TResponse>> Send<TQuery, TResponse>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>
    {
        var handler = _provider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();

        var behaviors = _provider
            .GetServices<IQueryBehavior<TQuery, TResponse>>()
            .Reverse()
            .ToList();

        Func<Task<Result<TResponse>>> handle =
            () => handler.Handle(query, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var next = handle;

            handle = () => behavior.Handle(
                query,
                cancellationToken,
                next);
        }

        return await handle();
    }
}
