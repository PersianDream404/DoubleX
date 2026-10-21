using Ardalis.Result;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framwork.Bus.Command;


public interface ICommand<TResponse>
{
}

public interface ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand Command, CancellationToken cancellationToken);
}
public interface ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(
        TCommand Command,
        CancellationToken cancellationToken,
        Func<Task<Result<TResponse>>> next);
}
