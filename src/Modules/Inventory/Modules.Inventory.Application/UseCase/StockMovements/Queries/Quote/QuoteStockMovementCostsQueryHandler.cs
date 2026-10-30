using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Inventory.Application.Contract.DTOs;
using Modules.Inventory.Application.Contract.Interfaces.StockMovements;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Enums;

namespace Modules.Inventory.Application.UseCase.StockMovements.Queries.Quote;

public sealed class QuoteStockMovementCostsQueryHandler(
    IStockMovementCostQueryRepository repository)
    : IQueryHandler<QuoteStockMovementCostsQuery, QuoteStockMovementCostsResponseDto>
{
    public async Task<Result<QuoteStockMovementCostsResponseDto>> Handle(
        QuoteStockMovementCostsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await repository.QuoteAsync(query.Request, cancellationToken);
        return result.Status switch
        {
            InventoryOperationStatus.Succeeded => Result.Success(result.Quote!),
            InventoryOperationStatus.NotFound => Result.NotFound("یکی از مواد انتخاب‌شده پیدا نشد."),
            InventoryOperationStatus.Inactive =>
                Result.Error("برای مادهٔ غیرفعال قیمت قابل محاسبه نیست."),
            InventoryOperationStatus.InsufficientStock =>
                Result.Error("موجودی یکی از مواد برای مقدار واردشده کافی نیست."),
            InventoryOperationStatus.HistoricalDataConflict =>
                Result.Error("بهای FIFO موجودی کافی نیست؛ موجودی بدون قیمت خرید را تعیین تکلیف کنید."),
            InventoryOperationStatus.InvalidInput =>
                Result.Invalid(new ValidationError("اطلاعات محاسبهٔ بهای مواد معتبر نیست.")),
            _ => Result.Error("محاسبهٔ بهای FIFO مواد انجام نشد.")
        };
    }
}

public sealed class QuoteStockMovementCostsQueryValidator
    : AbstractValidator<QuoteStockMovementCostsQuery>
{
    public QuoteStockMovementCostsQueryValidator()
    {
        RuleFor(query => query.Request).NotNull();
        When(query => query.Request is not null, () =>
        {
            RuleFor(query => query.Request.Items).NotNull();
            When(query => query.Request.Items is not null, () =>
            {
                RuleFor(query => query.Request.Items)
                    .NotEmpty();
                RuleForEach(query => query.Request.Items).ChildRules(item =>
                {
                    item.RuleFor(value => value.Key).NotEmpty().MaximumLength(100);
                    item.RuleFor(value => value.MaterialId).GreaterThan(0);
                    item.RuleFor(value => value.Quantity)
                        .GreaterThan(0)
                        .LessThanOrEqualTo(999999999999999.999m)
                        .Must(value => decimal.Round(value, 3) == value);
                });
                RuleFor(query => query.Request.Items)
                    .Must(items => items.Select(item => item.Key).Distinct().Count() == items.Count)
                    .WithMessage("شناسه‌های ردیف مواد باید یکتا باشند.");
            });
        });
    }
}
