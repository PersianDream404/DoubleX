namespace Modules.Service.Application.Contract.DTOs.ServicePersonnels;

public sealed record ServicePersonnelDto(long Id, long SalonId, long ServiceId, long PersonnelId);

public sealed record ServicePersonnelRequestDto(long SalonId, long ServiceId, long PersonnelId);
