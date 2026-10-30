using Framwork.Bus.Query;
using Modules.Reception.Application.Contract.DTOs.Reports;

namespace Modules.Reception.Application.Contract.UseCase.Reports.Queries;

public sealed record GetReceptionReportSummaryQuery(GetReceptionReportSummaryRequestDto Request)
    : IQuery<ReceptionReportSummaryDto>;

public sealed record GetReceptionReportReceptionsQuery(GetReceptionReportReceptionsRequestDto Request)
    : IQuery<ReceptionReportPagedDto<ReceptionReportReceptionDto, ReceptionReportTotalsDto>>;

public sealed record GetReceptionReportPersonnelQuery(GetReceptionReportPersonnelRequestDto Request)
    : IQuery<ReceptionReportPagedDto<ReceptionReportPersonnelDto, ReceptionReportTotalsDto>>;

public sealed record GetReceptionReportServicesQuery(GetReceptionReportServicesRequestDto Request)
    : IQuery<ReceptionReportPagedDto<ReceptionReportServiceDto, ReceptionReportTotalsDto>>;

public sealed record GetReceptionReportDebtsQuery(GetReceptionReportDebtsRequestDto Request)
    : IQuery<ReceptionReportPagedDto<ReceptionReportDebtDto, ReceptionReportDebtTotalsDto>>;

public sealed record GetReceptionReportDiscountsQuery(GetReceptionReportDiscountsRequestDto Request)
    : IQuery<ReceptionReportPagedDto<ReceptionReportDiscountDto, ReceptionReportDiscountTotalsDto>>;

public sealed record GetReceptionReportMaterialsQuery(GetReceptionReportMaterialsRequestDto Request)
    : IQuery<ReceptionReportPagedDto<ReceptionReportMaterialDto, ReceptionReportMaterialTotalsDto>>;
