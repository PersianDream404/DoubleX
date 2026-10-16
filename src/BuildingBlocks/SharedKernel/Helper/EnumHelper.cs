using Shared.Contract.DTOs.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SharedKernel.Helper;

public static class EnumHelper
{
    public static List<EnumItemDto> GetItems<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>()
            .Select(x => new EnumItemDto
            {
                DisplayName = GetDisplayName(x),
                Value = Convert.ToInt32(x)
            })
            .ToList();
    }

    private static string GetDisplayName<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var member = typeof(TEnum).GetMember(value.ToString()).FirstOrDefault();
        var displayAttribute = member?.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? value.ToString();
    }
}
