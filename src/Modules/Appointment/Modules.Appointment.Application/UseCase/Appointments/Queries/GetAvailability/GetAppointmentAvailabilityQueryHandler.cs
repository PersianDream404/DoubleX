using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Appointment.Application.Contract.DTOs.Appointments;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Queries.GetAvailability;
using Modules.Appointment.Domain.Interfaces;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetAll;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Queries.GetById;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetActive;

namespace Modules.Appointment.Application.UseCase.Appointments.Queries.GetAvailability;

public sealed class GetAppointmentAvailabilityQueryHandler(
    IAppointmentCommandRepository repository,
    IQueryBus queryBus)
    : IQueryHandler<GetAppointmentAvailabilityQuery, AppointmentAvailabilityDto>
{
    public async Task<Result<AppointmentAvailabilityDto>> Handle(
        GetAppointmentAvailabilityQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var servicesResult = await queryBus.Send<GetActiveServicesQuery, IReadOnlyList<ServiceDto>>(
            new GetActiveServicesQuery(query.SalonId),
            cancellationToken);
        if (!servicesResult.IsSuccess)
            return Result.Invalid(new ValidationError("دریافت خدمات فعال سالن ناموفق بود."));
        var service = servicesResult.Value.SingleOrDefault(item =>
            item.Id == request.ServiceId && item.SalonId == query.SalonId && item.IsActive);
        if (service is null)
            return Result.Invalid(new ValidationError("خدمت فعال و متعلق به سالن انتخاب نشده است."));

        var tariffResult = await queryBus.Send<GetServiceTariffByIdQuery, ServiceTariffDto>(
            new GetServiceTariffByIdQuery(request.ServiceTariffId),
            cancellationToken);
        if (!tariffResult.IsSuccess ||
            tariffResult.Value.SalonId != query.SalonId ||
            tariffResult.Value.ValidFrom > request.StartsAt ||
            (tariffResult.Value.ValidTo.HasValue &&
             tariffResult.Value.ValidTo.Value < request.StartsAt))
        {
            return Result.Invalid(new ValidationError("تعرفه در تاریخ نوبت معتبر نیست."));
        }
        var tariffItem = tariffResult.Value.Services.SingleOrDefault(item =>
            item.ServiceId == request.ServiceId);
        if (tariffItem is null || tariffItem.Amount < 0)
            return Result.Invalid(new ValidationError("خدمت در تعرفه انتخاب‌شده وجود ندارد."));

        var assignmentsResult = await queryBus.Send<
            GetServicePersonnelsQuery,
            IReadOnlyList<ServicePersonnelDto>>(
            new GetServicePersonnelsQuery(query.SalonId, request.ServiceId),
            cancellationToken);
        if (!assignmentsResult.IsSuccess ||
            !assignmentsResult.Value.Any(item => item.PersonnelId == request.PersonnelId))
        {
            return Result.Invalid(new ValidationError("پرسنل انتخاب‌شده برای این خدمت مجاز نیست."));
        }

        var personnelResult = await queryBus.Send<
            GetPersonnelByIdQuery,
            GetByIdPersonnelResponseDto>(
            new GetPersonnelByIdQuery(request.PersonnelId),
            cancellationToken);
        if (!personnelResult.IsSuccess ||
            !personnelResult.Value.Status ||
            personnelResult.Value.SalonId != query.SalonId)
        {
            return Result.Invalid(new ValidationError("پرسنل فعال و متعلق به سالن انتخاب نشده است."));
        }

        var startsAt = request.StartsAt.ToUniversalTime();
        if (startsAt <= DateTimeOffset.UtcNow)
            return Result.Invalid(new ValidationError("زمان نوبت باید در آینده باشد."));
        var endsAt = startsAt.AddMinutes(service.DurationMinutes);
        var available = await repository.IsAvailableAsync(
            query.SalonId,
            request.PersonnelId,
            startsAt,
            endsAt,
            null,
            cancellationToken);
        return Result.Success(new AppointmentAvailabilityDto(
            available,
            startsAt,
            endsAt,
            service.DurationMinutes,
            tariffItem.Amount));
    }
}

public sealed class GetAppointmentAvailabilityQueryValidator
    : AbstractValidator<GetAppointmentAvailabilityQuery>
{
    public GetAppointmentAvailabilityQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.Request).NotNull();
        When(query => query.Request is not null, () =>
        {
            RuleFor(query => query.Request.ServiceId).GreaterThan(0);
            RuleFor(query => query.Request.ServiceTariffId).GreaterThan(0);
            RuleFor(query => query.Request.PersonnelId).GreaterThan(0);
            RuleFor(query => query.Request.StartsAt)
                .Must(value => value > DateTimeOffset.UtcNow)
                .WithMessage("زمان نوبت باید در آینده باشد.");
        });
    }
}
