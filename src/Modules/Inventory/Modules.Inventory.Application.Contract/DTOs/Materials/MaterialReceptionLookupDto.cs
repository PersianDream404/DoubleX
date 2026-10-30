namespace Modules.Inventory.Application.Contract.DTOs.Materials;

public sealed record MaterialReceptionLookupDto(
    long Id,
    string Name,
    string BaseUnitName,
    string BaseUnitSymbol);
