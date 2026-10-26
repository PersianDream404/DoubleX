using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.Contract.DTOs.Receptions.Update;

public sealed record ChangeReceptionStatusRequestDto(long SalonId, ReceptionStatus Status);
