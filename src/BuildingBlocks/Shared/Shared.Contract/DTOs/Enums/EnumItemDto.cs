using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contract.DTOs.Enums;

public class EnumItemDto
{
    public string DisplayName { get; set; } = default!;
    public int Value { get; set; }
}
