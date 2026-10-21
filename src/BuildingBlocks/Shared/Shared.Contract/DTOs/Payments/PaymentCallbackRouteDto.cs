using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contract.DTOs.Payments;

public sealed record PaymentCallbackRouteDto
{
    public string Action { get; init; } = default!;
    public string Controller { get; init; } = default!;
    public string? Area { get; init; }

    public Dictionary<string, object?> RouteValues { get; init; } = new();
}

