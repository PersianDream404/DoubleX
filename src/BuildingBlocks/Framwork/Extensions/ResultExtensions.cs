using System;
using System.Collections.Generic;
using System.Text;

namespace Framwork.Extensions;

using Ardalis.Result;
using System.Text;

public static class ResultExtensions
{
    public static string GetErrorMessage<T>(this Result<T> result)
    {
        var errors = new List<string>();

        if (result.Errors != null && result.Errors.Any())
            errors.AddRange(result.Errors);

        if (result.ValidationErrors != null && result.ValidationErrors.Any())
            errors.AddRange(result.ValidationErrors.Select(x => x.ErrorMessage));

        return string.Join(" | ", errors);
    }
}
