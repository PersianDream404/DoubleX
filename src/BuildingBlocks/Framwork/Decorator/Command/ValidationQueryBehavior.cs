namespace Framwork.Decorator.Query;

using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;

public class ValidationQueryBehavior<TQuery, TResponse>
    : IQueryBehavior<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IEnumerable<IValidator<TQuery>> _validators;

    public ValidationQueryBehavior(IEnumerable<IValidator<TQuery>> validators)
    {
        _validators = validators;
    }

    public async Task<Result<TResponse>> Handle(
        TQuery query,
        CancellationToken cancellationToken,
        Func<Task<Result<TResponse>>> next)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TQuery>(query);

        var failures = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = failures
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (errors.Any())
        {
            var validationErrors = errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
                .ToList();

            return Result.Invalid(validationErrors);
        }

        return await next();
    }
}
