using Framwork.Bus.Command;

namespace Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Delete;

public sealed record DeleteServiceCategoryCommand(long SalonId, long Id) : ICommand<bool>;
