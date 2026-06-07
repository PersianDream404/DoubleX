using Ardalis.Result;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framwork.Bus.Query;


public interface IQuery<TResponse>
{
}

public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
public interface IQueryBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(
        TQuery query,
        CancellationToken cancellationToken,
        Func<Task<Result<TResponse>>> next);
}
