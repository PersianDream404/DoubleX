using Ardalis.Result;
using Microsoft.Extensions.DependencyInjection;

namespace Framwork.Bus.Command;

public interface ICommandBus
{
    Task<Result<TResponse>> Send<TCommand, TResponse>(
        TCommand Command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>;
}
public class CommandBus : ICommandBus
{
    private readonly IServiceProvider _provider;

    public CommandBus(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<Result<TResponse>> Send<TCommand, TResponse>(
        TCommand Command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>
    {
        var handler = _provider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

        var behaviors = _provider
            .GetServices<ICommandBehavior<TCommand, TResponse>>()
            .Reverse()
            .ToList();

        Func<Task<Result<TResponse>>> handle =
            () => handler.Handle(Command, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var next = handle;

            handle = () => behavior.Handle(
                Command,
                cancellationToken,
                next);
        }

        return await handle();
    }
}
