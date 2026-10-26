using Framwork.Bus.Command;

namespace Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Deactivate;

public sealed record DeactivateServiceCategoryCommand(long SalonId, long Id) : ICommand<bool>;
