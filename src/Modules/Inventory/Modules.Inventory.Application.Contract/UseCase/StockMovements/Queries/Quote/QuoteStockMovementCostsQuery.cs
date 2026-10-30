using Framwork.Bus.Query;
using Modules.Inventory.Application.Contract.DTOs;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record QuoteStockMovementCostsQuery(QuoteStockMovementCostsRequestDto Request)
    : IQuery<QuoteStockMovementCostsResponseDto>;
