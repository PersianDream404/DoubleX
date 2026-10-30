using Framwork.PagedList;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.Contract.DTOs.Reports;

public abstract class ReceptionReportPagedRequestDto : PagedParamData
{
    public long? SalonId { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDateExclusive { get; set; }
    public string? CustomerSearch { get; set; }
    public long? CustomerId { get; set; }
    public long? ReceptionNumber { get; set; }
    public long? PersonnelId { get; set; }
    public long? ServiceId { get; set; }
    public long? MaterialId { get; set; }
    public ReceptionStatus? ReceptionStatus { get; set; }
    public ReceptionFinancialStatus? FinancialStatus { get; set; }
    public ReceptionServiceStatus? ServiceStatus { get; set; }
    public bool? HasAppointment { get; set; }
    public decimal? MinimumRemainingAmount { get; set; }
    public decimal? MaximumRemainingAmount { get; set; }
    public bool? OldestFirst { get; set; }
}

public sealed class GetReceptionReportSummaryRequestDto
{
    public long? SalonId { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDateExclusive { get; set; }
}

public sealed class GetReceptionReportReceptionsRequestDto : ReceptionReportPagedRequestDto { }
public sealed class GetReceptionReportPersonnelRequestDto : ReceptionReportPagedRequestDto { }
public sealed class GetReceptionReportServicesRequestDto : ReceptionReportPagedRequestDto { }
public sealed class GetReceptionReportDebtsRequestDto : ReceptionReportPagedRequestDto { }
public sealed class GetReceptionReportDiscountsRequestDto : ReceptionReportPagedRequestDto { }
public sealed class GetReceptionReportMaterialsRequestDto : ReceptionReportPagedRequestDto { }
