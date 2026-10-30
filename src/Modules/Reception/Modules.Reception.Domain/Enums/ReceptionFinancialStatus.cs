using System.Text.Json.Serialization;

namespace Modules.Reception.Domain.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReceptionFinancialStatus
{
    Unpaid = 1,
    PartiallyPaid = 2,
    Paid = 3
}
