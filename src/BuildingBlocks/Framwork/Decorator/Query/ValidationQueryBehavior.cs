namespace Framwork.Decorator.Command;

using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;

public class ValidationCommandBehavior<TCommand, TResponse>
    : ICommandBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public ValidationCommandBehavior(IEnumerable<IValidator<TCommand>> validators)
    {
        _validators = validators;
    }

    public async Task<Result<TResponse>> Handle(
        TCommand Command,
        CancellationToken cancellationToken,
        Func<Task<Result<TResponse>>> next)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TCommand>(Command);

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
