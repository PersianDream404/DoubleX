using System;
using System.Collections.Generic;
using System.Text;

namespace Framwork.Extensions;

using Ardalis.Result;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
public static class stringExtensions
{
    public static string NormalizePhoneNumber(this string phoneNumber)
    {
        phoneNumber = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

        if (phoneNumber.StartsWith("+98"))
            phoneNumber = "0" + phoneNumber.Substring(3);

        return phoneNumber;
    }
}

public static class FluentValidationConfig
{
    public static void Configure()
    {
        ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
        {
            if (member != null)
            {
                var display = member.GetCustomAttribute<DisplayAttribute>();

                if (display != null)
                    return display.GetName();
            }

            return member?.Name;
        };
    }
}
