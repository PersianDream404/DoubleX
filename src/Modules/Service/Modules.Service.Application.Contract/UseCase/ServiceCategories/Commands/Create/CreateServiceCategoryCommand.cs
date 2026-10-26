using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;

namespace Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Create;

public sealed record CreateServiceCategoryCommand(ServiceCategoryRequestDto Request) : ICommand<bool>;
