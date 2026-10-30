using Modules.Reception.Domain.ReadModels;

namespace Modules.Reception.Domain.Interfaces;

public interface IReceptionReportQueryRepository
{
    Task<ReceptionReportSummaryReadModel> GetSummaryAsync(
        ReceptionReportFilter filter,
        CancellationToken cancellationToken);

    Task<ReceptionReportPageReadModel<ReceptionReportReceptionReadModel, ReceptionReportTotalsReadModel>>
        GetReceptionsAsync(ReceptionReportFilter filter, CancellationToken cancellationToken);

    Task<ReceptionReportPageReadModel<ReceptionReportPersonnelReadModel, ReceptionReportTotalsReadModel>>
        GetPersonnelAsync(ReceptionReportFilter filter, CancellationToken cancellationToken);

    Task<ReceptionReportPageReadModel<ReceptionReportServiceReadModel, ReceptionReportTotalsReadModel>>
        GetServicesAsync(ReceptionReportFilter filter, CancellationToken cancellationToken);

    Task<ReceptionReportPageReadModel<ReceptionReportDebtReadModel, ReceptionReportDebtTotalsReadModel>>
        GetDebtsAsync(ReceptionReportFilter filter, CancellationToken cancellationToken);

    Task<ReceptionReportPageReadModel<ReceptionReportDiscountReadModel, ReceptionReportDiscountTotalsReadModel>>
        GetDiscountsAsync(ReceptionReportFilter filter, CancellationToken cancellationToken);

    Task<ReceptionReportPageReadModel<ReceptionReportMaterialReadModel, ReceptionReportMaterialTotalsReadModel>>
        GetMaterialsAsync(ReceptionReportFilter filter, CancellationToken cancellationToken);
}
