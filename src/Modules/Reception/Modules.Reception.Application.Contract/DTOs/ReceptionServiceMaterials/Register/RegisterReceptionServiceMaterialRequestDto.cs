namespace Modules.Reception.Application.Contract.DTOs.ReceptionServiceMaterials.Register;

public sealed record RegisterReceptionServiceMaterialRequestDto(
    long SalonId,
    long MaterialId,
    decimal Quantity,
    string IdempotencyKey,
    string? Description);
